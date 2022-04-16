using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public int damage;
    float timer;

    void Start()
    {
        Destroy(gameObject, 2);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.2f)
        {
            GetComponent<SphereCollider>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyAI>() != false)
        {
            other.GetComponent<EnemyAI>().takeDamage(damage);
        }
        if (other.GetComponent<PlayerHealth>() != false)
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
