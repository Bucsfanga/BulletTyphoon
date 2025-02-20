using UnityEngine;
using System.Collections.Generic;

public static class PersistentData
{
    //  Persistent Variables
    public static List<gunStats> savedGunList = new List<gunStats>();
    public static Dictionary<string, GunAmmoData> savedAmmoDic = new Dictionary<string, GunAmmoData>();
    public static int savedGunListPos;

    // Restart variables
    public static List<gunStats> levelStartGunList = new List<gunStats>();
    public static Dictionary<string, GunAmmoData> levelStartAmmoDic = new Dictionary<string, GunAmmoData>();
    public static int levelStartGunListPos;
}
