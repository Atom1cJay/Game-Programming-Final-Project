using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Robat : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject deadPrefab;
    [SerializeField] private Vector3 deadPrefabSpawnPos;
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private int health = 7;
    private bool diving = false;

    [SerializeField] private float visionRange;
    [SerializeField] private float explodeRange;
    private float speed;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float divingSpeed;
    [SerializeField] private GameObject[] patrolPoints;
    private Vector3 nextDestination;
    private int currentDestinationIndex = 0;
    private float elapsedTime;
    [SerializeField] private float patrolStopTime;
    private float patrolStopTimer;

    [SerializeField] private Transform enemyEyes;
    [SerializeField] private float fieldOfView = 80;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        healthSlider.maxValue = health;
        healthSlider.value = health;
        FindNextDestination();
    }

    void Update()
    {
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

        if (diving)
        {
            DivingUpdate();
        }
        else
        {
            PatrolUpdate();
        }
    }

    private void FixedUpdate()
    {
        float idealSpeed;
        if (diving)
        {
            idealSpeed = divingSpeed;
        }
        else
        {
            idealSpeed = patrolSpeed;
        }
        speed = Mathf.Lerp(speed, idealSpeed, 0.01f);
        transform.position = Vector3.MoveTowards(transform.position, nextDestination, speed);
    }

    private void DivingUpdate()
    {
        nextDestination = player.transform.position;
        if (Vector3.Distance(player.transform.position, transform.position) <= explodeRange)
        {
            Die();
        }
        /*else if (distanceToPlayer > chaseDistance)
        {
            currentState = FSMStates.Patrol;
        }*/
        FaceTarget(nextDestination);
        enemyAttack();
    }

    private void PatrolUpdate()
    {
        Vector3 patrolPos = new Vector3(nextDestination.x, nextDestination.y, nextDestination.z);
        if (Vector3.Distance(transform.position, patrolPos) < 1.0f)
        {
            patrolStopTimer += Time.deltaTime;
            if (patrolStopTimer > patrolStopTime)
            {
                FindNextDestination();
                patrolStopTimer = 0;
            }
        }
        else if (SeesPlayer())
        {
            diving = true;
        }
        if (patrolStopTimer < patrolStopTime)
        {
            FaceTarget(nextDestination);
        }
    }

    void FindNextDestination()
    {
        nextDestination = patrolPoints[currentDestinationIndex].transform.position;
        currentDestinationIndex = (currentDestinationIndex + 1) % patrolPoints.Length;
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;
        //directionToTarget.y = transform.position.y;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10 * Time.deltaTime);
    }

    void enemyAttack()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= explodeRange)
        {
            Die();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Die();
    }

    public void takeDamage(int amount)
    {
        health = Mathf.Clamp(health - amount, 0, health);
        healthSlider.value = health;
        if (health <= 0)
        {
            Die();
        }
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
        RaycastHit hit;

        if (Vector3.Distance(transform.position, player.transform.position) <= visionRange)
        {
            if (Physics.Raycast(enemyEyes.position, (player.transform.position - enemyEyes.position), out hit, visionRange))
            {
                //Debug.Log("hit: " + hit.collider.gameObject.name);
                //Debug.DrawLine(enemyEyes.position, player.transform.position, Color.cyan);
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
