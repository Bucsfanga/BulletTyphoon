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
            // Transfer gun to object that enters trigger
            pick.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
