using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Transform player;
    Rigidbody rb;

    public float seekingRange = 4;
    public float seekingAngle = 30;
    public float startSpeed = 0.2f;
    public float maxSpeed = 5;
    public float accel;
    float speed;

    public float maxLifeTime;
    private float lifetime;

    public GameObject explosion;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.LookAt(player);
        rb = GetComponent<Rigidbody>();
        speed = startSpeed;
        Invoke("Explode", maxLifeTime);
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, player.position) <= seekingRange)
        {
            Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f * Time.deltaTime);
        }
        if (rb.velocity.magnitude < maxSpeed)
        {
            speed = Mathf.Clamp(speed + accel, 0, maxSpeed);
        }
        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    bool SeesPlayer()
    {
        float angleObjects = Vector3.Angle(player.transform.position - transform.position, transform.forward);
        return (angleObjects > -1 * seekingAngle && angleObjects <= seekingAngle);
    }

    public void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
