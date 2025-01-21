using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage, IPickup
{
    #region Variables
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Transform playerCamera;

    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] float crouchHeight;
    [SerializeField] float crouchMod;
    [SerializeField] int gravity;
    [SerializeField] float targetFOV;
    [SerializeField] private float zoomSpeed = 5f;

    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    cameraController camController;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;
    float baseSpeed;
    int gunListPos;

    bool _isSprinting;
    bool _isJumping;
    bool _isCrouching;
    #endregion

    #region GET/SET
    public bool isJumping
    {
        get => _isJumping;
        set
        {
            if (value)
            {
                _isJumping = value;
                jumpCount++;
                playerVel.y = jumpSpeed;
            }
            else
            {
                _isJumping = value;
                jumpCount = 0;
                playerVel = Vector3.zero;
            }
        }
    }

    public bool isSprinting
    {
        get => _isSprinting;
        set
        {
            _isSprinting = value;
            speed = value ? baseSpeed * sprintMod : baseSpeed;
        }
    }

    public bool isCrouching
    {
        get => _isCrouching;
        set
        {
            if (_isCrouching != value)
            {
                _isCrouching = value;
                crouchHeight = _isCrouching ? 0 : 1;
                speed = (isCrouching ? speed /= crouchMod : speed *= crouchMod);

                // Update camera position
                playerCamera.localPosition = new Vector3(
                    playerCamera.localPosition.x,
                    crouchHeight,
                    playerCamera.localPosition.z
                );
            }
        }
    }
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        baseSpeed = speed;
        updatePlayerUI();
        camController = playerCamera.GetComponent<cameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
       
        if(!GameManager.instance.isPaused)
        {
            movement();
            selectGun();
        }

        movement();
        sprint();

        //Updates FOV with lerp
        Camera playerCam = playerCamera.GetComponent<Camera>();
        playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
    }

    void movement()
    {
        // Movement checks
        if (controller.isGrounded)
        {
            isJumping = false;
            jumpCount = 0;

            // Ensures correct speed when landing
            isSprinting = Input.GetButton("Sprint");
        }

        jump();
        crouch();

        // Movement implementation
        moveDir = Input.GetAxis("Horizontal") * transform.right +
            Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        // Controls
        if (Input.GetButtonDown("Shoot"))
        {
            shoot();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            zoom(30f);
            camController.sens = (int)(camController.sens * 0.4);
        }
        if (Input.GetButtonUp("Fire2"))
        {
            zoom(60f);
            camController.sens = (int)(camController.sens / 0.4);
        }
    }

    void zoom(float speed)
    {
        targetFOV = speed;
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint") && !isJumping)
        {
            if (isCrouching)
            {
                isCrouching = false;
            }
            if (!isSprinting)
                isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint") && !isJumping)
        {
            isSprinting = false;
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            if (isCrouching)
            {
                isCrouching = false;
            }
            isJumping = true;
        }
    }

    void crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            if (isSprinting)
            {
                isSprinting = false;
            }
            if (isJumping)
            {
                return;
            }
            else
            {
                isCrouching = !isCrouching;
            }
        }
    }

    void shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {
            Debug.Log(hit.collider.name);
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamagePanel());

        if (HP <= 0)
        {
            GameManager.instance.youLose();
        }
    }
    public void IncreaseHealth(int amount)
    {
        if(HP >= HPOrig)
        {
            Debug.Log("Health is already full");
            return;
        }

        HP += amount;
        if (HP > HPOrig)
        {
            HP = HPOrig; // Don't heal past max health
        }

        updatePlayerUI();
        Debug.Log("Health increased. Current health: " + HP);
    }

    public bool isHealthFull()
    {
        return HP >= HPOrig;
    }

    IEnumerator flashDamagePanel()
    {
        GameManager.instance.damagePanel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.damagePanel.SetActive(false);
    }

    void updatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;

        changeGun();
    }

    void selectGun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
    }

    void changeGun()
    {
        // Change player gun stats
        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;

        // Change the model
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }
}
