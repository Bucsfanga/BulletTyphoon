using UnityEngine;
using UnityEngine.SceneManagement;

public class pickup : MonoBehaviour
{
    [SerializeField] gunStats gun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Guns"))
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
        else if (other.CompareTag("Classified"))
        {
            IPickup pick = other.GetComponent<IPickup>();

            if (pick != null)
            {
                //Createe classified file instance and copy the data
                string _levelName = SceneManager.GetActiveScene().name;
                Item_Classified newClassified = ScriptableObject.CreateInstance<Item_Classified>();
                newClassified.SetLevel(_levelName);
                newClassified.CopyTo(newClassified);

                // transfer item collected to player
                pick.collectClassified(newClassified);

                Destroy(gameObject);
            }

            
        }
    }
}
