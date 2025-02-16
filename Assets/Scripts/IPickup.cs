using Unity.Collections;
using UnityEngine;

public interface IPickup
{
    public void getGunStats(gunStats gun);

    public void collectClassified(Item_Classified _classified);
}
