using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : MonoBehaviour
{
    private Transform player;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject deadPrefab;
    [SerializeField] private Transform firePoint; // Origin of laser
    [SerializeField] private float shootTime = 2; // Seconds between shots
    private float shootTimer = 0;
    [SerializeField] private float shootRange = 3; // Length of laser prefab
    [SerializeField] private Transform rollingBall;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        transform.LookAt(player);
        //transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);

        shootTimer += Time.deltaTime;
        if (Vector3.Distance(transform.position, player.position) <= shootRange && shootTimer >= shootTime)
        {
            firePoint.LookAt(player);
            //firePoint.rotation = Quaternion.Euler(firePoint.rotation.x + 90, firePoint.rotation.y, firePoint.rotation.z);
            Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
            shootTimer = 0;
        }

        //rollingBall.rotation;

        /*if (Input.GetKey(KeyCode.LeftShift))
        {
            Die();
        }*/
    }

    void Die()
    {
        Instantiate(deadPrefab, transform.position - new Vector3(0, 1.3f, 0), transform.rotation);
        Destroy(gameObject);
    }
}
