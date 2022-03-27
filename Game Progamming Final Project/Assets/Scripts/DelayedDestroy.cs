using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDestroy : MonoBehaviour
{
    public float delay = 2;
    void Start()
    {
        Destroy(this.gameObject, delay);
    }
}
