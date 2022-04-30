using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOnLoad : MonoBehaviour
{

    public float delay = 0f;
    public float lerpSpeed = 1f;
    public float distance = 5f;

    private Vector3 startPosition;
    private Color startColor;
    private CanvasRenderer[] canvasRenderers;

    private bool shouldMove;

    void Awake()
    {
        startPosition = transform.position;

        // move the ui element down
        transform.position += Vector3.down * distance;

        // make it invisible
        canvasRenderers = this.GetComponentsInChildren<CanvasRenderer>();
        foreach (CanvasRenderer cr in canvasRenderers)
        {
            cr.SetAlpha(0);
        }

        shouldMove = false;
        Invoke("MoveUp", delay);
    }

    void Update()
    {
        if (shouldMove)
        {
            transform.position = Vector3.Lerp(transform.position, startPosition, Time.deltaTime * lerpSpeed);
            foreach (CanvasRenderer cr in canvasRenderers)
            {
                cr.SetAlpha(Mathf.Lerp(cr.GetAlpha(), 1, Time.deltaTime * lerpSpeed));
            }
            
        }
    }

    private void MoveUp()
    {
        shouldMove = true;
    }

}
