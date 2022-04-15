using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    
    public float moveSpeed = 10f;
    public float jumpHeight = 5f;
    public float gravity = 9.81f;
    public float airControl = 10f;
    public float sprintFactor = 2f;

    private Vector3 input;
    Vector3 moveDirection;
    CharacterController _controller;

    Vector3 appliedForce;

    Vector3 externalAcc;
    Vector3 externalVel;
    public float accDamping;
    public float velDamping;
    public float externalMovementModifier = 1;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        appliedForce = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // get inputs
        float moveH = Input.GetAxisRaw("Horizontal");
        float moveV = Input.GetAxisRaw("Vertical");

        //Debug.Log("HAxis: " + moveH + ", VAxis: " + moveV);

        // Create movement from the input vectors
        input = (transform.right * moveH + transform.forward * moveV + transform.up * 0).normalized;

        if (_controller.isGrounded)
        {
            moveDirection = input;
            //Debug.Log("Grounded!");
            // we can jump
            if (Input.GetButton("Jump"))
            {
                // jump
                moveDirection.y = Mathf.Sqrt(2 * gravity * jumpHeight);

            }
            else
            {
                // ground the object
                moveDirection.y = 0.0f;
            }
        }
        else
        {
            // we are in the air
            input.y = moveDirection.y;

            moveDirection = Vector3.Lerp(moveDirection, input, Time.deltaTime * airControl);

            moveDirection.y -= gravity * Time.deltaTime;
        }

        Vector3 baseMovement = moveDirection * moveSpeed;


        // apply external acceleration and velocity
        externalVel += externalAcc;
        externalAcc /= accDamping;
        externalVel /= velDamping;
        Debug.Log("Applying velocity: " + externalVel * externalMovementModifier * Time.deltaTime);
        Vector3 externalMovement = externalVel * externalMovementModifier;


        _controller.Move((baseMovement + externalMovement) * Time.deltaTime);


        //_controller.Move(appliedForce * Time.deltaTime);
        //appliedForce = Vector3.Lerp(appliedForce, Vector3.zero, Time.deltaTime);

    }

    private void FixedUpdate()
    {
        
    }


    public void ApplyForce(Vector3 force)
    {
        force.y += moveDirection.y;
        //appliedForce = force;
        Debug.Log(force);
        _controller.Move(force * Time.deltaTime);
    }

    public void AddAcceleration(Vector3 acceleration)
    {
        externalAcc += acceleration;
    }

    public void AddVelocity(Vector3 velocity)
    {
        externalVel += velocity;
    }



}
