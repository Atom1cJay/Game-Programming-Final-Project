using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetSensitivityNumber : MonoBehaviour
{

    public Slider slider;
    private TextMeshProUGUI text;
    

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = "" + Mathf.Round(slider.value);
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "" + Mathf.Round(slider.value);
    }
}
