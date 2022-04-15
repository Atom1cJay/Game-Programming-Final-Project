using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootClaw : MonoBehaviour
{
    //public GameObject projectilePrefab;
    //public float launchPower = 100f;

    
    //public float clawPullForceXZ = 5f;
    //public float clawPullForceY = 10f;

    public float reticleRange = 100f;
    public Image reticleImage;
    public Color grappleableReticleColor;
    Color baseReticleColor;

    public float reticleChangeSpeed = 10f;


    public enum ClawState
    {
        neutral, extending, attached, retracting
    }

    [HideInInspector]
    public ClawState state;
    private Vector3 attachLocation;

    public GameObject claw;
    public GameObject clawVisual;
    public float clawMoveSpeed = 2f;
    public float clawLaunchPower = 1f;

    public float clawSecondsMaximum = 3f;
    private float clawCount;

    private Transform playerTransform;

    AudioSource audio;
    public AudioClip shootSFX;
    public AudioClip returnSFX;

    public bool normalizeLaunch = true;

    PlayerController pc;

    //added code -jared=
    [SerializeField] LineRenderer lr;
    public float clawForce = 30f;
    public float clawMaxForce = 20f;


    private void Start()
    {
        pc = GetComponentInParent<PlayerController>();
        baseReticleColor = reticleImage.color;
        state = ClawState.neutral;
        clawCount = clawSecondsMaximum;
        playerTransform = GetComponentInParent<Transform>();
        claw.SetActive(false);
        clawVisual.SetActive(true);
        if (audio == null)
        {
            audio = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        

        //Debug.Log("claw count: " + clawCount + ", state: " + state);

        switch (state)
        {
            case ClawState.neutral:
                NeutralUpdate();
                break;
            case ClawState.extending:
                Debug.DrawLine(transform.position, claw.transform.position);
                MoveClaw();
                break;
            case ClawState.attached:
                AttachedUpdate();
                break;
            case ClawState.retracting:
                RetractClaw();
                break;
            default:
                break;
        }


    }

    private void LateUpdate()
    {
        if (state != ClawState.neutral)
            DrawRope();
    }

    private void NeutralUpdate()
    {
        lr.positionCount = 0;

        if (Input.GetButtonDown("Fire1"))
        {
            audio.PlayOneShot(shootSFX);
            Shoot();
            state = ClawState.extending;
        }

        pc.ApplyForce(Vector3.zero);
    }

    private void Shoot()
    {
        claw.transform.position = transform.position;
        float pitch = transform.rotation.eulerAngles.x;
        float yaw = playerTransform.rotation.eulerAngles.y;
        claw.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        clawVisual.SetActive(false);
        claw.SetActive(true);
        clawCount = clawSecondsMaximum;
    }

    private void MoveClaw()
    {
        clawCount -= Time.deltaTime;

        float moveDistance = clawMoveSpeed * Time.deltaTime;
        Debug.DrawLine(claw.transform.position, (claw.transform.position + (claw.transform.forward * moveDistance)), Color.red, 0.5f);
        RaycastHit hit;
        if (Physics.Raycast(claw.transform.position, claw.transform.forward, out hit, (moveDistance * 1.1f)))
        {
            //Debug.Log(hit.collider + ", moveDist: " + moveDistance + ", hit.dist: " + hit.distance);
            //hit.
            //claw.transform.Translate(Vector3.forward * hit.distance);

            clawCount = 0;

            if (hit.collider.tag.Equals("Grappleable"))
            {
                //CharacterController playerRB = GetComponentInParent<CharacterController>();
                //playerRB.Move((hitLocation - transform.position) * clawLaunchPower);

                attachLocation = hit.point;
                claw.transform.position = attachLocation;
                state = ClawState.attached;
                return;
            }
        }
        else
        {
            claw.transform.Translate(Vector3.forward * moveDistance);
        }

        if (clawCount <= 0 || Input.GetButtonUp("Fire1"))
        {
            state = ClawState.retracting;
        }

        pc.ApplyForce(Vector3.zero);
        //claw.GetComponent<Rigidbody>().MovePosition(claw.transform.position + (claw.transform.forward * clawMoveSpeed * Time.deltaTime));

    }


    private void AttachedUpdate()
    {
        //Rigidbody _rb = GetComponentInParent<Rigidbody>();

        //Debug.DrawLine(_rb.transform.position, attachLocation, Color.green);
        Debug.DrawRay(transform.position, (attachLocation - transform.position), Color.cyan);
        //_rb.AddForce((attachLocation - _rb.transform.position), ForceMode.Acceleration);

        Vector3 forceToApply = (attachLocation - transform.position);

        /*
        if (normalizeLaunch && forceToApply.magnitude > 1)
        {
            forceToApply = forceToApply.normalized;
        }

        forceToApply.y = forceToApply.y * clawPullForceY;
        if (forceToApply.y < 0) forceToApply.y = 0;
        forceToApply.x = forceToApply.x * clawPullForceXZ;
        forceToApply.z = forceToApply.z * clawPullForceXZ;
        */

        PlayerController cm = GetComponentInParent<PlayerController>();

        Vector3.ClampMagnitude(forceToApply, 10f);

        cm.ApplyForce(forceToApply * clawForce);

        if (Input.GetButtonUp("Fire1"))
        {
            state = ClawState.retracting;
        }
    }


    void DrawRope()
    {
        if (!claw.activeSelf)
            return;

        lr.positionCount = 2;

        lr.SetPosition(0, transform.root.position);
        lr.SetPosition(1, claw.transform.position);
    }
    private void RetractClaw()
    {
        float moveDistance = clawMoveSpeed * Time.deltaTime;
        
        //claw.transform.Translate(Vector3.back * moveDistance);
        claw.transform.position = Vector3.MoveTowards(claw.transform.position, transform.position, moveDistance);

        if (Vector3.Distance(claw.transform.position, playerTransform.position) < (moveDistance * 1.1f))
        {
            audio.PlayOneShot(returnSFX);
            claw.SetActive(false);
            clawVisual.SetActive(true);
            state = ClawState.neutral;
        }

        pc.ApplyForce(Vector3.zero);
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
            else
            {
                RestoreReticleColor();
            }
        }
        else
        {
            RestoreReticleColor();
        }

    }

    private void RestoreReticleColor()
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
