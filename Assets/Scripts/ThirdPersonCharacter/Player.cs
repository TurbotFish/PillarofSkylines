using UnityEngine;
using Game.Player;

[RequireComponent(typeof(CharacControllerRecu))]
public class Player : MonoBehaviour {


	#region speed variables

	[Header("Speed and control variables")]
	/// <summary>
	/// The character speed.
	/// </summary>
	public float characSpeed = 2f;
	/// <summary>
	/// The speed at which the player's speed changes in the air.
	/// </summary>
	[Tooltip("The speed at which the player's speed changes in the air.")]
	public float airControl = 2.5f;
	/// <summary>
	/// Multiplies the control of the player when they come out of gliding or sliding etc.
	/// </summary>
	[Tooltip("Multiplies the control of the player when they come out of gliding or sliding etc.")]
	public float momentumCoeff = .5f;
	/// <summary>
	/// The speed at which the player's speed changes on the ground.
	/// </summary>
	[Tooltip("The speed at which the player's speed changes on the ground.")]
	public float groundControl = 6f;
	/// <summary>
	/// The speed at which the player's input impacts the slide.
	/// </summary>
	[Tooltip("The speed at which the player's input impacts the slide.")]
	public float slopeControl = 2f;
	/// <summary>
	/// The speed at which the player's speed changes when sliding.
	/// </summary>
	[Tooltip("The speed at which the player's speed changes when sliding.")]
	public float slopeStrength = .8f;
	/// <summary>
	/// The value by which the speed is multiplied when the player sprints.
	/// </summary>
	[Tooltip("The value by which the speed is multiplied when the player sprints.")]
	public float sprintCoeff = 1.5f;
	/// <summary>
	/// The max walkable slope angle.
	/// </summary>
	[Tooltip("The max walkable slope angle.")]
	public float maxSlopeAngle = 45f;
	/// <summary>
	/// How much being on slanted terrain impacts the player's speed (1 means the player will double their speed down a MaxSlopeAngle slope).
	/// </summary>
	[Tooltip("How much being on slanted terrain impacts the player's speed (1 means the player will double their speed down a MaxSlopeAngle slope).")]
	public float slopeCoeff = .1f;
	/// <summary>
	/// Variable used in the speed smooth damp.
	/// </summary>
	float speedSmoothVelocity;
	#endregion speed variables

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
	/// The time during which the avatar can still jump after falling off.
	/// </summary>
	[Tooltip("How many seconds after falling can the avatar still jump.")]
	public float canStillJumpTime = .12f;
	public float gravity;
	public float maxFallSpeed = 100f;

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

	float permissiveJumpTime;

	#endregion jump variables

	#region glide variables

	[Header("Glide")]
	/// <summary>
	/// The speed of the strafe when gliding.
	/// </summary>
	[Tooltip("The speed of the strafe when gliding.")]
	public float glideStrafeControlAngle = 5f;
	/// <summary>
	/// The max angle of the strafe when gliding.
	/// </summary>
	[Tooltip("The max angle of the strafe when gliding.")]
	public float glideStrafeMaxAngle = 5f;
	/// <summary>
	/// The speed at which the player's vertical angle changes downwards when gliding.
	/// </summary>
	[Tooltip("The speed at which the player's vertical angle changes downwards when gliding.")]
	public float glideVerticalDownAngleControl = 1f;

	/// <summary>
	/// The speed at which the player's vertical angle comes back to 0 from a downwards position.
	/// </summary>
	//[Tooltip("The speed at which the player's vertical angle comes back to 0 from a downwards position.")]
	//public float glideVerticalDownComingBack = .1f;

	/// <summary>
	/// The speed at which the player's vertical angle changes upwards when gliding.
	/// </summary>
	[Tooltip("The speed at which the player's vertical angle changes upwards when gliding.")]
	public float glideVerticalUpAngleControl = 1f;

	/// <summary>
	/// The speed at which the player's vertical angle comes back to 0 from an upwards position.
	/// </summary>
	//[Tooltip("The speed at which the player's vertical angle comes back to 0 from an upwards position.")]
	//public float glideVerticalUpComingBack = .1f;

	/// <summary>
	/// The speed at which the player's horizontal angle changes when gliding.
	/// </summary>
	[Tooltip("The speed at which the player's horizontal angle changes when gliding.")]
	public float glideHorizontalAngleControl = 1f;
	/// <summary>
	/// The speed at which the player's horizontal angle comes back to 0.
	/// </summary>
	[Tooltip("The speed at which the player's horizontal angle comes back to 0.")]
	public float glideHorizontalComingBack = 5f;
	/// <summary>
	/// How much having no input impacts the update of the vertical angle of the player.
	/// </summary>
	[Tooltip("How much having no input impacts the update of the vertical angle of the player.")]
	public float glideNoInputImpactCoeff = .2f;
	public float glideBaseAngle = 10f;
	public float glideBaseSpeed = 1f;
	public float glideStallSpeed = 5f;
	public float glideSpeedSmooth = .1f;
	public AnimationCurve glideDownwardAcceleration;
	public AnimationCurve glideUpwardDecelaration;
	public float glideMinAngle = -80f;
	public float glideMaxAngle = 80f;
	public float glideMinHorizontalAngle = -80f;
	public float glideMaxHorizontalAngle = 80f;
	[HideInInspector]
	public float glideVerticalAngle;
	float targetGlideVerticalAngle;
	float glideHorizontalAngle;
	float targetGlideHorizontalAngle;
	float glideStrafeAngle;
	float targetGlideStrafeAngle;
	#endregion glide variables

	#region dash variables

	[Header("Dash")]

	public float dashSpeed = 1f;
	public float dashRange = 5f;
	public float dashCooldown = 1f;
	public float wallMaxAngle = 20f;
	float dashTimer = 0f;
	float dashDuration = 0f;
	[HideInInspector]
	public bool isDashing;

	#endregion dash variables

	[Header("FX")]
	public ParticlesManager windParticles; 
	public ParticlesManager glideParticles; 
	public GameObject jumpParticles;
	public ParticlesManager dashParticles;


	/// <summary>
	/// The velocity calculated each frame and sent to the controller.
	/// </summary>
	Vector3 velocity = Vector3.zero;

	/// <summary>
	/// The player is sliding.
	/// </summary>
	bool isSliding;
	[Space(20)]

	public ePlayerState currentPlayerState;


	/// <summary>
	/// The rotator used to turn the player.
	/// </summary>
	public Transform rotator;

	/// <summary>
	/// The script to get info about abilities.
	/// </summary>
	public PlayerModel playerMod;

	#region private variables
	bool readingInputs = true;
	bool pressedJump = false;
	bool releasedJump = false;
	bool pressedDash = false;
	bool pressedSprint = false;
	bool pressingSprint = false;
	bool keepMomentum = false;
	Vector3 inputRaw;
	Vector3 inputToCamera;
	Vector3 inputToSlope;
	float leftTrigger;
	float rightTrigger;
	Vector3 flatVelocity;
	float currentSpeed;
	/// <summary>
	/// The controller checking if there's collisions on the way.
	/// </summary>
	CharacControllerRecu controller;
	/// <summary>
	/// The animator of the character.
	/// </summary>
	Animator animator;
	#endregion private variables

	void Start(){
		controller = GetComponent<CharacControllerRecu> ();
		animator = GetComponentInChildren<Animator> ();

		currentPlayerState = ePlayerState.inAir;

		maxJumpVelocity = maxJumpHeight;
		minJumpVelocity = minJumpHeight;
		maxAerialJumpVelocity = maxJumpVelocity * coeffAerialJumpEfficiency;
		minAerialJumpVelocity = minJumpVelocity * coeffAerialJumpEfficiency;
		permissiveJumpTime = canStillJumpTime;

		velocity = Vector3.zero;

		Game.Utilities.EventManager.OnMenuSwitchedEvent += HandleEventMenuSwitched;
	}

	void HandleEventMenuSwitched (object sender, Game.Utilities.EventManager.OnMenuSwitchedEventArgs args){
		if (args.NewUiState == Game.Player.UI.eUiState.HUD)
		{
			readingInputs = true;
		}
		else
		{
			readingInputs = false;
		}
	}

	void Update(){

		#region update timers
		dashTimer -= Time.deltaTime;
		#endregion update timers

		#region input detection

		pressedJump = false;
		releasedJump = false;
		pressedDash = false;
		pressedSprint = false;
		if (readingInputs) {
			
			inputRaw = new Vector3(Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
			inputToCamera = rotator.forward * Input.GetAxisRaw ("Vertical") + rotator.right * Input.GetAxisRaw ("Horizontal");
			inputToSlope = (Quaternion.AngleAxis(Vector3.Angle(transform.up, controller.collisions.currentGroundNormal), Vector3.Cross(transform.up, controller.collisions.currentGroundNormal))) * inputToCamera;
			inputToCamera = (Quaternion.AngleAxis (Vector3.Angle (transform.up, Vector3.up), Vector3.Cross (transform.up, Vector3.up))) * inputToCamera;
			flatVelocity = velocity;
			flatVelocity.y = 0;

			leftTrigger = Input.GetAxisRaw("Left Trigger");
			rightTrigger = Input.GetAxisRaw("Right Trigger");

			if (Input.GetButtonDown ("Jump")) {
				pressedJump = true;
			}
			if (Input.GetButtonUp ("Jump")) {
				releasedJump = true;
			}
			if (Input.GetButtonDown ("Dash")) {
				pressedDash = true;
			}
			if (Input.GetButtonDown ("Sprint")) {
				pressedSprint = true;
			}
			if (Input.GetButton ("Sprint")) {
				pressingSprint = true;
			}


		} else {
			inputRaw = Vector3.zero;
			inputToCamera = Vector3.zero;
			inputToSlope = Vector3.zero;
			leftTrigger = 0f;
			rightTrigger = 0f;
		}

		#endregion input detection


		#region direction

		Vector3 targetVelocity = Vector3.zero;
		Debug.Log("state : " + currentPlayerState + " momentum : " + keepMomentum);
		switch (currentPlayerState) {
		default:
			Debug.LogWarning ("pas de player state >:c");
			break;


		case ePlayerState.inAir:

			if (flatVelocity.magnitude < characSpeed)
				keepMomentum = false;
			
			targetVelocity = inputToCamera * characSpeed;
			velocity.y -= gravity * Time.deltaTime;
			velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, maxFallSpeed);
			flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, airControl * Time.deltaTime * (keepMomentum ? momentumCoeff : 1f));
			if (releasedJump) {
				releasedJump = false;
				if (lastJumpAerial) {
					if (velocity.y > minAerialJumpVelocity) {
						velocity.y = minAerialJumpVelocity;
					}
				} else {
					if (velocity.y > minJumpVelocity) {
						velocity.y = minJumpVelocity;
					}
				}
			}
			if (pressedDash && dashTimer <= 0f && playerMod.CheckAbilityActive(eAbilityType.Dash)) {
				StartDash();
			}
			if (pressedJump && rmngAerialJumps > 0 && playerMod.CheckAbilityActive(eAbilityType.DoubleJump)) {
				Debug.Log("aerial jump");
				velocity.y = maxAerialJumpVelocity;
				rmngAerialJumps--;
				lastJumpAerial = true;
				playerMod.FlagAbility(eAbilityType.DoubleJump);
			}
			if (pressedSprint && playerMod.CheckAbilityActive(eAbilityType.Glide)) {
				Debug.Log("glide");
				glideParticles.Play();
				glideVerticalAngle = Vector3.Angle(transform.up, velocity) - 90f;
				glideHorizontalAngle = 0f;
				currentPlayerState = ePlayerState.gliding;
				playerMod.FlagAbility(eAbilityType.Glide);
			}


			break;


		case ePlayerState.onGround:

			if (flatVelocity.magnitude < characSpeed)
				keepMomentum = false;
			
			rmngAerialJumps = numberOfAerialJumps;
			playerMod.UnflagAbility(eAbilityType.DoubleJump);

			flatVelocity = velocity;
			velocity.y = 0f;
			targetVelocity = inputToSlope * characSpeed * (pressingSprint ? sprintCoeff : 1);
			flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, groundControl * Time.deltaTime * (keepMomentum ? momentumCoeff : 1f));
			// Detects if the player is moving up or down the slope, and multiply their speed based on that slope
			flatVelocity *= 1 + slopeCoeff * Vector3.Angle (transform.forward, Vector3.ProjectOnPlane (transform.forward, controller.collisions.currentGroundNormal)) * (Vector3.Dot (transform.forward, controller.collisions.currentGroundNormal) > 0 ? 1 : -1) / (maxSlopeAngle);

			/*   OTHER VERSION 
			flatVelocity = (Quaternion.AngleAxis(Vector3.Angle(transform.up, controller.collisions.currentGroundNormal), Vector3.Cross(controller.collisions.currentGroundNormal, transform.up))) * velocity;
			velocity.y = 0f;
			targetVelocity = inputToCamera * characSpeed;
			flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, groundControl * Time.deltaTime);
			flatVelocity = (Quaternion.AngleAxis(Vector3.Angle(transform.up, controller.collisions.currentGroundNormal), Vector3.Cross(transform.up, controller.collisions.currentGroundNormal))) * flatVelocity;
			flatVelocity *= Vector3.Angle (transform.forward, Vector3.ProjectOnPlane (transform.forward, controller.collisions.currentGroundNormal)) * (Vector3.Dot (transform.forward, controller.collisions.currentGroundNormal) > 0 ? 1 : -1) / (90 * 5) + 1;
*/

			if (pressedJump) {
				pressedJump = false;
				velocity.y = maxJumpVelocity;
				currentPlayerState = ePlayerState.inAir;
				lastJumpAerial = false;
			}
			if (pressedDash && dashTimer <= 0f && playerMod.CheckAbilityActive(eAbilityType.Dash)) {
				StartDash();
			}
			break;


		case ePlayerState.dashing:
			flatVelocity = transform.forward * ((dashRange/dashSpeed)/Time.deltaTime);
			flatVelocity.y = 0f;
			velocity.y = 0f;
			dashDuration -= Time.deltaTime;

			if (dashDuration <= 0) {
				EndDash();
			}
			break;


		case ePlayerState.gliding:

			targetGlideVerticalAngle = Mathf.Clamp(Mathf.Lerp(glideMinAngle, glideMaxAngle, (inputRaw.z/2) +.5f) + glideBaseAngle, glideMinAngle, glideMaxAngle);
			glideVerticalAngle = Mathf.Lerp(glideVerticalAngle, targetGlideVerticalAngle, (targetGlideVerticalAngle > glideVerticalAngle ? glideVerticalUpAngleControl : glideVerticalDownAngleControl) * Time.deltaTime * (targetGlideVerticalAngle == glideBaseAngle ? glideNoInputImpactCoeff : 1f));

			if (glideVerticalAngle < glideBaseAngle){
				Debug.Log("on retire " + glideUpwardDecelaration.Evaluate(Mathf.Abs((glideVerticalAngle - glideBaseAngle)/(glideMinAngle - glideBaseAngle))) * Time.deltaTime);
				currentSpeed = velocity.magnitude - glideUpwardDecelaration.Evaluate(Mathf.Abs((glideVerticalAngle - glideBaseAngle)/(glideMinAngle - glideBaseAngle))) * Time.deltaTime;
			} else {
				Debug.Log("on lerp de " + velocity.magnitude + " vers " + (glideBaseSpeed + glideDownwardAcceleration.Evaluate((glideVerticalAngle - glideBaseAngle)/(glideMaxAngle - glideBaseAngle))));
				currentSpeed = Mathf.Lerp(velocity.magnitude, glideBaseSpeed + glideDownwardAcceleration.Evaluate((glideVerticalAngle - glideBaseAngle)/(glideMaxAngle - glideBaseAngle)), glideSpeedSmooth * Time.deltaTime);
			}
			Debug.Log("speed : " + currentSpeed + " vertical angle : " + glideVerticalAngle);
			targetVelocity = Quaternion.AngleAxis(glideVerticalAngle, transform.right) * transform.forward * currentSpeed;

			if (currentSpeed < glideStallSpeed){
				glideVerticalAngle = glideMaxAngle;
			}

			targetGlideHorizontalAngle = Mathf.Lerp(glideMinHorizontalAngle, glideMaxHorizontalAngle, (inputRaw.x/2) +.5f);
			glideHorizontalAngle = Mathf.Lerp (glideHorizontalAngle, targetGlideHorizontalAngle, (Mathf.Abs(glideHorizontalAngle) > Mathf.Abs(targetGlideHorizontalAngle) ? glideHorizontalComingBack : glideHorizontalAngleControl) * Time.deltaTime);

			targetVelocity = Quaternion.AngleAxis(glideHorizontalAngle, transform.up) * targetVelocity;

			targetGlideStrafeAngle = (rightTrigger - leftTrigger) * glideStrafeMaxAngle;

			glideStrafeAngle = Mathf.Lerp(glideStrafeAngle, targetGlideStrafeAngle, glideStrafeControlAngle * Time.deltaTime);

			targetVelocity = Quaternion.AngleAxis(glideStrafeAngle, transform.up) * targetVelocity;

			velocity.y = 0f;
			flatVelocity = targetVelocity;

			if (pressedSprint) {
				currentPlayerState = ePlayerState.inAir;
				EndGlide();
			}
			break;


		case ePlayerState.sliding:
			float speed = Mathf.Lerp(gravity, 0, Vector3.Dot(transform.up, controller.collisions.currentGroundNormal));
			targetVelocity = Quaternion.AngleAxis(90f, Vector3.Cross(transform.up, controller.collisions.currentGroundNormal)) * controller.collisions.currentGroundNormal * speed;
			targetVelocity += inputToSlope;
			flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, slopeControl * Time.deltaTime * (keepMomentum ? momentumCoeff : 1f));

			if (pressedJump) {
				pressedJump = false;
				velocity.y = maxJumpVelocity;
				currentPlayerState = ePlayerState.inAir;
			}
			break;
		}

		velocity = new Vector3(0, velocity.y, 0);
		velocity += flatVelocity;
		
		#endregion direction

		//Calls the controller to check if the calculated velocity will run into walls and stuff
		velocity = controller.Move ((Quaternion.AngleAxis(Vector3.Angle(Vector3.up, transform.up), Vector3.Cross(Vector3.up, transform.up))) * velocity * Time.deltaTime);

		if (currentPlayerState != ePlayerState.gliding) {
			transform.LookAt (transform.position + Vector3.ProjectOnPlane ((Quaternion.AngleAxis (Vector3.Angle (Vector3.up, transform.up), Vector3.Cross (Vector3.up, transform.up))) * velocity, transform.up), transform.up);
		} else {
			transform.Rotate (transform.up, glideHorizontalAngle);
		}

		#region update state

		switch (currentPlayerState) {
		default:
			currentPlayerState = ePlayerState.inAir;
			break;


		case ePlayerState.inAir:
			if (controller.collisions.below) {
				if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) > maxSlopeAngle){
					currentPlayerState = ePlayerState.sliding;
				} else {
					currentPlayerState = ePlayerState.onGround;
				}
			}
			break;


		case ePlayerState.onGround:
			if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) > maxSlopeAngle){
				currentPlayerState = ePlayerState.sliding;
			}
			if (!controller.collisions.below) {
				currentPlayerState = ePlayerState.inAir;
			}
			break;


		case ePlayerState.dashing:
			break;


		case ePlayerState.gliding:
			if (controller.collisions.below) {
				if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) > maxSlopeAngle){
					currentPlayerState = ePlayerState.sliding;
					EndGlide();
				} else {
					currentPlayerState = ePlayerState.onGround;
					EndGlide();
				}
			}
			break;


		case ePlayerState.sliding:
			if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < maxSlopeAngle){
				currentPlayerState = ePlayerState.onGround;
				keepMomentum = true;
			}
			if (!controller.collisions.below) {
				currentPlayerState = ePlayerState.inAir;
				keepMomentum = true;
			}
			break;
		}
		/*
		if (currentPlayerState != ePlayerState.dashing) {
			if (controller.collisions.below) {
				if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) > maxSlopeAngle){
					currentPlayerState = Player.ePlayerState.sliding;
				} else {
					currentPlayerState = Player.ePlayerState.onGround;
				}
			} else {
				currentPlayerState = Player.ePlayerState.inAir;
			}
		}*/
		#endregion update state

		#region update animator
		float keyHalf = 0.5f;
		float m_RunCycleLegOffset = 0.2f;

		animator.SetBool ("OnGround", controller.collisions.below);
		animator.SetFloat ("Forward", inputRaw.magnitude);
		//animator.SetFloat ("Turn", Vector3.Dot (transform.right, inputRaw));
		animator.SetFloat ("Jump", velocity.y/5);
		float runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
		float jumpLeg = (runCycle < keyHalf ? 1 : -1) * inputRaw.magnitude;
		if (controller.collisions.below) {
			animator.SetFloat("JumpLeg", jumpLeg);
		}
		/*if (currentPlayerState == ePlayerState.gliding) {
			animator.transform.rotation = Quaternion.Euler(glideVerticalAngle, 0, 0);
		}*/

		windParticles.SetVelocity(velocity);
		glideParticles.SetVelocity(velocity);
		#endregion update animator

	}

	void StartDash(){
		playerMod.FlagAbility (eAbilityType.Dash);
		currentPlayerState = ePlayerState.dashing;
		dashDuration = dashSpeed;
		dashParticles.Play ();
	}

	void EndDash(){
		currentPlayerState = ePlayerState.inAir;
		dashDuration = 0f;
		dashTimer = dashCooldown;
		playerMod.UnflagAbility (eAbilityType.Dash);
	}

	void EndGlide(){
		glideParticles.Stop();
		keepMomentum = true;
	}



}

public enum ePlayerState {
	onGround,
	inAir,
	gliding,
	dashing,
	sliding
}