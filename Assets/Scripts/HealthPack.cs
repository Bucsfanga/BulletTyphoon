using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] int healthIncrease;
    [SerializeField] float respawnTime;
    [SerializeField] GameObject model;
    [SerializeField] string buttonInfo;

    bool inTrigger;
    private playerController player;
    private Vector3 initialPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store initial position of health pack
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (inTrigger && player != null)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (player.isHealthFull)
                {
                    //Debug.Log("Health is already full!");
                    return;
                }

                audioManager.instance.PlaySound("FifthOctArpH");
                player.IncreaseHealth(healthIncrease); // Heal player on interact

                // Hide health pack and interact screen
                model.SetActive(false);
                GameManager.instance.buttonInteract.SetActive(false);
                GameManager.instance.buttonInfo.text = "";

                gameObject.SetActive(false);
                Invoke("Respawn", respawnTime); // Respawn after delay
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        iInteract open = other.GetComponent<iInteract>();
        playerController foundplayer = other.GetComponent<playerController>();

        if (open != null && foundplayer != null)
        {
            inTrigger = true;
            player = foundplayer;


            GameManager.instance.buttonInteract.SetActive(true);
            GameManager.instance.buttonInfo.text = buttonInfo;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<playerController>() != null)
        {
            inTrigger = false;
            player = null;
            GameManager.instance.buttonInteract.SetActive(false);
            GameManager.instance.buttonInfo.text = null;
        }
    }

    void Respawn()
    {
        transform.position = initialPosition;
        model.SetActive(true); // Re-activate model
        gameObject.SetActive(true);
    }

}
