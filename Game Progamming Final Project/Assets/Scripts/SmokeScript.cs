using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(100000);
        }
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyAI>().takeDamage(100000);
        }
    }
}
