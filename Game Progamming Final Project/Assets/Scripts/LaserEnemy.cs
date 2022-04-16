using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LaserEnemy : MonoBehaviour
{
    private Transform player;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject deadPrefab;
    [SerializeField] private Transform firePoint; // Origin of laser
    [SerializeField] private float shootTime = 2; // Seconds between shots
    private float shootTimer = 0;
    [SerializeField] private float shootRange = 5;
    [SerializeField] private Transform rollingBall;

    private Rigidbody rb;
    private NavMeshAgent agent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Mathf.Abs(player.position.x - transform.position.x) > 1
            && Mathf.Abs(player.position.z - transform.position.z) > 1)
        {
            Vector3 lookPos = new Vector3(player.position.x,
            Mathf.Clamp(player.position.y, transform.position.y, transform.position.y + 1),
            player.position.z);
            transform.LookAt(lookPos);
        }
        //transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);

        shootTimer += Time.deltaTime;
        if (Vector3.Distance(transform.position, player.position) <= shootRange && shootTimer >= shootTime && !player.GetComponent<PlayerHealth>().IsDead())
        {
            firePoint.LookAt(player);
            //firePoint.rotation = Quaternion.Euler(firePoint.rotation.x + 90, firePoint.rotation.y, firePoint.rotation.z);
            Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
            shootTimer = 0;
        }

        if (rb.velocity.magnitude > 0)
        {
            rollingBall.Rotate(rb.velocity.magnitude * Time.deltaTime * 1 * Vector3.right);
        }

        if (Input.GetKey(KeyCode.L))
        {
            Die();
        }
    }

    void Die()
    {
        FindObjectOfType<LevelManager>().EnemyDestroyed();
        Instantiate(deadPrefab, transform.position - new Vector3(0, 1.3f, 0), transform.rotation);
        Destroy(gameObject);
    }
}
