using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] gunStats gun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickup pick = other.GetComponent<IPickup>();

        if (pick != null)
        {
            // Create a new gunStats instance and copy the gun data
            gunStats newGun = ScriptableObject.CreateInstance<gunStats>();
            gun.CopyTo(newGun);

            // Transfer the new gun to the player
            pick.getGunStats(newGun);

            Destroy(gameObject);
        }
    }
}
