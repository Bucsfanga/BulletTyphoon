using System.Collections;
using UnityEngine;

public class Damage : MonoBehaviour
{
    enum damageType { moving, stationary, falling, flood }

    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount, damageInterval;
    [SerializeField] float speed;
    [SerializeField] float destroyTime;
    [SerializeField] float delayTime;

    [SerializeField] LayerMask ignoreLayer; // Layer(s) to ignore

    private bool hasDealtDamage = false; // Flag to prevent multiple applications of damage
    private bool isApplyingDamage = false;
    private float damageRadius;

    public void setDamageRadius(float radius)
    {
        damageRadius = radius;
    }

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

    private IEnumerator delay()
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
        if (other is CapsuleCollider && !IsIgnoredLayer(other.gameObject.layer))
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
        if (type == damageType.flood)
        {
            isApplyingDamage = false;
            StopAllCoroutines(); // Stop applying flood damage coroutine
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || hasDealtDamage || IsIgnoredLayer(other.gameObject.layer))
            return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg == null)
            return;

        if (type == damageType.flood)
        {
            if (!(other is CapsuleCollider))
                return;

            StartCoroutine(applyFloodDamage(dmg));
        }
        else if (type == damageType.moving)
        {
            dmg.takeDamage(damageAmount);
            hasDealtDamage = true;
            Destroy(gameObject);
        }
        else if (type == damageType.falling)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);

            foreach (Collider col in colliders)
            {
                if (col == other && dmg != null)
                {
                    dmg.takeDamage(damageAmount);
                    hasDealtDamage = true;
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }

    private bool IsIgnoredLayer(int layer)
    {
        // Check if the layer is in the ignoreLayer mask
        return (ignoreLayer.value & (1 << layer)) != 0;
    }
}
