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
    [SerializeField] float buffTime;
    [SerializeField] float buffAmount;
    [SerializeField] GameObject model;
    [SerializeField] string buttonInfo;
    [SerializeField] AbilityType type;

    bool inTrigger;
    private playerController player;
    private Vector3 initialPosition;
    private float originalDmgBuff;

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
                if (player.hasAbility)
                {
                    Debug.Log("Already powered up!");
                    return;
                }
                else
                {
                    audioManager.instance.PlaySound("FifthArp");
                    model.SetActive(false);
                    player.hasAbility = true;
                    GameManager.instance.buttonInteract.SetActive(false);
                    GameManager.instance.buttonInfo.text = "";
                    Debug.Log("Ability picked up");

                    ApplyAbility(type);
                    Debug.Log("Ability applied");

                    Invoke("Respawn", respawnTime); // Respawn after delay
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

    void ApplyAbility(AbilityType type)
    {
        switch(type)
        {
            case (AbilityType.Speed):
                StartCoroutine(SpeedBuff(buffTime, player));
                return;
            case (AbilityType.GodMode):
                StartCoroutine(GodmodeBuff(buffTime, player));
                return;
            case (AbilityType.Jump):
                StartCoroutine(JumpBuff(buffTime, player));
                return;
            case (AbilityType.Damage):
                StartCoroutine(DamageBuff(buffTime, player));
                return;
        }
    }

    IEnumerator GodmodeBuff(float bufftime, playerController player)
    {
        player.hasAbility = true;
        player.type = AbilityType.GodMode;
        yield return new WaitForSeconds(bufftime);
        player.type = new AbilityType();
        player.hasAbility = false;
    }

    IEnumerator SpeedBuff(float bufftime, playerController player)
    {
        player.speedBuff = buffAmount;
        player.hasAbility = true;
        yield return new WaitForSeconds(bufftime);
        player.hasAbility = false;
        player.speedBuff = 0f;
    }

    IEnumerator JumpBuff(float buffTime, playerController player)
    {
        player.jumpBuff += (int)buffAmount;
        player.hasAbility = true;
        yield return new WaitForSeconds(buffTime);
        player.hasAbility = false;
        player.jumpBuff -= (int)buffAmount;
    }

    IEnumerator DamageBuff(float buffTime, playerController player)
    {
        originalDmgBuff = player.damageMultiplier;
        player.damageMultiplier = buffAmount;
        player.hasAbility = true;
        yield return new WaitForSeconds(buffTime);
        player.hasAbility = false;
        player.damageMultiplier = originalDmgBuff;
        audioManager.instance.PlaySound("MinThird");
    }
}