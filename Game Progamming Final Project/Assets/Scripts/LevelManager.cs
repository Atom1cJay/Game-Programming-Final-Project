using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public int levelDuration = 0;
    float countdown;
    public static int enemiesLeft;
    public TMP_Text statusText;
    public TMP_Text scoreText;
    public string levelName;
    public AudioClip winSFX;
    public AudioClip loseSFX;
    public Image fade;
    private float fadeSpeed;

    public static bool isGameOver = false;

    private void Awake()
    {
        // set score to 0
        FadeIn();
    }

    void Start()
    {
        if (levelName == "Alpha Release")
        {
            enemiesLeft = 2;
            levelDuration = 0;
        }
        else if (levelName == "Level 1")
        {
            enemiesLeft = 4;
            levelDuration = 0;
        }
        else if (levelName == "Level 2")
        {
            enemiesLeft = 0;
            levelDuration = 90;
        }
        else if (levelName == "Level 3")
        {
            enemiesLeft = 30;    // figure out
            levelDuration = 120;
        }

        isGameOver = false;

        countdown = levelDuration;

        // if the level does not start as a timed level 
        if (levelName != "Level 2")
            scoreText.text = "Enemies Remaining: " + enemiesLeft.ToString();
    }

    private void Update()
    {
        if (!isGameOver)
        {
            if (levelName == "Level 2" || (levelName == "Level 3" && enemiesLeft <= 0))
            {
                if (countdown > 0)
                    countdown -= Time.deltaTime;
                else
                {
                    countdown = 0.0f;
                    LevelLost();
                }
                scoreText.text = countdown.ToString("0.00");
            }
        }

        fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, fade.color.a + fadeSpeed * Time.deltaTime);
        if (fade.color.a <= 0)
        {
            fadeSpeed = 0;
        }
    }

    void SetGameOverStatus(string gameMessage, Color color,  AudioClip statusSFX)
    {
        isGameOver = true;
        statusText.text = gameMessage;
        statusText.color = color;
        statusText.enabled = true;

        AudioSource.PlayClipAtPoint(statusSFX, Camera.main.transform.position);
    }

    public void EnemyDestroyed()
    {
        enemiesLeft--;
        scoreText.text = "Enemies Left: " + enemiesLeft.ToString();
        if (enemiesLeft <= 0 && levelName != "Level 3" && levelName != "Level 2")
        {
            LevelBeaten();
        }
    }

    public void LevelLost()
    {
        SetGameOverStatus("YOU DIED!", Color.red, loseSFX);
        Invoke("FadeOut", 1);
        Invoke("LoadCurrentLevel", 2);
    }

    public void LevelBeaten()
    {
        SetGameOverStatus(levelName + " PASSED!", Color.green, winSFX);
        Invoke("FadeOut", 1);
        Invoke("LoadNextLevel", 2);
    }

    private void LoadCurrentLevel()
    {
        SceneManager.LoadScene(levelName);
        //FadeIn();
    }

    private void LoadNextLevel()
    {
        if (levelName == "Alpha Release")
            SceneManager.LoadScene("Level 2");
        else if (levelName == "Level 1")
            SceneManager.LoadScene("Level 2");
        else if (levelName == "Level 2")
            SceneManager.LoadScene("Level 3");
        else if (levelName == "Level 3")
            SceneManager.LoadScene("EndScreen");
    }

    private void FadeIn()
    {
        fadeSpeed = -0.3f;
    }

    private void FadeOut()
    {
        fadeSpeed = 1f;
    }
}
