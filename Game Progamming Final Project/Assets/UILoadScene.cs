using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoadScene : MonoBehaviour, IUIElement
{
    [SerializeField] string sceneName;

    public void useElement()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
