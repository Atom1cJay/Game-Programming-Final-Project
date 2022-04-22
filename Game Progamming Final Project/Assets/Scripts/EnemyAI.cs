using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum FSMStates
    {
        Patrol,
        Chase,
        Attack,
        Dying
    }

    public FSMStates currentState = FSMStates.Patrol;
    public int health = 10;
    public float chaseDistance = 12;
    public float chaseStoppingDistance = 3;
    public float attackDistance = 6;
    public float patrolSpeed = 5;
    public float chaseSpeed = 5;
    public float shootRate = 1;
    public int scoreValue = 1;
    public int healChance = 50;

    public GameObject healLoot;
    GameObject player;
    public GameObject deadPrefab;
    public Vector3 deadPrefabSpawnPos;
    public GameObject attackPrefab;
    public Transform firepoint;
    public Slider healthSlider;
    NavMeshAgent agent;

    public GameObject[] patrolPoints;
    Vector3 nextDestination;
    int currentDestinationIndex = 0;
    float distanceToPlayer;
    float elapsedTime;
    public float patrolStopTime;
    float patrolStopTimer;

    public Transform enemyEyes;
    public float fieldOfView = 80;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        healthSlider.maxValue = health;
        healthSlider.value = health;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

        if (patrolPoints.Length == 0)
        {
            NewPatrolPoints();
        }
        if (patrolPoints.Length > 0)
        {
            if (patrolPoints[0] == null)
            {
                NewPatrolPoints();
            }
        }
        FindNextDestination();
    }

    void Update()
    {
        /*if (Input.GetKey(KeyCode.L))
        {
            Die();
        }*/
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        switch (currentState)
        {
            case FSMStates.Patrol:
                UpdatePatrolState();
                break;
            case FSMStates.Chase:
                UpdateChaseState();
                break;
            case FSMStates.Attack:
                UpdateAttackState();
                break;
            case FSMStates.Dying:
                UpdateDyingState();
                break;
            default:
                break;
        }
        elapsedTime += Time.deltaTime;
        if (health == healthSlider.maxValue || health == 0)
        {
            healthSlider.transform.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            healthSlider.transform.localScale = new Vector3(5.4f, 5.4f, 5.4f);
            healthSlider.transform.LookAt(player.transform.position);
            healthSlider.transform.Rotate(Vector3.up * 180);
        }
    }

    void UpdatePatrolState()
    {
        agent.stoppingDistance = 0;
        Vector3 patrolPos = new Vector3(nextDestination.x, transform.position.y, nextDestination.z);
        if (Vector3.Distance(transform.position, patrolPos) < 1.0f)
        {
            patrolStopTimer += Time.deltaTime;
            if (patrolStopTimer > patrolStopTime)
            {
                FindNextDestination();
            }
        }
        else if (SeesPlayer())
        {
            currentState = FSMStates.Chase;
        }
        FaceTarget(nextDestination);
        //agent.speed = patrolSpeed; agent.speed is set in FindNextDestination
        agent.SetDestination(nextDestination);
    }

    void UpdateChaseState()
    {
        agent.stoppingDistance = chaseStoppingDistance;
        nextDestination = player.transform.position;
        if (distanceToPlayer <= attackDistance)
        {
            currentState = FSMStates.Attack;
        }
        else if (distanceToPlayer > chaseDistance)
        {
            currentState = FSMStates.Patrol;
        }
        FaceTarget(nextDestination);
        //agent.speed = chaseSpeed; agent.speed is set in FindNextDestination
        agent.SetDestination(nextDestination);
    }

    void UpdateAttackState()
    {
        nextDestination = player.transform.position;
        if (distanceToPlayer <= attackDistance)
        {
            currentState = FSMStates.Attack;
        }
        else if (distanceToPlayer > attackDistance && SeesPlayer())
        {
            currentState = FSMStates.Chase;
        }
        else if (distanceToPlayer > chaseDistance)
        {
            currentState = FSMStates.Patrol;
        }
        FaceTarget(nextDestination);
        enemyAttack();
    }

    void UpdateDyingState()
    {
        Instantiate(deadPrefab, transform.position + deadPrefabSpawnPos, transform.rotation);
        if (Random.Range(0, 100) <= healChance)
        {
            Instantiate(healLoot, transform.position, Quaternion.identity);
        }
        // healthSlider.transform.position = new Vector3(0, -100, 0);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().enemiesKilled += scoreValue;
        FindObjectOfType<LevelManager>().EnemyDestroyed();
        agent.enabled = false;
        Destroy(gameObject, 0);
    }

    void FindNextDestination()
    {
        nextDestination = patrolPoints[currentDestinationIndex].transform.position;
        if (currentState == FSMStates.Chase || currentState == FSMStates.Attack)
        {
            agent.speed = chaseSpeed;
        }
        else
        {
            agent.speed = patrolSpeed;
        }
        currentDestinationIndex = (currentDestinationIndex + 1) % patrolPoints.Length;
        agent.SetDestination(nextDestination);
    }

    void NewPatrolPoints()
    {
        Debug.Log("Enemy was not given patrol points. Finding nearest patrol points..");
        GameObject[] possiblePoints = GameObject.FindGameObjectsWithTag("PatrolPoint");
        List<GameObject> newPoints = new List<GameObject>();
        foreach (GameObject g in possiblePoints)
        {
            if (Vector3.Distance(transform.position, g.transform.position) <= 25)
            {
                newPoints.Add(g);
            }
        }
        patrolPoints = newPoints.ToArray();
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;
        directionToTarget.y = transform.position.y;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10 * Time.deltaTime);
    }

    void enemyAttack()
    {
        if (elapsedTime >= shootRate)
        {
            GameObject attackObject = Instantiate(attackPrefab, firepoint.position, firepoint.rotation);
            Physics.IgnoreCollision(GetComponent<Collider>(), attackObject.GetComponent<Collider>(), true);
            elapsedTime = 0;
        }
    }

    public void takeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            health = 0;
            currentState = FSMStates.Dying;
        }
        nextDestination = player.transform.position;
        agent.speed = chaseSpeed;
        healthSlider.value = health;
    }

    private bool SeesPlayer()
    {
        return PlayerWithinAngle() && PlayerUnobstructedInVision();
    }

    private bool PlayerWithinAngle()
    {
        float angleObjects = Vector3.Angle(player.transform.position - enemyEyes.position, enemyEyes.forward);
        return (angleObjects > -1 * fieldOfView && angleObjects <= fieldOfView);
    }

    private bool PlayerUnobstructedInVision()
    {
        /*Vector3 playerPos = GameObject.FindGameObjectWithTag("PlayerPosition").GetComponent<Transform>().position;
        RaycastHit hit;

        if (Vector3.Distance(transform.position, playerPos) <= chaseDistance)
        {
            if (Physics.Raycast(enemyEyes.position, (playerPos - enemyEyes.position), out hit, chaseDistance))
            {
                Debug.Log("hit: " + hit.collider.gameObject.name);
                Debug.DrawLine(enemyEyes.position, (playerPos - enemyEyes.position), Color.cyan);
                if (hit.transform.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;*/

        RaycastHit hit;

        if (Vector3.Distance(transform.position, player.transform.position) <= chaseDistance)
        {
            return true;
            if (Physics.Raycast(enemyEyes.position, (player.transform.position - enemyEyes.position), out hit, chaseDistance))
            {
                Debug.Log("hit: " + hit.collider.gameObject.name);
                Debug.DrawLine(enemyEyes.position, (player.transform.position - enemyEyes.position), Color.cyan);
                if (hit.transform.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Die()
    {
        FindObjectOfType<LevelManager>().EnemyDestroyed();
        Instantiate(deadPrefab, transform.position - new Vector3(0, 1.3f, 0), transform.rotation);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        /*Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);*/

        /*Vector3 frontRayPoint = enemyEyes.position + (enemyEyes.forward * chaseDistance);
        Vector3 leftRayPoint = Quaternion.Euler(0, fieldOfView / 2, 0) * frontRayPoint;
        Vector3 rightRayPoint = Quaternion.Euler(0, -fieldOfView / 2, 0) * frontRayPoint;
        Debug.DrawLine(enemyEyes.position, frontRayPoint, Color.cyan);
        Debug.DrawLine(enemyEyes.position, leftRayPoint, Color.cyan);
        Debug.DrawLine(enemyEyes.position, rightRayPoint, Color.cyan);*/
    }
}
