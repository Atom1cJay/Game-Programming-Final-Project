using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour, IUIElement
{

    [SerializeField] KeyCode keyCodeToEnter;
    bool released = true;

    [SerializeField] List<GameObject> objectsToEnable, objectsToDisable;
    [SerializeField] List<Object> otherElements;

    // Update is called once per frame
    void Update()
    {
        if (released && Input.GetKey(keyCodeToEnter))
        {
            released = false;
            useElement();
        }
        if (!Input.GetKey(keyCodeToEnter))
        {
            released = true;
        }
    }

    public void useElement()
    {
        foreach (GameObject g in objectsToEnable)
            g.SetActive(true);
        

        foreach (GameObject g in objectsToDisable)
            g.SetActive(false);


        foreach (Object g in otherElements)
        {
            if ((IUIElement)g != null)
                ((IUIElement)g).useElement();
        }
    }
}
