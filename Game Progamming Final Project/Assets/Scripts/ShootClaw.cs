using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootClaw : MonoBehaviour
{
    //public GameObject projectilePrefab;
    //public float launchPower = 100f;

    public float reticleRange = 100f;
    public Image reticleImage;
    public Color grappleableReticleColor;
    Color baseReticleColor;

    public float reticleChangeSpeed = 10f;

    private bool isExtended;
    private bool isRetracting;

    public GameObject claw;
    public float clawMoveSpeed = 2f;
    public float clawLaunchPower = 1f;

    public float clawSecondsMaximum = 3f;
    private float clawCount;

    private Transform playerTransform;

    private void Start()
    {
        baseReticleColor = reticleImage.color;
        isExtended = false;
        isRetracting = false;
        clawCount = clawSecondsMaximum;
        playerTransform = GetComponentInParent<Transform>();
    }

    void Update()
    {

        clawCount -= Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && !isExtended)
        {
            Shoot();
            isExtended = true;
        }

        if (isExtended && clawCount > 0)
        {
            Debug.DrawLine(transform.position, claw.transform.position);
            MoveClaw();
        }

        if (clawCount <= 0)
        {
            claw.SetActive(false);
            isExtended = false;
            clawCount = clawSecondsMaximum;
        }

    }

    private void Shoot()
    {
        claw.transform.position = transform.position;
        float pitch = transform.rotation.eulerAngles.x;
        float yaw = playerTransform.rotation.eulerAngles.y;
        claw.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        claw.SetActive(true);
    }

    private void MoveClaw()
    {
        float moveDistance = clawMoveSpeed * Time.deltaTime;
        Debug.DrawLine(claw.transform.position, (claw.transform.position + (claw.transform.forward * moveDistance)), Color.red, 0.5f);
        RaycastHit hit;
        if (Physics.Raycast(claw.transform.position, claw.transform.forward, out hit, (moveDistance * 1.1f)))
        {
            //Debug.Log(hit.collider + ", moveDist: " + moveDistance + ", hit.dist: " + hit.distance);
            //hit.
            //claw.transform.Translate(Vector3.forward * hit.distance);
            ClawHit(hit.collider.tag, hit.point);
        }
        else
        {
            claw.transform.Translate(Vector3.forward * moveDistance);
        }

        //claw.GetComponent<Rigidbody>().MovePosition(claw.transform.position + (claw.transform.forward * clawMoveSpeed * Time.deltaTime));

    }

    public void ClawHit(string otherTag, Vector3 hitLocation)
    {
        clawCount = 0;

        //Debug.Log("Claw hit tag " + otherTag + ", at " + hitLocation);

        if (otherTag.Equals("Grappleable"))
        {
            CharacterController playerRB = GetComponentInParent<CharacterController>();
            playerRB.Move((hitLocation - transform.position) * clawLaunchPower);
            
            isRetracting = true;
        }
        
    }






    // Reticle effect:

    private void FixedUpdate()
    {
        ReticleEffect();
    }

    void ReticleEffect()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, reticleRange))
        {
            if (hit.collider.CompareTag("Grappleable"))
            {
                reticleImage.color = Color.Lerp(
                    reticleImage.color,
                    grappleableReticleColor,
                    Time.deltaTime * reticleChangeSpeed);

                reticleImage.transform.localScale = Vector3.Lerp(
                    reticleImage.transform.localScale,
                    new Vector3(0.7f, 0.7f, 1),
                    Time.deltaTime * reticleChangeSpeed);
            }
        }
        else
        {
            reticleImage.color = Color.Lerp(
                    reticleImage.color,
                    baseReticleColor,
                    Time.deltaTime * reticleChangeSpeed);

            reticleImage.transform.localScale = Vector3.Lerp(
                reticleImage.transform.localScale,
                Vector3.one,
                Time.deltaTime * reticleChangeSpeed);
        }

    }
}
