using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetSystemInfo : MonoBehaviour
{

    private TextMeshProUGUI sysInfo;

    // Start is called before the first frame update
    void Start()
    {
        sysInfo = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        sysInfo.text = "Device name: " + SystemInfo.deviceName + "\n" +
            "Graphics device name: " + SystemInfo.graphicsDeviceName + "\n" +
            "Operating system: " + SystemInfo.operatingSystem + "\n" +
            "Time spent ingame: " + Time.time;
    }
}
