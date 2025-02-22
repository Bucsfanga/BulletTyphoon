using System.Collections;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public enum AbilityType
    {
        Speed,
        Jump,
        Damage,
        GodMode,
        Points
    }

    [SerializeField] float respawnTime;
    [SerializeField] GameObject model;
    [SerializeField] string buttonInfo;
    [SerializeField] AbilityType type;

    bool inTrigger;
    private playerController player;
    private Vector3 initialPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (inTrigger && player != null)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (player.hasAbility) //Placeholder for Getter/Setter if already have ability
                {
                    //Debug.Log("Already powered up!");
                    return;
                }
                else
                {
                    audioManager.instance.PlaySound("FifthArp");
                    model.SetActive(false);
                    player.hasAbility = true;
                    GameManager.instance.buttonInteract.SetActive(false);
                    GameManager.instance.buttonInfo.text = "";
                    //Debug.Log("Ability picked up");

                    Invoke("Respawn", respawnTime); // Respawn after delay
                    StartCoroutine(checkAbility(respawnTime, player));
                }
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
        GameManager.instance.playerScript.hasAbility = false;
    }

    IEnumerator checkAbility(float respawnTime, playerController player)
    {
        yield return new WaitForSeconds(respawnTime);
        audioManager.instance.PlaySound("MinThird");
        player.hasAbility = false; // Set _hasAbility to false after waiting
        //Debug.Log("Ability has expired!");
    }
}