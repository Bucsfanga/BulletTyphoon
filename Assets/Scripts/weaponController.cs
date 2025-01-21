using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float fireRate = 0.1f;
    public float range = 100f;
    public int maxAmmo = 20;
    public int currentAmmo;
    public float reloadTime = 2f;

    private bool isReloading = false;
    private float nextTimeToFire = 0f;

    void Start()
    {
        // Initialize ammo
        currentAmmo = maxAmmo;

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
}