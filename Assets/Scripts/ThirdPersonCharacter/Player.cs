using UnityEngine;
using Game.Player;

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
	/// The value by which the speed is multiplied when the player sprints.
	/// </summary>
	[Tooltip("The value by which the speed is multiplied when the player sprints.")]
	public float sprintCoeff = 2f;
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
	[Header("Aerial Jumps")]
	/// <summary>
	/// The number of jumps the player can do while in the air.
	/// </summary>
	[Tooltip("The number of jumps the player can do while in the air.")]
	public int numberOfAerialJumps = 0;
	/// <summary>
	/// The efficiency of the aerial jump compared to the regular jump (2 makes it 2 times stronger, 0.5 makes it 2 times weaker (not really, play around with it)).
	/// </summary>
	[Tooltip("The efficiency of the aerial jump compared to the regular jump (2 makes it 2 times stronger, 0.5 makes it 2 times weaker (not really, play around with it)).")]
	public float coeffAerialJumpEfficiency = 1f;


	/// <summary>
	/// The current number of aerial jumps remaining to the player.
	/// </summary>
	int rmngAerialJumps;
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
	/// <summary>
	/// The maximum aerial jump velocity calculated at the start.
	/// </summary>
	float maxAerialJumpVelocity;
	/// <summary>
	/// The minimum aerial jump velocity calculated at the start.
	/// </summary>
	float minAerialJumpVelocity;
	/// <summary>
	/// States if the last jump is an aerial one or a regular one.
	/// </summary>
	bool lastJumpAerial = false;

	#endregion jump variables

	#region glide variables

	[Header("Glide")]
	/// <summary>
	/// The glide speed.
	/// </summary>
	public float glideSpeed = 5f;
	/// <summary>
	/// The speed that can be added when the player tilts forward.
	/// </summary>
	[Tooltip("The speed that can be added when the player tilts forward.")]
	public float glideMaxSpeed = 3f;
	/// <summary>
	/// The speed at which the avatar adjusts his Left and Right tilt to follow the player input.
	/// </summary>
	[Tooltip("The speed at which the avatar adjusts his Left and Right tilt to follow the player input.")]
	public float glideLRAttitudeTiltingSpeed = .2f;
	/// <summary>
	/// The speed at which the avatar comes back to a plane attitude after being tilted Left or Right.
	/// </summary>
	[Tooltip("The speed at which the avatar comes back to a plane attitude after being tilted Left or Right.")]
	public float glideLRAttitudeRecoverSpeed = 2.5f;
	/// <summary>
	/// The speed at which the avatar adjusts his Forward tilt to follow the player input.
	/// </summary>
	[Tooltip("The speed at which the avatar adjusts his Forward tilt to follow the player input.")]
	public float glideForwardAttitudeTiltingSpeed = 2f;
	/// <summary>
	/// The speed at which the avatar comes back to a plane attitude after being tilted Forward.
	/// </summary>
	[Tooltip("The speed at which the avatar comes back to a plane attitude after being tilted Forward.")]
	public float glideForwardAttitudeRecoverSpeed = 1f;
	/// <summary>
	/// The speed at which the avatar adjusts his Backward tilt to follow the player input.
	/// </summary>
	[Tooltip("The speed at which the avatar adjusts his Backward tilt to follow the player input.")]
	public float glideBackwardAttitudeTiltingSpeed = .5f;
	/// <summary>
	/// The speed at which the avatar comes back to a plane attitude after being tilted Backward.
	/// </summary>
	[Tooltip("The speed at which the avatar comes back to a plane attitude after being tilted Backward.")]
	public float glideBackwardAttitudeRecoverSpeed = 2f;
	/// <summary>
	/// The maximum turn amount when gliding.
	/// </summary>
	[Tooltip("The maximum turn amount when gliding.")]
	public float glideMaxTurn = 1f;
	/// <summary>
	/// How much does gliding slow down the speed.
	/// </summary>
	[Tooltip("How much does gliding slow down the speed.")]
	public float glideDrag = .2f;
	/// <summary>
	/// The speed at which the velocity is evolving when gliding.
	/// </summary>
	[Tooltip("The speed at which the velocity is going up when gliding.")]
	public float glideAccelerate;
	/// <summary>
	/// The speed at which the velocity is evolving when gliding.
	/// </summary>
	[Tooltip("The speed at which the velocity is slowing down when gliding.")]
	public float glideDecelerate;
	/// <summary>
	/// Is the player gliding ?
	/// </summary>
	bool isGliding = false;
	/// <summary>
	/// The current attitude when the player is gliding.
	/// </summary>
	Vector3 glideAttitude;
	#endregion glide variables

	#region dash variables

	[Header("Dash")]

	bool isDashing;

	#endregion dash variables

	/// <summary>
	/// The velocity calculated each frame and sent to the controller.
	/// </summary>
	Vector3 velocity;

	/// <summary>
	/// The gravi pente.
	/// </summary>
	float graviPente;
	//public float graviPenteCoeff = .2f;

	/// <summary>
	/// The controller checking if there's collisions on the way.
	/// </summary>
	CharacControllerRecu controller;

	/// <summary>
	/// The animator of the character.
	/// </summary>
	Animator animator;

	/// <summary>
	/// The script to get info about abilities.
	/// </summary>
	PlayerModel PlayerMod;

	[Space(20)]
	/// <summary>
	/// The rotator used to turn the player.
	/// </summary>
	public Transform rotator;

	void Start(){
		controller = GetComponent<CharacControllerRecu> ();
		animator = GetComponentInChildren<Animator> ();
		PlayerMod = GetComponent<PlayerModel> ();
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
		maxAerialJumpVelocity = maxJumpVelocity * coeffAerialJumpEfficiency;
		minAerialJumpVelocity = minJumpVelocity * coeffAerialJumpEfficiency;
	}

	void Update(){

		float angle = Vector3.Angle (Vector3.up, transform.up);

		//Get the input of the player and translate it into the camera angle
		Vector3 input = rotator.forward * Input.GetAxisRaw ("Vertical") + rotator.right * Input.GetAxisRaw ("Horizontal");

		if (Input.GetButtonDown ("Dash")) {
			//isDashing = true;
		}

		// Updates the gliding attitude of the player depending of the player's input
		if (isGliding) {
			Vector3 inputGlide = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
			//                                                                                                                            coming back from left/right    tilting left/right
			glideAttitude = new Vector3(Mathf.Lerp (glideAttitude.x, inputGlide.x, (glideAttitude.sqrMagnitude > inputGlide.sqrMagnitude ? glideLRAttitudeRecoverSpeed : glideLRAttitudeTiltingSpeed) * Time.deltaTime)
				//                                                                                                            coming back from forward     tilting forward
				,0, Mathf.Lerp (glideAttitude.z, inputGlide.z, (glideAttitude.z > 0 ? (glideAttitude.z > inputGlide.z ? glideForwardAttitudeRecoverSpeed : glideForwardAttitudeTiltingSpeed) 
					//                                                                                                      coming back from backward    tilting backward
					                                                              : (glideAttitude.z < inputGlide.z? glideBackwardAttitudeRecoverSpeed : glideBackwardAttitudeTiltingSpeed)) * Time.deltaTime));
		} else {
			glideAttitude = Vector2.zero;
		}

		//Turn the player in the direction of the input and decelerating when turning back
		#region turning the player 
		if (input != Vector3.zero && !isGliding && !isDashing) {


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

		//Case when the player is gliding
		if (isGliding) {
			Debug.Log("assiette : " + glideAttitude);
			Vector3 direction = Vector3.SmoothDamp (transform.forward, glideAttitude.x * glideMaxTurn * transform.right * 0.1f + transform.forward, ref turnSmoothVelocity, turnSmoothTime);
			transform.rotation = Quaternion.LookRotation (direction, transform.up);
		}
		#endregion turning the player

		#region update velocity

		if (!isGliding) {
			// Calculate current speed of the player and detects if the player is sprinting
			float targetSpeed = characSpeed * Mathf.Clamp01(input.magnitude) * ((Input.GetButton("Sprint") && controller.collisions.below) ? sprintCoeff : 1);

			
			currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, (controller.collisions.below ? speedSmoothTimeGround : airSpeedSmoothTime));

			//Combine speed and direction to calculate horizontal components of the velocity vector
			velocity.x = Vector3.ProjectOnPlane(transform.forward, rotator.forward).magnitude * currentSpeed * (Vector3.Dot(transform.forward, rotator.right) > 0 ? 1 : -1);
			velocity.z = Vector3.ProjectOnPlane(transform.forward, rotator.right).magnitude * currentSpeed * (Vector3.Dot(transform.forward, rotator.forward) > 0 ? 1 : -1);
		
			//Translate the forward of the camera in a standard plane to rotate the horizontal velocity accordingly
			Vector3 inputDir = (Quaternion.AngleAxis(-angle, Vector3.Cross(Vector3.up, transform.up))) * rotator.forward;
			float targetRotation = Vector3.Angle(Vector3.forward, inputDir) * (Vector3.Dot(inputDir, Vector3.right) > 0 ? 1 : -1);
			velocity = Quaternion.Euler (0, targetRotation, 0) * velocity;
		}

		//Adds the gravity to the velocity
		if (isGliding)
			velocity = Vector3.zero;
		velocity.y += gravity * Time.deltaTime;

		if(!controller.collisions.onSteepSlope)
			graviPente = 0;
		
		#endregion update velocity

		#region jump controls

		//Resets the number of aerial jumps remaining when the player is on the ground
		if (controller.collisions.below){
			rmngAerialJumps = numberOfAerialJumps;
		}

		//Detects jump input from the player and adds vertical velocity
		if (Input.GetButtonDown ("Jump")) {
			if (controller.collisions.below){
				lastJumpAerial = false;
				if (controller.collisions.onSteepSlope){
					//MAYBE TODO: change behavior of jump when on a steep slope
					velocity.y = maxJumpVelocity;
				} else {
					velocity.y = maxJumpVelocity;
				}
			} else if (rmngAerialJumps > 0) {
				lastJumpAerial = true;
				rmngAerialJumps--;
				velocity.y = maxAerialJumpVelocity;
			}
		}

		//Detects the release of the jump button and sets the vertical velocity to its minimum
		if (Input.GetButtonUp ("Jump")) {
			if (lastJumpAerial){
				if (velocity.y > minAerialJumpVelocity) {
					velocity.y = minAerialJumpVelocity;
				}
			}
			else {
				if (velocity.y > minJumpVelocity) {
					velocity.y = minJumpVelocity;
				}
			}
		}

		//Glide
		if (Input.GetButton ("Jump") && velocity.y < 0 && !controller.collisions.below){
			isGliding = true;

			float targetSpeed = glideSpeed + (glideMaxSpeed * glideAttitude.z);

			currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, (currentSpeed < targetSpeed) ? glideAccelerate : glideDecelerate);
			Debug.Log ("speed : " + currentSpeed + " target  : " + targetSpeed);

			velocity += Vector3.LerpUnclamped(transform.forward, -transform.up, glideAttitude.z) * currentSpeed;
			animator.transform.LookAt(transform.position + velocity, transform.up + transform.right*glideAttitude.x);
			/*
			velocity *= glideDrag;
			Debug.Log("glide : " + (glideSpeed + glideMaxSpeed * glideAttitude.z) * Vector3.Lerp (transform.forward, -transform.up, glideAttitude.y));
			velocity += (glideSpeed + glideMaxSpeed * glideAttitude.y) * Vector3.Lerp (transform.forward, -transform.up, glideAttitude.y);

			float targetGlideVelocity = (glideSpeed + glideMaxSpeed * glideAttitude.z);


			velocity *= glideDrag;
			currentSpeed = Mathf.SmoothDamp (currentSpeed, targetGlideVelocity, ref speedSmoothVelocity, glideSpeedSmoothVelocity);
			velocity = velocity.normalized * currentSpeed;
			velocity = Vector3.Reflect (velocity, transform.up + glideAttitude.z * transform.forward);*/


		} else {
			isGliding = false;
			animator.transform.LookAt(transform.position + transform.forward, transform.up);
		}

		#endregion jump controls


		//Calls the controller to check if the calculated velocity will run into walls and stuff
		velocity = controller.Move ((Quaternion.AngleAxis(angle, Vector3.Cross(Vector3.up, transform.up))) * velocity * Time.deltaTime);

		#region update animator
		float keyHalf = 0.5f;
		float m_RunCycleLegOffset = 0.2f;

		animator.SetBool ("OnGround", controller.collisions.below);
		animator.SetFloat ("Forward", input.magnitude);
		animator.SetFloat ("Turn", Vector3.Dot (transform.right, input));
		animator.SetFloat ("Jump", velocity.y/5);
		float runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
		float jumpLeg = (runCycle < keyHalf ? 1 : -1) * input.magnitude;
		if (controller.collisions.below) {
			animator.SetFloat("JumpLeg", jumpLeg);
		}
		#endregion update animator
	}

}