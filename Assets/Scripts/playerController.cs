using System.Collections;
using System.Collections.Generic;
//using NUnit.Framework;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage, IPickup, iInteract
{
    [Header("-----Components-----")]
    #region Variables
    [SerializeField] CharacterController controller;
    //[SerializeField] AudioSource aud; // Lecture 6 - IAN NOTE: Commenting out to use the audioManager singleton
    [SerializeField] audioManager audioManager; //using the audioManager rather than accessing the AudioSource directly
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Transform playerCamera;

    [Header("-----Stats-----")]
    [Range(1, 10)] [SerializeField] int HP;
    [Range(1, 10)][SerializeField] float speed;
    [Range(1, 10)][SerializeField] int sprintMod;
    [Range(1, 10)][SerializeField] int jumpMax;
    [Range(1, 20)][SerializeField] int jumpSpeed;
    [SerializeField] float crouchHeight;
    [SerializeField] float crouchMod;
    [Range(1, 20)][SerializeField] int gravity;
    [SerializeField] float targetFOV;
    [SerializeField] private float zoomSpeed = 5f;

    [Header("-----Guns-----")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject muzzleFlash; // Lecture 6
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] int ammoCur;
    [SerializeField] int ammoMax;
    [SerializeField] float reloadTime;
    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootSoundVol;
    public gunStats currentGunStats;

    [Header("-----Audio-----")]
    
    [SerializeField] AudioClip[] audSteps;
    [SerializeField][Range(0, 1)] float audStepsVol;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField] [Range (0,1)] float audHurtVol;
    [SerializeField] AudioClip[] audJump;
    [SerializeField][Range(0, 1)] float audJumpVol;
    [SerializeField] AudioClip[] audReload;
    [SerializeField][Range(0, 1)] float audReloadVol;
    [SerializeField] gunshotAudio gunshotAudio;
    [SerializeField] gunReloadAudio gunReloadAudio;
    [SerializeField] gunClickAudio gunClickAudio;

    cameraController camController;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;
    float baseSpeed;
    int gunListPos;
    int currentAmmo, maxAmmo;
    float shootTimer; // Lecture 6

    private float originalFOV;
    private bool _isShooting;
    private bool _isReloading;
    private bool _isSprinting;
    private bool _isJumping;
    private bool _isCrouching;
    private bool _isPlayingSteps;
    #endregion

    #region GET/SET
    public bool isPlayingSteps
    {
        get => _isPlayingSteps;
        set
        {
            if (value)
            {
                //aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);IAN TODO: Commented out to use the audioManager singleton
            }
            _isPlayingSteps = value;
        }
    }

    public bool isHealthFull => HP >= HPOrig;

    public bool isShooting
    {
        get => _isShooting;
        set
        {
            if (value)
            {
                //aud.PlayOneShot(shootSound[Random.Range(0, shootSound.Length)], shootSoundVol);IAN TODO: Commented out to use the audioManager singleton
                //audioManager.instance.PlaySound();
                //gunshotAudio.PlayGunShot();
            }
            _isShooting = value;
        }
    }

    public bool isReloading
    {
        get => _isReloading;
        set
        {
            if (value)
            {
                _isReloading = value;
                Debug.Log("Reloading..."); // Debug log to ensure method is triggered.
                //aud.PlayOneShot(audReload[Random.Range(0, audReload.Length)], audReloadVol);IAN TODO: Commented out to use the audioManager singleton
                gunReloadAudio.PlayGunReload();


                // Wait for reload time
                Invoke("FinishReload", reloadTime);
            }
            else
            {
                _isReloading = value;
                currentAmmo = maxAmmo;

                GameManager.instance.UpdateAmmo(currentAmmo, maxAmmo); // Update UI
                Debug.Log("Reload complete!"); // Debug log to confirm.
            }
        }
    }

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
        if (gunList.Count > 0)
        {
            gunListPos = 0;
            changeGun();
        }
        else
        {
            Debug.LogError("Player has no starting gun assigned!");
        }

        audioManager = audioManager.instance; //set the audioManager instance to the instance in the scene

        originalFOV = targetFOV;
        HPOrig = HP;
        baseSpeed = speed;
        updatePlayerUI();
        camController = playerCamera.GetComponent<cameraController>();
        shootTimer = shootRate;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (!GameManager.instance.isPaused)
        {
            shootTimer += Time.deltaTime; // Increment timer every frame.
            movement();
            selectGun();
            reload();
        }

        sprint();

        //Updates FOV with lerp
        Camera playerCam = playerCamera.GetComponent<Camera>();
        playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
    }

    IEnumerator playSteps()
    {
        isPlayingSteps = true;

        if(!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);

        isPlayingSteps = false;
    }

    void movement()
    {
        // Movement checks
        if (controller.isGrounded)
        {
            if (moveDir.magnitude > 0.3f && !isPlayingSteps)
            {
                StartCoroutine(playSteps());
            }

            isJumping = false;
            jumpCount = 0;

            // Ensures correct speed when landing
            isSprinting = Input.GetButton("Sprint");
        }

        jump();
        crouch();
        reload();

        // Movement implementation
        moveDir = Input.GetAxis("Horizontal") * transform.right +
            Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        // Controls
        if (gunList.Count > 0 && gunListPos >= 0 && gunListPos < gunList.Count)
        {
            if (Input.GetButton("Shoot") && shootTimer >= shootRate && !isReloading)
            {
                if (currentAmmo == 0 && !isReloading)
                {
                    reload();
                }
                else
                {
                    shoot();
                }
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            zoom(targetFOV / 2);
            camController.sens = (int)(camController.sens * 0.4);
        }
        if (Input.GetButtonUp("Fire2"))
        {
            zoom(originalFOV);
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
            //aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol); // Lecture 6 IAN TODO: Commented out to use the audioManager singleton
            audioManager.PlayRandomJumpSound();
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
        if (shootTimer < shootRate) return; // Prevent shooting if timer is less than the fire rate.

        shootTimer = 0; // Reset the timer when shooting.

        // Play shooting sound
        gunshotAudio.PlayGunShot();
        //if (shootSound != null && shootSound.Length > 0)
        //{
        //    isShooting = true;
        //}

        // Visual effects
        StartCoroutine(flashMuzzleFire());

        // Check for hit
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {
            Instantiate(hitEffect, hit.point, Quaternion.identity);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }

        // Reduce ammo
        currentAmmo--;
        if( currentAmmo <= 0)
        {
            gunClickAudio.PlayGunClick();
        }
        GameManager.instance.UpdateAmmo(currentAmmo, maxAmmo); // Update UI
    }
        
    void reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            isReloading = true;
        }
    }

    void FinishReload()
    {
        isReloading = false;
    }

    IEnumerator flashMuzzleFire()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        //aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol); // Commented out to use the audioManager singleton
        if (HP > 0)
        {
            audioManager.PlayRandomDamageSound();
        }
        updatePlayerUI();
        StartCoroutine(flashDamagePanel());

        if (HP <= 0)
        {
            audioManager.PlayRandomDeathSound();
            GameManager.instance.youLose();
        }
    }
    public void IncreaseHealth(int amount)
    {
        if(isHealthFull)
        {
            Debug.Log("Health is already full");
            return;
        }

        HP += amount;
        if (isHealthFull)
        {
            HP = HPOrig; // Don't heal past max health
        }

        updatePlayerUI();
        Debug.Log("Health increased. Current health: " + HP);
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

        currentGunStats = gun; // Store current gun for reference
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
        maxAmmo = gunList[gunListPos].ammoMax;
        currentAmmo = gunList[gunListPos].ammoCur;
        hitEffect = gunList[gunListPos].hitEffect;
        shootSound = gunList[gunListPos].shootSound;
        shootSoundVol = gunList[gunListPos].shootSoundVol;

        GameManager.instance.UpdateAmmo(currentAmmo, maxAmmo);

        // Change the model
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }
}
