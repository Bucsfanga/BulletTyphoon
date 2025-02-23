using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;


public class MudEnemy : MonoBehaviour
{
    private bool canattack;
    private bool ishitting;
    float attackcooldown = 3f;
    [SerializeField] int melee;
    [SerializeField] int dmgdistance;
    IEnumerator Attack(playerController player)
    {
        canattack = true;
        if (Random.value <= 0.15f)
        {
            StartCoroutine(slow(player));
        }
        else
        {
            StartCoroutine(MeleeDamage());
        }
        yield return new WaitForSeconds(attackcooldown);
        canattack = false;
    }
    IEnumerator MeleeDamage()
    {
        ishitting = true;
        Vector3 playerMidsection = GameManager.instance.player.transform.position + Vector3.up * 1.0f;
        yield return new WaitForSeconds(attackcooldown);
        ishitting = false;
    }
    IEnumerator slow(playerController player)
    {
        float originalSpeed = player.speed;
        player.speed = originalSpeed * 0.75f;
        yield return new WaitForSeconds(2f);
        player.speed = originalSpeed;
    }

}
