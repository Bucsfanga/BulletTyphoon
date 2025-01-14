using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] int healthIncrease;
    [SerializeField] float respawnTime;

    private Vector3 initialPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store initial position of health pack
        initialPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player detected");
            playerController player = other.GetComponent<playerController>();
            if (player != null)
            {
                if (player.isHealthFull())
                {
                    return;
                }

                audioManager.instance.PlaySound("HealthPackPickup");
                player.IncreaseHealth(healthIncrease);

                gameObject.SetActive(false);
                Invoke("Respawn", respawnTime);
            }
        }
    }


    void Respawn()
    {
        transform.position = initialPosition;
        gameObject.SetActive(true);
    }

}
