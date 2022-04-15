using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	//cam and controller

	public GameObject cam;
	public Camera camComponent;

	public Rigidbody rb;

	float headBob = 0f;
	float initialCamY;

	//Input
	private float xInput;
	private float zInput;

	//movmeent
	public float speed;
	public float gravity;
	public float jumpHeight;
	public float sprintMultiplier;
	public float sneakMultiplier;
	public float headBobMult;
	public float headBobSpeed;


	Vector3 velocity;

	//ground
	public Transform groundCheck;
	public float groundDistance;
	public LayerMask groundMask;

	bool isGrounded;

	//other vars
	float currentSpeed;
	private float dragMult;

	private float walkFOV = 65f;
	private float sprintFOV = 71f;
	private float intendedFOV = 65f;

	private float lerpFOV = 65f;

	private float lerpHeight;
	private float intendedHeight;

	[SerializeField] float distanceBetweenStep;
	bool rightStep = true;
	float distanceFromLastStep = 0f;

	//claw shiz
	public float forceChangeSpeed = 1f;
	Vector3 currentForce = Vector3.zero;
	Vector3 targetForce = Vector3.zero;

	[SerializeField]
	float bounceMult, maxVelocity;

	private void Start()
	{
		//get initial cam positio
		initialCamY = cam.transform.localPosition.y;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Time.timeScale = 1f;
	}

	private void FixedUpdate()
	{
		//perfrom movement functions on update
		MovementController();
	}

	private void MovementController()
	{
		InfluenceVelocity();
		CharacterPhysics();
		HeadBob();
		CameraFOV();

		//if miniminal minimal input is provided dont move
		if (Mathf.Abs(zInput) + Mathf.Abs(xInput) < 0.1f)
		{
			currentSpeed = 0f;
			distanceFromLastStep = distanceBetweenStep;
		}
		else
        {
			FootstepController();
		}

	}

	private void InfluenceVelocity()
	{
		//ground check
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

		//initialize inputs
		xInput = 0f;
		zInput = 0f;

		//movement controls
		if (Input.GetKey(KeyCode.A))
		{
			xInput += -1f;
		}
		if (Input.GetKey(KeyCode.D))
		{
			xInput += 1f;
		}
		if (Input.GetKey(KeyCode.W))
		{
			zInput += 1f;
		}
		if (Input.GetKey(KeyCode.S))
		{
			zInput += -1f;
		}

		//sloower  back input
		if (zInput < 0) { zInput *= 0.75f; }

		//movement vector controls
		Vector3 moveVector = transform.right * xInput * 0.85f + transform.forward * zInput;
		moveVector = new Vector3(moveVector.x, 0f, moveVector.z);

		PlayerMovementType currentMoveType = GetCurrentMoveType(out float speedMult);
		currentSpeed = speed * speedMult;

		velocity += moveVector * currentSpeed * 6f * Time.deltaTime;

		currentForce = Vector3.Lerp(currentForce, targetForce, Time.deltaTime * forceChangeSpeed);

		if (currentForce.magnitude < 3f)
			currentForce = Vector3.zero;


		if (Input.GetButton("Jump") && isGrounded)
		{
			isGrounded = false;
			distanceFromLastStep = distanceBetweenStep;
			velocity.y = jumpHeight;
		}

		//Debug.Log(currentForce);

		velocity += currentForce * Time.deltaTime;

		if (isGrounded && velocity.y < 0f)
			velocity.y = 0f;

	}

	//get curren movetype and set up all FOV's, speedMults, etc.
	private PlayerMovementType GetCurrentMoveType(out float speedMult)
	{
		if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
		{
			speedMult = sprintMultiplier;
			dragMult = 0.5f;
			intendedFOV = sprintFOV;
			intendedHeight = 0f;
			return PlayerMovementType.Sprinting;
		}
		else if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
		{
			speedMult = sneakMultiplier;
			dragMult = 2f;
			intendedFOV = walkFOV;
			intendedHeight = -0.5f;
			return PlayerMovementType.Crouching;
		}
		else
		{
			speedMult = 1f;
			dragMult = 1f;
			intendedFOV = walkFOV;
			intendedHeight = 0f;
			return PlayerMovementType.Walking;
		}
	}

	private void CharacterPhysics()
	{
		//physics
		float drag = 4f;
		float effectiveDrag = 1 - drag * dragMult * Time.deltaTime;
		velocity.x *= effectiveDrag;
		velocity.z *= effectiveDrag;
		velocity.y += gravity * Time.deltaTime;

		velocity = Vector3.ClampMagnitude(velocity, maxVelocity);

		rb.velocity = velocity;
	}

	private void HeadBob()
	{
		//Crouching check
		lerpHeight = Mathf.Lerp(lerpHeight, intendedHeight, Time.deltaTime * 6f);

		//clamp
		if (headBob < 1f || headBob > 360f)
		{
			headBob = 1f;
		}

		//Head bob maths
		if (isGrounded && (xInput != 0 || zInput != 0))
		{
			float trueWalkingSpeed = (new Vector2(rb.velocity.x, rb.velocity.z)).magnitude * 0.85f;
			headBob += (360f * headBobSpeed * Time.deltaTime * trueWalkingSpeed / 2f);
		}
		else
		{
			if (headBob <= 180f)
				headBob -= 360f * Time.deltaTime;
			else if (headBob < 360f)
				headBob += 360f * Time.deltaTime;
			else
				headBob = 0f;
		}


		//actual bobing
		float headBobPos = Mathf.Sin(headBob * Mathf.Deg2Rad) * headBobMult;
		bool temp = isGrounded && (xInput != 0 || zInput != 0);

		//camera pos getting updated
		cam.transform.localPosition = new Vector3(0f, initialCamY + headBobPos + lerpHeight, 0f);
	}

	void FootstepController()
	{
		if (!isGrounded)
			return;

		if (xInput != 0 || zInput != 0)
			distanceFromLastStep += rb.velocity.magnitude * Time.deltaTime;
		else
			distanceFromLastStep = 0f;

		if (distanceFromLastStep >= distanceBetweenStep)
		{
			distanceFromLastStep = 0f;

			if (rightStep)
			{

				AudioMasterController.instance.playSound("RStep");
			}
			else
			{
				AudioMasterController.instance.playSound("LStep");
			}

			rightStep = !rightStep;
		}
	}

	private void CameraFOV()
	{
		//always lerping to the camera fov we want
		lerpFOV = Mathf.Lerp(lerpFOV, intendedFOV, Time.deltaTime * 9f);
		camComponent.fieldOfView = lerpFOV;
	}

	public void ApplyForce(Vector3 force)
	{

		targetForce = force;
	}

	public void AddAcceleration(Vector3 acceleration)
	{
		//externalAcc += acceleration;
	}

	public void AddVelocity(Vector3 velocity)
	{
		//externalVel += velocity;
	}

    private void OnCollisionEnter(Collision collision)
	{
		if (!isGrounded)
		{
			ContactPoint point = collision.contacts[0];

			velocity += point.normal * bounceMult;
			//targetForce = Vector3.zero;
			//velocity.y = -10f;
			Debug.Log("hit something");
		}
    }
}

//types of movement, simple enough lol
public enum PlayerMovementType
{
	Walking,
	Sprinting,
	Crouching
}
