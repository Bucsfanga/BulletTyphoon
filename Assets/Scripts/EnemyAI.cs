using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] bool isElite;
    [SerializeField] Color eliteTintColor;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos, headPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] int FOV;
    [SerializeField] int animSpeedTrans;
    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDist; // How far he roams

    float angleToPlayer;
    float stoppingDistOrig;

    bool isShooting;
    bool playerInRange;
    bool isRoaming;

    Color colorOrig;

    Vector3 playerDir;
    Vector3 startingPos;

    Coroutine co;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (model == null)
        {
            Debug.LogError($"Renderer (model) is not assigned on {gameObject.name}");
            return;
        }

        if (model.sharedMaterial == null)
        {
            Debug.LogError($"Material is missing on the Renderer of {gameObject.name}");
            return;
        }

        model.material = new Material(model.sharedMaterial); // Create new material for elite
        colorOrig = model.material.color; // Save original color

        if (isElite)
        {
            eliteTint();
        }
        
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeed = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.MoveTowards(animSpeed, agentSpeed, Time.deltaTime * animSpeedTrans));

        if (playerInRange && !canSeePlayer())
        {
            if (!isRoaming && agent.remainingDistance < 0.1f)
            {
                co = StartCoroutine(roam());
            }
        }
        else if (!playerInRange)
        {
            if (!isRoaming && agent.remainingDistance < 0.1f)
            {
                co = StartCoroutine(roam());
            }
        }
    }

    IEnumerator roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamPauseTime);

        agent.stoppingDistance = 0;

        Vector3 randomPos = Random.insideUnitSphere * roamDist;
        randomPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);

        isRoaming = false;
    }

    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        // Adjust FOV check to include some vertical lenience
        float verticalAngleToPlayer = Vector3.Angle(Vector3.ProjectOnPlane(playerDir, Vector3.right), transform.forward);

        Debug.DrawRay(headPos.position, playerDir); // Draw ray for visual test in editor

        if (angleToPlayer <= FOV && verticalAngleToPlayer <= FOV)
        {
            RaycastHit hit;
            if (Physics.Raycast(headPos.position, playerDir, out hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    agent.SetDestination(GameManager.instance.player.transform.position);

                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        faceTarget();
                    }

                    if (!isShooting)
                    {
                        StartCoroutine(shoot());
                    }

                    agent.stoppingDistance = stoppingDistOrig;
                    return true;
                }
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }
    public void takeDamage(int amount)
    {
        Debug.Log($"[Damage Taken] Object: {gameObject.name}, Damage: {amount}");
        HP -= amount;

        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(GameManager.instance.player.transform.position); // Go to player last know location
        }

        if (co != null)
        {
            StopCoroutine(co);
            co = null;
            isRoaming = false;
        }

        if (HP > 0)
        {
            StartCoroutine(flashRed());
        }
        else
        {
            GameManager.instance.updateGameGoal(-1); // Subtract 1 from game goals enemy counter

            // Stop any remaining coroutines
            StopAllCoroutines();

            // Disable components to avoid further errors
            if (agent != null)
            {
                agent.enabled = false;
            }

            Destroy(gameObject); // Destroy object
        }
    }

    IEnumerator flashRed()
    {
        if (!isElite)
        {
            model.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            model.material.color = colorOrig;
        } 
        else
        {
            model.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            eliteTint();
        }
        
    }

    IEnumerator shoot()
    {
        isShooting = true;
        
        // Aim for middle of player
        Vector3 playerMidsection = GameManager.instance.player.transform.position + Vector3.up * 1.0f;

        // Calculate direction toward adjusted player position
        Vector3 directionToPlayer = (playerMidsection - shootPos.position).normalized;

        // Instantiate bullet and set forward direction
        GameObject spawnedBullet = Instantiate(bullet, shootPos.position, Quaternion.LookRotation(directionToPlayer));

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void eliteTint()
    {
        model.material.color = colorOrig * eliteTintColor; // Tint over base color
        model.material.SetColor("_EmissionColor", eliteTintColor * 5.0f); // Add glow effect
        model.material.EnableKeyword("_EMISSION");
    }

}
