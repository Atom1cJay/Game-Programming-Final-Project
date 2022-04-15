using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private Transform player;
    [SerializeField] private int damageMin;
    [SerializeField] private int damageMax;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        transform.LookAt(player);
        //transform.rotation = Quaternion.Euler(transform.rotation.x + 90, transform.rotation.y, transform.rotation.z - 45);
        //transform.Translate(Vector3.up * 50);
        //transform.Translate(Vector3.forward * 50);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(Random.Range(damageMin, damageMax));
        }
    }
}
