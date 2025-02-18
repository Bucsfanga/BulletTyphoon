using UnityEngine;

public class GunAmmoData 
{
    public int currentAmmo, maxAmmo, totalAmmo;

    public GunAmmoData(int cur, int max, int total)
    {
        currentAmmo = cur;
        maxAmmo = max;
        totalAmmo = total;
    }
}
