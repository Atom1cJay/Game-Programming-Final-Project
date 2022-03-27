using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeSomewhat : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float force = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(Random.Range(-1, 1), Random.Range(0, 1), Random.Range(-1, 1)) * force, ForceMode.Impulse);
    }
}
