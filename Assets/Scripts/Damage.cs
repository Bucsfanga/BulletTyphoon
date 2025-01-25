using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Damage : MonoBehaviour
{
    enum damageType { moving, stationary, falling, flood }

    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount, damageInterval;
    [SerializeField] float speed;
    [SerializeField] int destroyTime;
    [SerializeField] float delayTime;

    private bool hasDealtDamage = false; // Flag to prevent multiple applications of damage
    private bool isApplyingDamage = false;
    private float damageRadius;

    public void setDamageRadius(float radius)
    {
        damageRadius = radius;
    }

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

    private void OnTriggerStay(Collider other)
    {
        if (type != damageType.flood || isApplyingDamage) return;

        // Check if the collider is a capsule collider
        if (other is CapsuleCollider)
        {
            IDamage dmg = other.GetComponent<IDamage>();

            if (dmg != null)
            {
                StartCoroutine(applyFloodDamage(dmg));
            }
        }
    }

    private IEnumerator applyFloodDamage(IDamage dmg)
    {
        isApplyingDamage = true;

        while (dmg != null && ((MonoBehaviour)dmg) != null) // Ensure object is not destroyed
        {
            dmg.takeDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval); // Damage interval
        }

        isApplyingDamage = false; // Reset flag when coroutine stops
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop applying damage when the object exits the flood
        if (type == damageType.flood)
        {
            isApplyingDamage = false;
            StopAllCoroutines(); // Stop applying flood damage coroutine
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || hasDealtDamage)
            return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg == null)
            return;

        if (type == damageType.flood)
        {
            if (!(other is CapsuleCollider))
                return;


            if (dmg != null)
            {
                // Start damage coroutine
                StartCoroutine(applyFloodDamage(dmg));
            }
        }
        else if (type == damageType.moving)
        {
            if (dmg != null)
            {
                dmg.takeDamage(damageAmount);
                hasDealtDamage = true;
                Destroy(gameObject);
            }
        }
        else if (type == damageType.falling)
        {
            // Check if the player/enemy is inside damage area
            Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius); // Adjust radius to match damage area

            foreach (Collider col in colliders)
            {
                if (col == other)
                {
                    if (dmg != null)
                    {
                        dmg.takeDamage(damageAmount);
                        hasDealtDamage = true;

                        Destroy(gameObject);
                        return;
                    }
                }
            }
        }
    }
}
