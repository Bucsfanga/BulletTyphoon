using UnityEngine;
using UnityEngine.SceneManagement;

public class pickup : MonoBehaviour
{
    [SerializeField] gunStats gun;
    [SerializeField] Item_Classified classified;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        IPickup pick = other.GetComponent<IPickup>();

        if (pick != null)
        {
            if (gun != null)
            {
                // Create a new gunStats instance and copy the gun data
                gunStats newGun = ScriptableObject.CreateInstance<gunStats>();
                gun.CopyTo(newGun);

                newGun.gunID = System.Guid.NewGuid().ToString(); // Assign unique ID to each gun instance
                pick.getGunStats(newGun); // Transfer the new gun to the player
                Destroy(gameObject);
            }
            else if (classified != null)
            {

                //Create classified file instance and copy the data
                string _levelName = SceneManager.GetActiveScene().name;
                Item_Classified newClassified = ScriptableObject.CreateInstance<Item_Classified>();
                newClassified.SetLevel(_levelName);
                classified.CopyTo(newClassified);

                // transfer item collected to player
                pick.collectClassified(newClassified);

                Destroy(gameObject);
            }
        }
    }
}
