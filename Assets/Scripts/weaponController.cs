// WeaponController.cs
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public gunStats currentGunStats; // Reference to the GunStats scriptable object
    private playerController playerCtrl;

    private bool isReloading = false;
    private float nextTimeToFire = 0f;

    // These variables are now private since they'll be set by gunStats
    private float fireRate;
    private float range;
    private int maxAmmo;
    private int currentAmmo;
    public float reloadTime = 2f; // Keeping this in WeaponController as it's more of a gameplay mechanic

    void Start()
    {
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        UpdateGunStats();
    }

    public void UpdateGunStats() { 
        if (currentGunStats != null)
        {
            // Initialize values from gunStats
            fireRate = currentGunStats.shootRate;
            range = currentGunStats.shootDist;
            maxAmmo = currentGunStats.ammoMax;
            currentAmmo = currentGunStats.ammoCur;
        }
        else
        {
            Debug.LogError("No GunStats assigned to WeaponController!");
        }

        // Update UI at start
        GameManager.instance.UpdateAmmo(currentAmmo, maxAmmo);
    }

    void Update()
    {
        // Handle reloading
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            Reload();
            return;
        }

        if (isReloading)
            return;

        // Handle shooting
        if (Input.GetButton("Shoot") && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        // Reduce ammo
        currentAmmo--;

        // Update UI
        GameManager.instance.UpdateAmmo(currentAmmo, maxAmmo);
    }

    void Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        // Wait for reload time
        Invoke("FinishReload", reloadTime);
    }

    void FinishReload()
    {
        currentAmmo = maxAmmo;
        isReloading = false;

        // Update UI
        GameManager.instance.UpdateAmmo(currentAmmo, maxAmmo);
        Debug.Log("Reload complete!");
    }

    // Method to change weapons
    public void EquipWeapon(gunStats newGunStats)
    {
        currentGunStats = newGunStats;

        // Update weapon values
        fireRate = newGunStats.shootRate;
        range = newGunStats.shootDist;
        maxAmmo = newGunStats.ammoMax;
        currentAmmo = newGunStats.ammoCur;

        // Update UI
        GameManager.instance.UpdateAmmo(currentAmmo, maxAmmo);
    }

    public void OnGunChanged(gunStats newGunStats)
    {
        currentGunStats = newGunStats;
        UpdateGunStats();
    }
}