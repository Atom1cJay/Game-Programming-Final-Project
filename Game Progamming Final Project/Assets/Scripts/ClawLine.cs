using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawLine : MonoBehaviour
{

    LineRenderer line;
    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponentInChildren<LineRenderer>();
        line.SetPosition(0, transform.parent.position);
        line.SetPosition(1, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, transform.parent.position);
        line.SetPosition(1, Vector3.zero);
    }
}
