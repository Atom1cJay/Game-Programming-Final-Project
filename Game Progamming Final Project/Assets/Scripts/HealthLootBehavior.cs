using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthLootBehavior : MonoBehaviour
{
    public int minHeal = 7;
    public int maxHeal = 12;
    //public GameObject healSound;
    

    void Update()
    {
        transform.Rotate(Vector3.up, 90 * Time.deltaTime);

        if (transform.position.y > 1.0f)
        {
            this.transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime, transform.position.z);
            //this.transform.Translate(Vector3.left * Time.deltaTime * 1f);
        }
        else
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            // Instantiate(healSound, transform.position, transform.rotation);
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.GiveHealth(Random.Range(minHeal, maxHeal));
            }

            gameObject.SetActive(false);
            Destroy(gameObject, 0.5f);
        }
    }
}
