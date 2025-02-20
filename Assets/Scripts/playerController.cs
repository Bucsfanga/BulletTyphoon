using System.Collections;
using System.Collections.Generic;
using UnityEditor;

//using NUnit.Framework;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage, IPickup, iInteract
{
    [Header("-----Components-----")]
    #region Variables
    [SerializeField] CharacterController controller;
    //[SerializeField] AudioSource aud; // Lecture 6 - IAN NOTE: Commenting out to use the audioManager singleton
    [SerializeField] audioManager audioManager; //using the audioManager rather than accessing the AudioSource directly
    [SerializeField] muzzleFlashParticleEffect muzzleFlashParticleEffect;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Transform playerCamera;

    [Header("-----Stats-----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(1, 10)][SerializeField] float speed;
    [Range(1, 10)][SerializeField] int sprintMod;
    [Range(1, 10)][SerializeField] int jumpMax;
    [Range(1, 20)][SerializeField] int jumpSpeed;
    //[SerializeField] float crouchHeight; Richard - commented this out as it has become redundant.
    [SerializeField] float crouchMod;
    [SerializeField] float crouchHeightOffset = 0.5f;
    private float standingHeight;// Richard - these 2 store the original camera height and movement speed for the crouch
    [Range(1, 20)][SerializeField] int gravity;
    [SerializeField] float targetFOV;
    [SerializeField] private float zoomSpeed = 5f;

    [Header("-----Guns-----")]
    [SerializeField] public List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] int ammoCur;
    [SerializeField] int ammoMax;
    [SerializeField] int totalAmmo;
    [SerializeField] float reloadTime;
    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootSoundVol;
    public gunStats currentGunStats;

    [Header("-----Audio-----")]

    [SerializeField][Range(0, 1)] float audStepsVol;
    [SerializeField][Range(0, 1)] float audHurtVol;
    [SerializeField][Range(0, 1)] float audJumpVol;
    [SerializeField][Range(0, 1)] float audReloadVol;
    [SerializeField] gunshotAudio gunshotAudio;
    [SerializeField] gunReloadAudio gunReloadAudio;
    [SerializeField] gunClickAudio gunClickAudio;

    [SerializeField] List<Item_Classified> classifiedList = new List<Item_Classified>();

    cameraController camController;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;
    float baseSpeed;
    public int gunListPos;
    public int currentAmmo, maxAmmo, ammoTotal;
    float shootTimer; // Lecture 6

    // Ammo Dictionary
    public Dictionary<string, GunAmmoData> GunAmmoDic = new Dictionary<string, GunAmmoData>();

    private int totalDamageTaken = 0; // Track total damage taken
    private int totalStepsTaken = 0; // Track total steps
    private float originalFOV;
    private bool _isShooting;
    private bool _isReloading;
    private bool _isSprinting;
    private bool _isJumping;
    private bool _isCrouching;
    private bool _isPlayingSteps;
    private bool _hasAbility;
    #endregion

    #region GET/SET
    public bool hasAbility
    {
        get => _hasAbility;
        set
        {
            _hasAbility = value;
        }
    }

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

                // Calculate the target height
                float targetHeight = _isCrouching ? standingHeight - crouchHeightOffset : standingHeight;

                // Update camera position
                playerCamera.localPosition = new Vector3(
                    playerCamera.localPosition.x,
                    targetHeight,
                    playerCamera.localPosition.z
                );

                // Update movement speed
                speed = _isCrouching ? baseSpeed / crouchMod : baseSpeed;
            }
        }
    }
    #endregion

    void Awake()
    {
        //DontDestroyOnLoad(this.gameObject); // Don't destroy player on scene change

        //// Prevent player being created multiple times when reloading
        //if (FindObjectsByType<playerController>(FindObjectsSortMode.None).Length > 1)
        //{
        //    Destroy(gameObject);
        //}
    }

    void Start()
    {
        // Load player data when changing to a new scene
        GameManager.instance.loadPlayerData(this);

        if (gunList.Count == 0)
        {
            gunStats defaultGun = Resources.Load<gunStats>("Guns/GunDefault");

            if (defaultGun != null)
            {
                gunList.Add(defaultGun);
            }
            else
            {
                Debug.LogError("Cannot load default gun. Verify location in Resources folder");
            }
        }

        // Initialize gunIDs for any existing guns in the list
        foreach (var gun in gunList)
        {
            if (string.IsNullOrEmpty(gun.gunID))
            {
                gun.gunID = System.Guid.NewGuid().ToString();
            }
        }

        gunListPos = 0;
        changeGun();

        audioManager = audioManager.instance; //set the audioManager instance to the instance in the scene

        originalFOV = targetFOV;
        HPOrig = HP;
        baseSpeed = speed;
        updatePlayerUI();
        camController = playerCamera.GetComponent<cameraController>();
        shootTimer = shootRate;
        standingHeight = playerCamera.localPosition.y;  // This stores the original camera height
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
        if (isPlayingSteps) yield break; // Prevent overlapping sounds
        isPlayingSteps = true;

        audioManager.PlayRandomFootstepSound();

        if (!isSprinting)
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
       
        Vector3 previousPosition = transform.position; // Store previous position
       
        // Movement implementation
        moveDir = Input.GetAxis("Horizontal") * transform.right +
            Input.GetAxis("Vertical") * transform.forward;

        controller.Move(moveDir * speed * Time.deltaTime);
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        // Track movement distance and count steps
        float distanceMoved = Vector3.Distance(previousPosition, transform.position);
        if (distanceMoved > 0.1f)  // Avoid counting tiny movements
        {
            totalStepsTaken += Mathf.FloorToInt(distanceMoved * 10); // Scale step count
        }

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
            camController.sens = (float)(camController.sens * 0.4);
        }
        if (Input.GetButtonUp("Fire2"))
        {
            zoom(originalFOV);
            camController.sens = (float)(camController.sens / 0.4);
        }

    }
    //Function to return total steps taken
    public int GetStepsTaken()
    {
        return totalStepsTaken;
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
        gunshotAudio.PlayGunShot(); // Play shooting sound

        // Visual effects
        GameManager.instance.ShootAnim();

        if (muzzleFlashParticleEffect != null)
        {
            muzzleFlashParticleEffect.PlayMuzzleFlash();
        }

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

        currentAmmo--; // Reduce ammo

        string gunID = currentGunStats.gunID;

        if (GunAmmoDic.ContainsKey(gunID))
        {
            GunAmmoDic[gunID].currentAmmo = currentAmmo;
        }

        if (currentAmmo <= 0)
        {
            gunClickAudio.PlayGunClick();
        }
        GameManager.instance.UpdateAmmo(currentAmmo, maxAmmo); // Update UI
        GameManager.instance.updateAmmoCounter(currentAmmo, maxAmmo, totalAmmo);
    }

    void reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            if (totalAmmo <= 0) // Check for reserve ammo
            {
                Debug.Log("Out of ammo!");
                return;
            }

            isReloading = true;

            int ammoNeeded = maxAmmo - currentAmmo; // How many bullets needed to fill clip
            int ammoToReload = Mathf.Min(ammoNeeded, totalAmmo); // Only reload from reserve ammo

            currentAmmo += ammoToReload; // Increase current ammo
            totalAmmo -= ammoToReload; // Decrease reserve ammo

            string gunID = currentGunStats.gunID;
            if (GunAmmoDic.ContainsKey(gunID)) // Store updated ammo values
            {
                GunAmmoDic[gunID].currentAmmo = currentAmmo;
                GunAmmoDic[gunID].totalAmmo = totalAmmo;
            }

            GameManager.instance.UpdateAmmo(currentAmmo, maxAmmo); // Update UI
            GameManager.instance.updateAmmoCounter(currentAmmo, maxAmmo, totalAmmo);
            isReloading = false; // Reset flag
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        totalDamageTaken += amount;  // Track total damage
        if (HP > 0)
        {
            audioManager.PlayRandomDamageSound();
        }
        updatePlayerUI();
        StartCoroutine(flashDamagePanel());

        if (HP <= 0)
        {
            audioManager.PlayRandomDeathSound();
            GameManager.instance.youLose("you ran out of health!");
        }
    }
    // Function to return total damage taken
    public int GetDamageTaken()
    {
        return totalDamageTaken;
    }
    public void IncreaseHealth(int amount)
    {
        if (isHealthFull)
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
        // Search for existing gun of the same type
        foreach (var existingGun in gunList)
        {
            if (existingGun.name == gun.name && existingGun.model == gun.model) // Same gun type found
            {
                string existingGunID = existingGun.gunID; // Keep same gunID

                if (GunAmmoDic.ContainsKey(existingGunID))
                {
                    GunAmmoDic[existingGunID].totalAmmo += gun.ammoCur + gun.ammoTotal; // Add current and total ammo to current guns total
                }
                else // If gun doesn't have a record stored
                {
                    GunAmmoDic[existingGunID] = new GunAmmoData(existingGun.ammoCur, existingGun.ammoMax, existingGun.ammoTotal + gun.ammoCur + gun.ammoTotal);
                }

                // Get updated values
                GunAmmoData updatedAmmo = GunAmmoDic[existingGunID];
                currentAmmo = updatedAmmo.currentAmmo;
                maxAmmo = updatedAmmo.maxAmmo;
                totalAmmo = updatedAmmo.totalAmmo;
                GameManager.instance.updateAmmoCounter(currentAmmo, maxAmmo, totalAmmo); // Update UI
                return;
            }
        }

        // For first time gun pick up
        gun.gunID = System.Guid.NewGuid().ToString(); // Generate unique ID
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
        string gunID = gun.gunID;

        // Check if ammo values exist
        if (!GunAmmoDic.ContainsKey(gunID))
        {
            GunAmmoDic[gunID] = new GunAmmoData(gun.ammoCur, gun.ammoMax, gun.ammoTotal);
        }

        // Get updated values
        GunAmmoData newGunAmmo = GunAmmoDic[gunID];
        currentAmmo = newGunAmmo.currentAmmo;
        maxAmmo = newGunAmmo.maxAmmo;
        totalAmmo = newGunAmmo.totalAmmo;
        GameManager.instance.updateAmmoCounter(currentAmmo, maxAmmo, totalAmmo); // Update UI

        currentGunStats = gun; // Store current gun for reference
        changeGun();
    }

    public void collectClassified(Item_Classified _classified)
    {
        classifiedList.Add(_classified);
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
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

    public void changeGun()
    {
        if (gunList.Count == 0)
        {
            return;
        }
        if (gunListPos < 0 || gunListPos >= gunList.Count)
        {
            Debug.LogWarning("Invalid gunListPos, resetting to 0.");
            gunListPos = 0;
        }

        currentGunStats = gunList[gunListPos];
        string gunID = currentGunStats.gunID;

        // Change player gun stats
        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;

        if (GunAmmoDic.ContainsKey(gunID))
        {
            GunAmmoData ammoData = GunAmmoDic[gunID];
            currentAmmo = ammoData.currentAmmo;
            maxAmmo = ammoData.maxAmmo;
            totalAmmo = ammoData.totalAmmo;
        }
        else
        {
            currentAmmo = currentGunStats.ammoCur; // Default when no value saved
            maxAmmo = currentGunStats.ammoMax;
            totalAmmo = currentGunStats.ammoTotal;
            GunAmmoDic[gunID] = new GunAmmoData(currentAmmo, maxAmmo, totalAmmo);
        }

        hitEffect = gunList[gunListPos].hitEffect;
        shootSound = gunList[gunListPos].shootSound;
        shootSoundVol = gunList[gunListPos].shootSoundVol;

        GameManager.instance.UpdateAmmo(currentAmmo, maxAmmo);
        GameManager.instance.updateAmmoCounter(currentAmmo, maxAmmo, totalAmmo);

        // Change the model
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    // Save data for player persistence
    public void savePlayerInventory()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.savePlayerData(this);
        }
    }
}
