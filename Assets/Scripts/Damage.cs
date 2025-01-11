using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Damage : MonoBehaviour
{
    enum damageType { moving, stationary, falling }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] float speed;
    [SerializeField] int destroyTime;
    [SerializeField] float delayTime;

    private bool hasDealtDamage = false; // Flag to prevent multiple applications of damage

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == damageType.falling)
        {
            StartCoroutine(delay());
        }
        else if (type == damageType.moving && rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private System.Collections.IEnumerator delay()
    {
        yield return new WaitForSeconds(delayTime);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.down * speed;
        }
        Destroy(gameObject, destroyTime);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || hasDealtDamage)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(damageAmount);
            hasDealtDamage = true;
        }

        if (type == damageType.moving)
        {
            Destroy(gameObject);
        }

        if (type == damageType.falling)
        {
            StartCoroutine(delayDestroy());
        }
    }

    private IEnumerator delayDestroy()
    {
        yield return new WaitForSeconds(0.05f);
        Destroy(gameObject);
    }
}
