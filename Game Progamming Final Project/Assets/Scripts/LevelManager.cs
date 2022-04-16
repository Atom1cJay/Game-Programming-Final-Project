using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static int enemiesLeft;
    public TMP_Text statusText;
    public TMP_Text scoreText;
    public string levelName;
    public AudioClip winSFX;
    public AudioClip loseSFX;

    public static bool isGameOver = false;

    private void Awake()
    {
        // set score to 0
    }

    void Start()
    {
        if (levelName == "Alpha Release")
            enemiesLeft = 2;
        else if (levelName == "Level 2")
            enemiesLeft = 13;    //change depending on number of enemies in level
        else if (levelName == "Level 3")
            enemiesLeft = 10;   //change depending on number of enemies in level

        isGameOver = false;

        scoreText.text = "Enemies Remaining: " + enemiesLeft.ToString();
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
        if (enemiesLeft == 0)
        {
            LevelBeaten();
        }
    }

    public void LevelLost()
    {
        SetGameOverStatus("YOU DIED!", Color.red, loseSFX);
        Invoke("LoadCurrentLevel", 2);
    }

    public void LevelBeaten()
    {
        SetGameOverStatus(levelName + " PASSED!", Color.green, winSFX);
        Invoke("LoadNextLevel", 2);
    }

    private void LoadCurrentLevel()
    {
        SceneManager.LoadScene(levelName);
    }

    private void LoadNextLevel()
    {
        if (levelName == "Alpha Release")
            SceneManager.LoadScene("LEVEL 2");
        else if (levelName == "LEVEL 2")
            SceneManager.LoadScene("LEVEL 3");
    }
}
