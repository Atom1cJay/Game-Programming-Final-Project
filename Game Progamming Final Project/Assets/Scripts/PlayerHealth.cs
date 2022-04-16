using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int maxHealth = 100;
    public Slider healthSlider;
    public int currentHealth = 100;
    public bool dead = false;
    
    public AudioClip deathSFX;
    
    public Image fadeImage;
    public Image hurt;

    public int enemiesKilled;
    
    public Camera cam;

    void Start()
    {
        enemiesKilled = 0;
        currentHealth = startingHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = startingHealth;
    }

    private void Update()
    {
        //Debug.Log("hp: " + currentHealth);
        if (dead)
        {
            fadeImage.color = new Color(0, 0, 0, fadeImage.color.a + Time.deltaTime * 0.5f);
            // died.GetComponent<TextMesh>().color = new Color(1, 0, 0, died.GetComponent<TextMesh>().color.a + Time.deltaTime * 0.4f);
            cam.transform.Translate(transform.rotation.normalized * Vector3.down * Time.deltaTime * 0.1f);
            hurt.color = new Color(1, 0, 0, 0);
        }
        else if (hurt.color.a > 0)
        {
            hurt.color = new Color(1, 0, 0, hurt.color.a - Time.deltaTime);
        }
        /*score.text = "score: " + enemiesKilled;
        if (enemiesKilled >= LevelManager.scoreNeeded && LevelManager.scoreNeeded != 0 && !winText.enabled)
        {
            YouWin();
        }*/
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        healthSlider.value = currentHealth;
        if (currentHealth <= 0)
        {
            if (!dead)
            {
                PlayerDies();
            }
        }
        else
        {
            hurt.color = new Color(1, 0, 0, 0.25f);
        }
    }

    public void GiveHealth(int amount)
    {
        if (!dead)
        {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            healthSlider.value = currentHealth;
            if (currentHealth <= 0)
            {
                PlayerDies();
            }
        }
    }

    void PlayerDies()
    {
        currentHealth = 0;
        if (fadeImage.color.a == 0)
        {
            dead = true;
            AudioSource.PlayClipAtPoint(deathSFX, transform.position);
            cam.GetComponent<CameraControl>().enabled = false;
            cam.GetComponent<ShootClaw>().enabled = false;
            gameObject.GetComponent<PlayerController>().enabled = false;
            GameObject.FindObjectOfType<LevelManager>().LevelLost();
            // gameObject.GetComponent<CharacterController>().enabled = false;
            //Invoke("Reload", 4.2f);
        }
    }

    /*void Reload()
    {
        GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().ReloadLevel();
    }

    void YouWin()
    {
        //GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().LoadNextLevel();
        GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().LoadNextLevelDelayed();
        winText.enabled = true;
        //Destroy(this);
    }*/

    public bool IsDead()
    {
        return dead;
    }
}
