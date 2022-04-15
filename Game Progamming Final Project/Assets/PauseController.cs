using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour, IUIElement
{
    bool paused = false;
    [SerializeField] GameObject pauseMenu;

    public void setTimePaused(bool pause)
    {
        if (pause)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
    }

    public void useElement()
    {
        paused = !paused;
        setTimePaused(paused);   
    }
}
