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

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // get inputs
        float moveH = Input.GetAxisRaw("Horizontal");
        float moveV = Input.GetAxisRaw("Vertical");

        //Debug.Log("HAxis: " + moveH + ", VAxis: " + moveV);

        // Create movement from the input vectors
        input = (transform.right * moveH + transform.forward * moveV).normalized;

        // sprinting?
        if (Input.GetButton("Sprint"))
        {
            input *= 2;
        }

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
        }




        moveDirection.y -= gravity * Time.deltaTime;
        
        _controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        
    }
}
