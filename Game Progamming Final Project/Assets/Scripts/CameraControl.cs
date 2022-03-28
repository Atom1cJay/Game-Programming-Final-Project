using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float sensitivity = 100.0f;
    public float pitchClamp = 75f;

    private Transform playerT;
    private float pitch;
    private float yaw;


    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerT = transform.parent;
    }

    // Start is called before the first frame update
    void Start()
    {
        pitch = 0f;
        yaw = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Look X") * sensitivity * Time.deltaTime;
        float moveY = Input.GetAxis("Look Y") * sensitivity * Time.deltaTime;

        //Debug.Log(moveY);

        // yaw
        yaw += moveX;

        // pitch
        pitch -= moveY;
        pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);

        transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        playerT.localRotation = Quaternion.Euler(0, yaw, 0);

    }
}
