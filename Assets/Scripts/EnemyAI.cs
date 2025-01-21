using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
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
        colorOrig = model.material.color;
        GameManager.instance.updateGameGoal(1);
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

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
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
        HP -= amount;

        if(agent != null && agent.isActiveAndEnabled)
        {
           agent.SetDestination(GameManager.instance.player.transform.position); // Go to player last know location
        }      

        if(co != null)
        {
            StopCoroutine(co);
            isRoaming = false;
        }

        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);
            agent.enabled = false;
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}
