using UnityEngine;

[RequireComponent(typeof(CharacControllerRecu))]
public class Player : MonoBehaviour {


	#region speed variables
	/// <summary>
	/// The character speed.
	/// </summary>
	public float characSpeed = 5f;
	/// <summary>
	/// The speed at which the player's speed changes on the ground.
	/// </summary>
	float speedSmoothTimeGround;
	/// <summary>
	/// The default time it takes for the player to get to their maximum speed.
	/// </summary>
	[Tooltip("The default time it takes for the player to get to  maximum speed.")]
	public float groundAccelerationSmoothTime_default = .2f;
	/// <summary>
	/// The default time it takes for the player to decelerate.
	/// </summary>
	[Tooltip("The default time it takes for the player to decelerate.")]
	public float groundDecelerationSmoothTime_default = .1f;
	/// <summary>
	/// The time it takes for the player to get to their maximum speed when turning back.
	/// </summary>
	[Tooltip("The time it takes for the player to get to their maximum speed when turning back.")]
	public float groundAccelerationSmoothTime_uTurn = .7f;
	/// <summary>
	/// The speed at which the player's speed changes in the air.
	/// </summary>
	[Tooltip("The speed at which the player's speed changes in the air.")]
	public float airSpeedSmoothTime = .3f;
	/// <summary>
	/// Variable used in the speed smooth damp.
	/// </summary>
	float speedSmoothVelocity;
	/// <summary>
	/// The current speed of the player.
	/// </summary>
	float currentSpeed;
	#endregion speed variables

	#region turning variables
	/// <summary>
	/// Character turning speed.
	/// </summary>
	float turnSmoothTime;
	/// <summary>
	/// Variable used for turning the character via smooth damp.
	/// </summary>
	Vector3 turnSmoothVelocity;
	/// <summary>
	/// The default time it takes for the player to turn towards the player's input.
	/// </summary>
	[Tooltip("The default time it takes for the player to turn towards the player's input.")]
	public float turnSmoothTime_default = .11f;
	/// <summary>
	/// The time it takes for the player to turn towards the player's input when turning back (make it instant).
	/// </summary>
	[Tooltip("The time it takes for the player to turn towards the player's input when turning back (make it instant).")]
	public float turnSmoothTime_uTurn = .01f;
	/// <summary>
	/// Is the input at the opposite side of the player's forward vector.
	/// </summary>
	bool suddenStop;
	#endregion turning variables

	#region jump variables
	[Header("Jump parameters")]
	/// <summary>
	/// The minimum height of the jump.
	/// </summary>
	[Tooltip("The minimum height of the jump.")]
	public float minJumpHeight = 3f;
	/// <summary>
	/// The maximum height of the jump.
	/// </summary>
	[Tooltip("The maximum height of the jump.")]
	public float maxJumpHeight = 6f;
	/// <summary>
	/// The time it takes for the player to reach their jump apex.
	/// </summary>
	[Tooltip("The time it takes for the player to reach their jump apex.")]
	public float timeToJumpApex = .4f;

	/// <summary>
	/// Calculated at the start from maxJumpHeight and timeToJumpApex.
	/// </summary>
	float gravity;
	/// <summary>
	/// The maximum jump velocity calculated at the start.
	/// </summary>
	float maxJumpVelocity;
	/// <summary>
	/// The minimum jump velocity calculated at the start.
	/// </summary>
	float minJumpVelocity;
	#endregion jump variables

	/// <summary>
	/// The velocity calculated each frame and sent to the controller.
	/// </summary>
	Vector3 velocity;

	/// <summary>
	/// The gravi pente.
	/// </summary>
	float graviPente;
	public float graviPenteCoeff = .2f;

	/// <summary>
	/// The controller checking if there's collisions on the way.
	/// </summary>
	CharacControllerRecu controller;

	[Space(20)]
	/// <summary>
	/// The rotator used to turn the player.
	/// </summary>
	public Transform rotator;

	void Start(){
		controller = GetComponent<CharacControllerRecu> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}


	void Update(){

		float angle = Vector3.Angle (Vector3.up, transform.up);

		//Get the input of the player and translate it into the camera angle
		Vector3 input = rotator.forward * Input.GetAxisRaw ("Vertical") + rotator.right * Input.GetAxisRaw ("Horizontal");

		//Turn the player in the direction of the input and decelerating when turning back
		#region turning the player 
		if (input != Vector3.zero) {


			//If the input is at the opposite side of the player's forward, turn instantly and slow down the player
			turnSmoothTime = Vector3.Dot (transform.forward, input) < -.75f ? turnSmoothTime_uTurn : turnSmoothTime_default;
			if (Vector3.Dot (transform.forward, input) < -.75f) {
				if (!suddenStop) {
					currentSpeed = -currentSpeed;
					speedSmoothTimeGround = groundAccelerationSmoothTime_uTurn;
					suddenStop = true;
				} else {
					speedSmoothTimeGround = Mathf.Clamp (speedSmoothTimeGround - Time.deltaTime, groundAccelerationSmoothTime_default, groundAccelerationSmoothTime_uTurn);
				}
			} else {
				suddenStop = false;
				speedSmoothTimeGround = groundAccelerationSmoothTime_default;
			}

			Vector3 direction = Vector3.SmoothDamp (transform.forward, input, ref turnSmoothVelocity, turnSmoothTime);

			transform.rotation = Quaternion.LookRotation (direction, transform.up);

		} else {
			suddenStop = false;
			speedSmoothTimeGround = groundDecelerationSmoothTime_default;
		}
		#endregion turning the player

		#region update velocity
		// Calculate current speed of the player
		float targetSpeed = characSpeed * Mathf.Clamp01(input.magnitude);
		currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, (controller.collisions.below ? speedSmoothTimeGround : airSpeedSmoothTime));

		//Combine speed and direction to calculate horizontal components of the velocity vector
		velocity.x = Vector3.ProjectOnPlane(transform.forward, rotator.forward).magnitude * currentSpeed * (Vector3.Dot(transform.forward, rotator.right) > 0 ? 1 : -1);
		velocity.z = Vector3.ProjectOnPlane(transform.forward, rotator.right).magnitude * currentSpeed * (Vector3.Dot(transform.forward, rotator.forward) > 0 ? 1 : -1);

		//Translate the forward of the camera in a standard plane to rotate the horizontal velocity accordingly
		Vector3 inputDir = (Quaternion.AngleAxis(-angle, Vector3.Cross(Vector3.up, transform.up))) * rotator.forward;
		float targetRotation = Vector3.Angle(Vector3.forward, inputDir) * (Vector3.Dot(inputDir, Vector3.right) > 0 ? 1 : -1);
		velocity = Quaternion.Euler (0, targetRotation, 0) * velocity;


		//Reset vertical velocity if the player is on the ground or hitting the ceiling
		if ((!controller.collisions.below && !controller.collisions.above) || controller.collisions.onSteepSlope) {
			velocity.y += gravity * Time.deltaTime;
		}
		if(!controller.collisions.onSteepSlope)
			graviPente = 0;
		
		#endregion update velocity

		#region Jump controls
		//Detects jump input from the player and adds vertical velocity
		if (Input.GetButtonDown ("Jump") && controller.collisions.below) {
			if (controller.collisions.onSteepSlope){
				velocity.y = maxJumpVelocity;
			} else {
				velocity.y = maxJumpVelocity;
			}
		}

		//graviPente += graviPenteCoeff;

		//Adds the gravity to the velocity

		//Detects the release of the jump button and sets the vertical velocity to its minimum
		if (Input.GetButtonUp ("Jump")) {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}
		#endregion


		//Debug.Log (velocity.y);


		//Calls the controller to check if the calculated velocity will run into walls and stuff
		velocity = controller.Move ((Quaternion.AngleAxis(angle, Vector3.Cross(Vector3.up, transform.up))) * velocity * Time.deltaTime);
	}

}
