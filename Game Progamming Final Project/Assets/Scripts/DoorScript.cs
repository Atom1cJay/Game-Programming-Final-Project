using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (LevelManager.enemiesLeft <= 0 && other.CompareTag("Player"))
        {
            FindObjectOfType<LevelManager>().LevelBeaten();
        }
    }
}
