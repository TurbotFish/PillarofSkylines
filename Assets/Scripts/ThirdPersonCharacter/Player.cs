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
	/// <summary>
	/// Calculated at the start from maxJumpHeight and timeToJumpApex.
	/// </summary>
	public float gravity;

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
	/// How much does gliding slow down the speed.
	/// </summary>
	[Tooltip("How much does gliding slow down the speed.")]
	public float glideDrag = .2f;
	/// <summary>
	/// The speed at which the player's angle changes when gliding.
	/// </summary>
	[Tooltip("The speed at which the player's angle changes when gliding.")]
	public float glideControl = .2f;
	public float glideBaseAngle = 10f;
	public float glideBaseSpeed = 2f;
	public float glideSpeedSmooth = .1f;
	public AnimationCurve glideDownwardAcceleration;
	public AnimationCurve glideUpwardDecelaration;
	public float glideMinAngle;
	public float glideMaxAngle;
	float glideAngle;
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
	Vector3 inputRaw;
	Vector3 inputToCamera;
	Vector3 inputToSlope;
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
		}

		#endregion input detection


		#region direction

		Vector3 targetVelocity = Vector3.zero;
		Debug.Log("state : " + currentPlayerState);
		switch (currentPlayerState) {
		default:
			Debug.LogWarning ("pas de player state >:c");
			break;


		case ePlayerState.inAir:
			targetVelocity = inputToCamera * characSpeed;
			velocity.y -= gravity * Time.deltaTime;
			flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, airControl * Time.deltaTime);
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
			if (pressedDash && dashTimer <= 0f) {
				Debug.Log("dash");
				currentPlayerState = ePlayerState.dashing;
			}
			if (pressedJump && rmngAerialJumps > 0) {
				Debug.Log("aerial jump");
				velocity.y = maxAerialJumpVelocity;
				rmngAerialJumps--;
				lastJumpAerial = true;
			}
			if (pressedSprint) {
				Debug.Log("glide");
				glideAngle = glideBaseAngle;
				currentPlayerState = ePlayerState.gliding;
			}
			break;


		case ePlayerState.onGround:

			rmngAerialJumps = numberOfAerialJumps;

			flatVelocity = velocity;
			velocity.y = 0f;
			targetVelocity = inputToSlope * characSpeed * (pressingSprint ? sprintCoeff : 1);
			flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, groundControl * Time.deltaTime);
			flatVelocity *= Vector3.Angle (transform.forward, Vector3.ProjectOnPlane (transform.forward, controller.collisions.currentGroundNormal)) * (Vector3.Dot (transform.forward, controller.collisions.currentGroundNormal) > 0 ? 1 : -1) / (90 * 5) + 1;

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
			if (pressedDash && dashTimer <= 0f) {
				Debug.Log("dash");
				currentPlayerState = ePlayerState.dashing;
			}
			break;


		case ePlayerState.dashing:
			flatVelocity = transform.forward * ((dashRange/dashSpeed)/Time.deltaTime);
			flatVelocity.y = 0f;
			velocity.y = 0f;
			dashDuration -= Time.deltaTime;

			if (dashDuration <= 0) {
				currentPlayerState = ePlayerState.inAir;
				dashTimer = dashCooldown;
				playerMod.UnflagAbility (eAbilityType.Dash);
			}
			break;


		case ePlayerState.gliding:

			glideAngle = Mathf.Lerp(glideAngle, Mathf.Clamp(Mathf.Lerp(glideMinAngle, glideMaxAngle, (inputRaw.z/2) +.5f) + glideBaseAngle, glideMinAngle, glideMaxAngle), glideControl * Time.deltaTime);

			if (glideAngle < glideBaseAngle){
				currentSpeed = velocity.magnitude - glideUpwardDecelaration.Evaluate(Mathf.Abs((glideAngle - glideBaseAngle)/(glideMinAngle - glideBaseAngle))) * Time.deltaTime;
			} else {
				currentSpeed = Mathf.Lerp(velocity.magnitude, glideBaseSpeed + glideDownwardAcceleration.Evaluate((glideAngle - glideBaseAngle)/(glideMaxAngle - glideBaseAngle)), glideSpeedSmooth * Time.deltaTime);
			}
			Debug.Log("speed : " + currentSpeed + " angle : " + glideAngle);
			targetVelocity = Quaternion.AngleAxis(glideAngle, transform.right) * transform.forward * currentSpeed;

			velocity.y = 0f;
			flatVelocity = targetVelocity;

			break;


		case ePlayerState.sliding:
			float speed = Mathf.Lerp(gravity, 0, Vector3.Dot(transform.up, controller.collisions.currentGroundNormal));
			targetVelocity = Quaternion.AngleAxis(90f, Vector3.Cross(transform.up, controller.collisions.currentGroundNormal)) * controller.collisions.currentGroundNormal * speed;
			targetVelocity += inputToSlope;
			flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, slopeControl * Time.deltaTime);

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
		velocity = controller.Move ((Quaternion.AngleAxis(Vector3.Angle(Vector3.up, transform.up), Vector3.Cross(Vector3.up, transform.up))) * velocity / 10f);

		transform.LookAt (transform.position + Vector3.ProjectOnPlane((Quaternion.AngleAxis(Vector3.Angle(Vector3.up, transform.up), Vector3.Cross(Vector3.up, transform.up))) * velocity, transform.up), transform.up);


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
			break;


		case ePlayerState.sliding:
			if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < maxSlopeAngle){
				currentPlayerState = ePlayerState.onGround;
			}
			if (!controller.collisions.below) {
				currentPlayerState = ePlayerState.inAir;
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
		animator.SetFloat ("Turn", Vector3.Dot (transform.right, inputRaw));
		animator.SetFloat ("Jump", velocity.y/5);
		float runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
		float jumpLeg = (runCycle < keyHalf ? 1 : -1) * inputRaw.magnitude;
		if (controller.collisions.below) {
			animator.SetFloat("JumpLeg", jumpLeg);
		}

		windParticles.SetVelocity(velocity);
		glideParticles.SetVelocity(velocity);
		#endregion update animator

	}

	public void CancelDash(){
		dashDuration = 0f;
		isDashing = false;
		dashTimer = dashCooldown;
		playerMod.UnflagAbility (eAbilityType.Dash);
	}


	public enum ePlayerState {
		onGround,
		inAir,
		gliding,
		dashing,
		sliding
	}
}