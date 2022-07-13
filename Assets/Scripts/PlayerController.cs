using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;

    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    [SerializeField] GameObject cameraHolder;

    [SerializeField] Animator animator;

    [SerializeField] private float animationMultiplier = 0.2f;

    private readonly string moveAnimation = "isMoving";
    private readonly string deathAnimation = "isDead";
    private readonly string shootAnimation = "isShooting";

    private readonly string moveAnimationSpeedMultiplier = "moveMultiplier";

    private float horizontalRaw;
    private float verticalRaw;

    bool isSprinting;

    bool grounded;

    [SerializeField] Item[] items;

    int itemIndex;
    int previousItemIndex = -1;

    float verticalLookRotation;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;

    PhotonView PV;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager playerManager;

    private int currentItem;

    public bool isPickedUp = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }
    private void Start()
    {
        if (PV.IsMine)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            //Destroy(rb);
            Destroy(ui);
        }
    }

    private void Update()
    {
        if (isPickedUp)
            return;

        if (!PV.IsMine)
            return;

        GetInput();

        Look();
        Move();
        Jump();


        if(currentItem != 2 && SpecialityGun.grabbedRB != null)
        {
            if(SpecialityGun.grabbedRB.gameObject.GetComponent<PlayerController>() != null)
            {
                SpecialityGun.grabbedRB.gameObject.GetComponent<PlayerController>().isPickedUp = false;
            }
            SpecialityGun.grabbedRB.isKinematic = false;
            SpecialityGun.grabbedRB = null;
        }

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Choose item by num keys
        for(int i = 0; i < items.Length; i++)
        {
            if(Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                currentItem = i;
                break;
            }
        }

        //Choose item by scrolling up
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if(itemIndex >= items.Length - 1)
            {
                EquipItem(0);
                currentItem = 0;
            }
            else
            {
                EquipItem(itemIndex + 1);
                currentItem = itemIndex +1;
            }
        }

        //Choose item by scrolling down
        else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if(itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
                currentItem = items.Length - 1;
            }
            else
            {
                EquipItem(itemIndex - 1);
                currentItem = itemIndex - 1;
            }
        }

        if (Input.GetMouseButton(0))
        {
            items[itemIndex].UsePrimary();
        }


        if (Input.GetMouseButtonDown(1))
        {
            items[itemIndex].UseSecondary();
        }


        //Die if you fall in void
        if(transform.position.y < -10f)
        {
            Die();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

    }

    private void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;

    }

    private void Move()
    {
        Vector3 moveDir = new Vector3(horizontalRaw, 0, verticalRaw).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (isSprinting ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);

    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }

    }

    private void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;

        items[itemIndex].itemGameObject.SetActive(true);

        if(previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add(("itemIndex"), itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;

    }

    private void FixedUpdate()
    {
        if (!PV.IsMine)
            return;
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    [PunRPC]
    private void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        Debug.Log($"Took {damage} damage...");

        currentHealth -= damage;

        healthbarImage.fillAmount = currentHealth / maxHealth;

        if(currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    private void Die()
    {
        playerManager.Die();
    }

    private void GetInput()
    {
        horizontalRaw = Input.GetAxisRaw("Horizontal");
        verticalRaw = Input.GetAxisRaw("Vertical");
    }

    private void PlayerAnimation()
    {
        if (horizontalRaw != 0 || verticalRaw != 0)
        {
            animator.SetBool(moveAnimation, true);
            animator.SetFloat(moveAnimationSpeedMultiplier, (isSprinting ? (sprintSpeed * animationMultiplier) : (walkSpeed * animationMultiplier)));
        }
        else animator.SetBool(moveAnimation, false);
    }
}
