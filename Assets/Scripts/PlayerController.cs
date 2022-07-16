using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
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

    [SerializeField] private Canvas gameOverScreen;

    private readonly string moveAnimation = "isMoving";
    private readonly string deathAnimation = "isDead";
    private readonly string shootAutoAnimation = "isShootingAuto";
    private readonly string shootSemiAnimation = "isShootingSemi";
    private readonly string reloadAnimation = "isReloading";
    //private readonly string jumpAnimation = "isJumping";

    private readonly string moveMultiplier = "moveMultiplier";

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

    public bool isPickedUp = false;

    [HideInInspector] public bool isDead;

    [SerializeField] private TMP_Text currentItemText;
    [SerializeField] private TMP_Text currentItemAmmo;

    [SerializeField] private Canvas reloadingScreen;

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
        if (!PV.IsMine)
            return;

        if (isPickedUp)
            return;

        GetInput();

        UpdateHUDInfo();

        PlayerAnimation();

        Look();

        if (isDead)
            return;

        Move();
        Jump();

        ShowReloadingPrompt();

        if (itemIndex != 2 && SpecialityGun.grabbedRB != null)
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
                break;
            }
        }

        //Choose item by scrolling up
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if(itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }

        //Choose item by scrolling down
        else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if(itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
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
        if(rb != null)
        {
            rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        }
    }

    private void UpdateHUDInfo()
    {
        currentItemText.text = itemIndex switch
        {
            0 => "Rifle",
            1 => "Pistol",
            2 => "GraviGun",
            _ => "Weapon",
        };

        currentItemAmmo.text = itemIndex switch
        {
            0 => $"{items[itemIndex].GetComponent<SingleShotGun>().currentBullets}/{items[itemIndex].GetComponent<SingleShotGun>().maxBullets}",
            1 => $"{items[itemIndex].GetComponent<SingleShotGun>().currentBullets}/{items[itemIndex].GetComponent<SingleShotGun>().maxBullets}",
            2 => "   \u221E",
            _ => "99/99",
        };
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
            if (isDead == true)
                return;

            gameOverScreen.gameObject.SetActive(true);

            this.tag = "DeadPlayer";

            isDead = true;

            PlayerManager.Find(info.Sender).GetKill();

            Invoke(nameof(Die), 2.0f);
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

    private bool IsPlayerMoving()
    {
        if (horizontalRaw != 0 || verticalRaw != 0)
            return true;
        else
            return false;
    }

    private void PlayerAnimation()
    {
        //Movement
        if (IsPlayerMoving() == true)
        {
            animator.SetBool(moveAnimation, true);
            animator.SetFloat(moveMultiplier, (isSprinting ? (sprintSpeed * animationMultiplier) : (walkSpeed * animationMultiplier)));
        }
        else animator.SetBool(moveAnimation, false);

        //Shooting
        if(items[itemIndex].GetComponent<SingleShotGun>() != null)
        {
            //Auto
            if(itemIndex == 0)
            {
                if (Input.GetMouseButton(0) && items[itemIndex].GetComponent<SingleShotGun>().isReloading == false)
                {
                    animator.SetBool(shootAutoAnimation, true);
                }
                else animator.SetBool(shootAutoAnimation, false);
            }
            //Semi
            else if(itemIndex == 1)
            {
                if (Input.GetMouseButtonDown(0) && items[itemIndex].GetComponent<SingleShotGun>().isReloading == false)
                {
                    animator.SetBool(shootSemiAnimation, true);
                    Invoke(nameof(SetSemiAnimationBackToFalse), 0.1f);
                }
            }
        }

        //Reloading
        if (items[itemIndex].GetComponent<SingleShotGun>() != null)
        {
            if (items[itemIndex].GetComponent<SingleShotGun>().isReloading == true)
            {
                animator.SetBool(reloadAnimation, true);
            }
            else animator.SetBool(reloadAnimation, false);
        }

        //Jumping
        //if (Input.GetKeyDown(KeyCode.Space) && grounded)
        //{
        //    animator.SetBool(jumpAnimation, true);
        //}
        //else animator.SetBool(jumpAnimation, false);

        //Death
        if (isDead == true)
        {
            animator.SetBool(deathAnimation, true);
        }
        else animator.SetBool(deathAnimation, false);
    }

    private void SetSemiAnimationBackToFalse()
    {
        animator.SetBool(shootSemiAnimation, false);
    }

    private void ShowReloadingPrompt()
    {
        if (items[itemIndex].GetComponent<SingleShotGun>() != null && items[itemIndex].GetComponent<SingleShotGun>().isReloading == true)
        {
           reloadingScreen.gameObject.SetActive(true);
        }
        else reloadingScreen.gameObject.SetActive(false);
    }

}
