using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILoadScene : MonoBehaviour, IUIElement
{
    [SerializeField] string sceneName;
    [SerializeField] Slider sensitivitySlider;

    public void useElement()
    {
        CameraControl.sensitivity = sensitivitySlider.value;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
