//using UnityEngine;
//using Game.Player;

//[RequireComponent(typeof(CharacControllerRecu))]
//public class Player : MonoBehaviour {


//	#region general
//	[Header("General")]
//	/// <summary>
//	/// The strength of the gravity.
//	/// </summary>
//	[Tooltip("The strength of the grabity (duh)")]
//	public float gravityStrength;
//	/// <summary>
//	/// The maximum falling speed of the player.
//	/// </summary>
//	[Tooltip("The maximum speed the player can go vertically. (up or down)")]
//	public float maxFallSpeed = 100f;
//	/// <summary>
//	/// The angle at which the player starts having trouble climbing up.
//	/// </summary>
//	[Tooltip("The angle at which the player starts having trouble climbing up.")]
//	public float minSlopeAngle = 30f;
//	/// <summary>
//	/// The max walkable slope angle.
//	/// </summary>
//	[Tooltip("The max walkable slope angle.")]
//	public float maxSlopeAngle = 45f;
//	/// <summary>
//	/// The angle at which the ground is considered a wall.
//	/// </summary>
//	[Tooltip("The angle at which the ground is considered a wall.")]
//	public float minWallAngle = 75f;
//	/// <summary>
//	/// The speed at which the player model turns around (only visual, no gameplay incidence).
//	/// </summary>
//	[Tooltip("The speed at which the player model turns around (only visual, no gameplay incidence).")]
//	public float playerModelTurnSpeed = 4f;
//	/// <summary>
//	/// The speed at which the player model turns around in the air (only visual, no gameplay incidence).
//	/// </summary>
//	[Tooltip("The speed at which the player model turns around in the air (only visual, no gameplay incidence).")]
//	public float playerModelAerialTurnSpeed = 4f;
//	/// <summary>
//	/// The strength of the camera offset when the player lands on ground.
//	/// </summary>
//	[Tooltip("The strength of the camera offset when the player lands on ground.")]
//	public float landingCameraOffsetStrength = .5f;
//	/// <summary>
//	/// The maximum height of step the player can climb.
//	/// </summary>
//	[Tooltip("The maximum height of step the player can climb.")]
//	public float maxStepHeight = 1f;
//	#endregion general

//	#region speed and controls variables
//	[Header("Speed and control variables")]
//	/// <summary>
//	/// The character speed.
//	/// </summary>
//	public float characSpeed = 2f;
//	/// <summary>
//	/// The speed at which the player's speed changes in the air.
//	/// </summary>
//	[Tooltip("The speed at which the player's speed changes in the air.")]
//	public float airControl = 2.5f;
//	/// <summary>
//	/// Multiplies the control of the player when they come out of gliding or sliding etc.
//	/// </summary>
//	[Tooltip("Multiplies the control of the player when they come out of gliding or sliding etc.")]
//	public float momentumCoeff = .5f;
//	/// <summary>
//	/// The speed at which the player's speed changes on the ground.
//	/// </summary>
//	[Tooltip("The speed at which the player's speed changes on the ground.")]
//	public float groundControl = 6f;
//	/// <summary>
//	/// The value by which the speed update of the player is multiplied when there's no input on the left stick.
//	/// </summary>
//	[Tooltip("The value by which the speed update of the player is multiplied when there's no input on the left stick.")]
//	public float groundNoInputCoeff = 5f;
//	/// <summary>
//	/// The speed at which the player's input impacts the slide.
//	/// </summary>
//	[Tooltip("The speed at which the player's input impacts the slide.")]
//	public float slopeControl = 2f;
//	/// <summary>
//	/// The speed at which the player's speed changes when sliding.
//	/// </summary>
//	[Tooltip("The speed at which the player's speed changes when sliding.")]
//	public float slopeStrength = .8f;
//	/// <summary>
//	/// The value by which the speed is multiplied when the player sprints.
//	/// </summary>
//	[Tooltip("The value by which the speed is multiplied when the player sprints.")]
//	public float sprintCoeff = 1.5f;
//	/// <summary>
//	/// How much being on slanted terrain impacts the player's speed (1 means the player will double their speed down a MaxSlopeAngle slope).
//	/// </summary>
//	[Tooltip("How much being on slanted terrain impacts the player's speed (1 means the player will double their speed down a MaxSlopeAngle slope).")]
//	public float slopeCoeff = .1f;
//	#endregion speed and controls variables

//	#region jump variables
//	[Header("Jump parameters")]
//	/// <summary>
//	/// The minimum height of the jump.
//	/// </summary>
//	[Tooltip("The minimum height of the jump.")]
//	public float minJumpHeight = 3f;
//	/// <summary>
//	/// The maximum height of the jump.
//	/// </summary>
//	[Tooltip("The maximum height of the jump.")]
//	public float maxJumpHeight = 6f;
//	/// <summary>
//	/// The time during which the avatar can still jump after falling off.
//	/// </summary>
//	[Tooltip("How many seconds after falling can the avatar still jump.")]
//	public float canStillJumpTime = .12f;

//	[Header("Aerial Jumps")]
//	/// <summary>
//	/// The number of jumps the player can do while in the air.
//	/// </summary>
//	[Tooltip("The number of jumps the player can do while in the air.")]
//	public int numberOfAerialJumps = 0;
//	/// <summary>
//	/// The efficiency of the aerial jump compared to the regular jump (2 makes it 2 times stronger, 0.5 makes it 2 times weaker (kinda)).
//	/// </summary>
//	[Tooltip("The efficiency of the aerial jump compared to the regular jump (2 makes it 2 times stronger, 0.5 makes it 2 times weaker (kinda)).")]
//	public float coeffAerialJumpEfficiency = 1f;

//	/// <summary>
//	/// The current number of aerial jumps remaining to the player.
//	/// </summary>
//	int rmngAerialJumps;
//	/// <summary>
//	/// The maximum jump velocity calculated at the start.
//	/// </summary>
//	float maxJumpVelocity;
//	/// <summary>
//	/// The minimum jump velocity calculated at the start.
//	/// </summary>
//	float minJumpVelocity;
//	/// <summary>
//	/// The maximum aerial jump velocity calculated at the start.
//	/// </summary>
//	float maxAerialJumpVelocity;
//	/// <summary>
//	/// The minimum aerial jump velocity calculated at the start.
//	/// </summary>
//	float minAerialJumpVelocity;
//	/// <summary>
//	/// States if the last jump is an aerial one or a regular one.
//	/// </summary>
//	bool lastJumpAerial = false;

//	float permissiveJumpTime;

//	#endregion jump variables

//	#region glide variables

//	[Header("Glide")]
//	/// <summary>
//	/// The speed of the strafe when gliding.
//	/// </summary>
//	[Tooltip("The speed of the strafe when gliding.")]
//	public float glideStrafeControlAngle = 5f;
//	/// <summary>
//	/// The max angle of the strafe when gliding.
//	/// </summary>
//	[Tooltip("The max angle of the strafe when gliding.")]
//	public float glideStrafeMaxAngle = 5f;
//	/// <summary>
//	/// The speed at which the player's vertical angle changes downwards when gliding.
//	/// </summary>
//	[Tooltip("The speed at which the player's vertical angle changes downwards when gliding.")]
//	public float glideVerticalDownAngleControl = 1f;


//	/// <summary>
//	/// The speed at which the player's vertical angle changes upwards when gliding.
//	/// </summary>
//	[Tooltip("The speed at which the player's vertical angle changes upwards when gliding.")]
//	public float glideVerticalUpAngleControl = 1f;


//	/// <summary>
//	/// The speed at which the player's horizontal angle changes when gliding.
//	/// </summary>
//	[Tooltip("The speed at which the player's horizontal angle changes when gliding.")]
//	public float glideHorizontalAngleControl = 1f;
//	/// <summary>
//	/// The speed at which the player's horizontal angle comes back to 0.
//	/// </summary>
//	[Tooltip("The speed at which the player's horizontal angle comes back to 0.")]
//	public float glideHorizontalComingBack = 5f;
//	/// <summary>
//	/// How much having no input impacts the update of the vertical angle of the player.
//	/// </summary>
//	[Tooltip("How much having no input impacts the update of the vertical angle of the player.")]
//	public float glideNoInputImpactCoeff = .2f;
//	/// <summary>
//	/// The target angle of the glide when the player has no input.
//	/// </summary>
//	public float glideBaseAngle = 10f;
//	/// <summary>
//	/// The target speed at which the player will naturally come back to when gliding at glideBaseAngle.
//	/// </summary>
//	public float glideBaseSpeed = 20f;
//	/// <summary>
//	/// The speed under which the player will stall.
//	/// </summary>
//	public float glideStallSpeed = 5f;
//	/// <summary>
//	/// The speed at which the player accelerates
//	/// </summary>
//	public float glideSpeedSmooth = .1f;
//	/// <summary>
//	/// The speed added to the target speed depending on the player's angle when gliding downwards.
//	/// </summary>
//	public AnimationCurve glideDownwardAcceleration;
//	/// <summary>
//	/// The speed removed from the player's when gliding upwards.
//	/// </summary>
//	public AnimationCurve glideUpwardDecelaration;
//	public float glideMinAngle = -80f;
//	public float glideMaxAngle = 80f;
//	public float glideMinHorizontalAngle = -80f;
//	public float glideMaxHorizontalAngle = 80f;
//	[HideInInspector]
//	public float glideVerticalAngle;
//	float targetGlideVerticalAngle;
//	float glideHorizontalAngle;
//	float targetGlideHorizontalAngle;
//	float glideStrafeAngle;
//	float targetGlideStrafeAngle;
//	#endregion glide variables

//	#region dash variables

//	[Header("Dash")]

//	public float dashSpeed = 1f;
//	public float dashRange = 5f;
//	public float dashCooldown = 1f;
//	float dashTimer = 0f;
//	float dashDuration = 0f;

//	#endregion dash variables

//	#region joli variables
//	[Header("FX")]
//	public ParticlesManager windParticles; 
//	public ParticlesManager glideParticles; 
//	public GameObject jumpParticles;
//	public ParticlesManager dashParticles;
//	#endregion joli variables

//	#region other variables
//	[Space(20)]
//	/// <summary>
//	/// The rotator used to turn the camera.
//	/// </summary>
//	public Transform rotator;

//	/// <summary>
//	/// The script to get info about abilities.
//	/// </summary>
//	PlayerModel playerMod;

//	/// <summary>
//	/// The controller checking if there's collisions on the way.
//	/// </summary>
//	CharacControllerRecu controller;
//	/// <summary>
//	/// The animator of the character.
//	/// </summary>
//	Animator animator;

//	new PoS_Camera camera;

//	//[HideInInspector]
//	public ePlayerState currentPlayerState;

//	/// <summary>
//	/// The velocity calculated each frame and sent to the controller.
//	/// </summary>
//	[HideInInspector]
//	public Vector3 velocity = Vector3.zero;
//	Vector3 externalVelocity = Vector3.zero;

//	public Vector3 gravity = -Vector3.up;

//	#endregion other variables

//	#region private variables
//	bool readingInputs = true;
//	bool pressedJump = false;
//	bool releasedJump = false;
//	bool pressedDash = false;
//	bool pressedSprint = false;
//	bool pressingSprint = false;
//	float leftTrigger;
//	float rightTrigger;

//	bool leftStickAtZero = false;

//	bool keepMomentum = false;
//	bool inWindTunnel = false;

//	Vector3 inputRaw;
//	Vector3 inputToCamera;
//	Vector3 inputToSlope;

//	Vector3 flatVelocity;
//	Vector3 turnedVelocity;
//	Vector3 windVelocity;

//	float currentSpeed;

//    bool isInitialized = false;
//	#endregion private variables


//	void Start(){
//		controller = GetComponent<CharacControllerRecu> ();
//		animator = GetComponentInChildren<Animator> ();
//		camera = FindObjectOfType<PoS_Camera>();

//		currentPlayerState = ePlayerState.inAir;

//		maxJumpVelocity = maxJumpHeight;
//		minJumpVelocity = minJumpHeight;
//		maxAerialJumpVelocity = maxJumpVelocity * coeffAerialJumpEfficiency;
//		minAerialJumpVelocity = minJumpVelocity * coeffAerialJumpEfficiency;
//		permissiveJumpTime = canStillJumpTime;

//		gravity = gravity.normalized;

//		velocity = Vector3.zero;

//		Game.Utilities.EventManager.OnMenuSwitchedEvent += HandleEventMenuSwitched;
//		Game.Utilities.EventManager.TeleportPlayerEvent += HandleEventTeleportPlayer;
//	}

//	void HandleEventMenuSwitched (object sender, Game.Utilities.EventManager.OnMenuSwitchedEventArgs args){
//		if (args.NewUiState == Game.UI.eUiState.HUD)
//		{
//			readingInputs = true;
//		}
//		else
//		{
//			readingInputs = false;
//		}
//	}

//	void HandleEventTeleportPlayer (object sender, Game.Utilities.EventManager.OnTeleportPlayerEventArgs args){
//		transform.position = args.Position;

//        if (args.IsNewScene)
//        {
//            velocity = Vector3.zero;
//            currentPlayerState = ePlayerState.inAir;
//            ChangeGravityDirection(Vector3.down);
//        }
//	}


//	void Update(){
//        if (!this.isInitialized)
//        {
//            return;
//        }

//		#region update timers
//		dashTimer -= Time.deltaTime;
//		#endregion update timers

//		#region turn the player
//		//Turn the player in the direction of his input
//		if (inputRaw.magnitude > 0f && currentPlayerState != ePlayerState.dashing) {
//			if (currentPlayerState != ePlayerState.gliding) {
//				transform.Rotate (transform.up, Mathf.Lerp (0f, Vector3.SignedAngle (transform.forward, Vector3.ProjectOnPlane (TurnLocalToSpace(inputToCamera), transform.up), transform.up), 
//					(currentPlayerState == ePlayerState.gliding ? playerModelAerialTurnSpeed : playerModelTurnSpeed) * Time.deltaTime), Space.World);
//			} else {
//				transform.Rotate (transform.up, glideHorizontalAngle, Space.World);
//			}
//		}
//		#endregion turn the player

//		#region input detection

//		leftStickAtZero = false;
//		pressedJump = false;
//		releasedJump = false;
//		pressedDash = false;
//		pressedSprint = false;
//		pressingSprint = false;
//		if (readingInputs) {
			
//			inputRaw = new Vector3(Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
//			if (inputRaw.magnitude < .1f){
//				leftStickAtZero = true;
//			}
//			inputToCamera = rotator.forward * Input.GetAxisRaw ("Vertical") + rotator.right * Input.GetAxisRaw ("Horizontal");
//			inputToCamera = (Quaternion.AngleAxis (Vector3.Angle (transform.up, Vector3.up), Vector3.Cross (transform.up, Vector3.up))) * inputToCamera;
//			inputToSlope = (Quaternion.AngleAxis(Vector3.Angle(transform.up, controller.collisions.currentGroundNormal), Vector3.Cross(transform.up, controller.collisions.currentGroundNormal))) * inputToCamera;
////			Debug.Log("Inputs = raw : " + inputRaw + " to camera = : " + inputToCamera + " to slope = " + inputToSlope + " slope " + Vector3.Angle(transform.up, controller.collisions.currentGroundNormal));

//			leftTrigger = Input.GetAxisRaw("Left Trigger");
//			rightTrigger = Input.GetAxisRaw("Right Trigger");

//			if (Input.GetButtonDown ("Jump")) {
//				pressedJump = true;
//			}
//			if (Input.GetButtonUp ("Jump")) {
//				releasedJump = true;
//			}
//			if (Input.GetButtonDown ("Dash")) {
//				pressedDash = true;
//			}
//			if (Input.GetButtonDown ("Sprint")) {
//				pressedSprint = true;
//			}
//			if (Input.GetButton ("Sprint")) {
//				pressingSprint = true;
//			}


//		} else {
//			inputRaw = Vector3.zero;
//			inputToCamera = Vector3.zero;
//			inputToSlope = Vector3.zero;
//			leftTrigger = 0f;
//			rightTrigger = 0f;
//		}
//		#endregion input detection

//		#region direction calculations

//		flatVelocity = velocity;
//		flatVelocity.y = 0;

//		Vector3 targetVelocity = Vector3.zero;
//		//		Debug.Log("state : " + currentPlayerState + " momentum : " + keepMomentum);
//		switch (currentPlayerState) {
//			default:
//				Debug.LogWarning ("pas de player state >:c");
//				break;


//			#region in air
//			case ePlayerState.inAir:
				
//				permissiveJumpTime -= Time.deltaTime;

//				if (flatVelocity.magnitude < characSpeed)
//					keepMomentum = false;
				
//				targetVelocity = inputToCamera * characSpeed;
//				velocity.y -= gravityStrength * Time.deltaTime;
//				velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, maxFallSpeed);
//				flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, airControl * Time.deltaTime * (keepMomentum ? momentumCoeff : 1f));
//				if (releasedJump) {
//					releasedJump = false;
//					if (lastJumpAerial) {
//						if (velocity.y > minAerialJumpVelocity) {
//							velocity.y = minAerialJumpVelocity;
//						}
//					} else {
//						if (velocity.y > minJumpVelocity) {
//							velocity.y = minJumpVelocity;
//						}
//					}
//				}
//				if (pressedDash && dashTimer <= 0f && playerMod.CheckAbilityActive(eAbilityType.Dash)) {
//					StartDash();
//				}
//				if (pressedJump) {
//					if (permissiveJumpTime > 0f) {
//						pressedJump = false;
//						velocity.y = maxJumpVelocity;
//						currentPlayerState = ePlayerState.inAir;
//						lastJumpAerial = false;
//					}else if (rmngAerialJumps > 0 && playerMod.CheckAbilityActive(eAbilityType.DoubleJump)) {
//						velocity.y = maxAerialJumpVelocity;
//						rmngAerialJumps--;
//						lastJumpAerial = true;
//						playerMod.FlagAbility(eAbilityType.DoubleJump);
//						Instantiate (jumpParticles, transform.position, Quaternion.identity, transform);
//					}
//				}
//				if (pressedSprint && playerMod.CheckAbilityActive(eAbilityType.Glide)) {
//					glideParticles.Play();
//					glideVerticalAngle = Vector3.Angle(transform.up, TurnLocalToSpace(velocity)) - 90f;
//					glideHorizontalAngle = 0f;
//					currentPlayerState = ePlayerState.gliding;
//					playerMod.FlagAbility(eAbilityType.Glide);
//				}


//				break;
//				#endregion in air


//				#region on ground
//			case ePlayerState.onGround:
				
//				permissiveJumpTime = canStillJumpTime;


//				if (flatVelocity.magnitude < characSpeed)
//					keepMomentum = false;
				
//				rmngAerialJumps = numberOfAerialJumps;
//				playerMod.UnflagAbility(eAbilityType.DoubleJump);


//				flatVelocity = velocity;
//				velocity.y = 0f;
//				targetVelocity = inputToSlope * characSpeed * (pressingSprint ? sprintCoeff : 1);
//				flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, groundControl * Time.deltaTime * (keepMomentum ? momentumCoeff : 1f) * (leftStickAtZero ? groundNoInputCoeff : 1f));
//				// Detects if the player is moving up or down the slope, and multiply their speed based on that slope
//				if (Vector3.Angle(transform.up, controller.collisions.currentGroundNormal) > minSlopeAngle) {
////					print("Current velocity : " + flatVelocity);
//					flatVelocity *= .99f + slopeCoeff * Vector3.Angle (transform.forward, Vector3.ProjectOnPlane (transform.forward, controller.collisions.currentGroundNormal)) * (Vector3.Dot (transform.forward, controller.collisions.currentGroundNormal) > 0 ? 1 : -1) / (maxSlopeAngle);
////					print("New velocity : " + flatVelocity + " modified by : " + (slopeCoeff * Vector3.Angle (transform.forward, Vector3.ProjectOnPlane (transform.forward, controller.collisions.currentGroundNormal)) * (Vector3.Dot (transform.forward, controller.collisions.currentGroundNormal) > 0 ? 1 : -1) / (maxSlopeAngle)));
//				}

//				if (pressedJump) {
//					pressedJump = false;
//					velocity.y = 0f;
//					flatVelocity +=  maxJumpVelocity * Vector3.up + flatVelocity/10f;
//					currentPlayerState = ePlayerState.inAir;
//					lastJumpAerial = false;
//				}
//				if (pressedDash && dashTimer <= 0f && playerMod.CheckAbilityActive(eAbilityType.Dash)) {
//					StartDash();
//				}
//				break;
//				#endregion on ground


//				#region dashing
//			case ePlayerState.dashing:
//				flatVelocity = transform.forward * ((dashRange/dashSpeed)/Time.deltaTime);
//				flatVelocity = TurnSpaceToLocal(flatVelocity);
//				flatVelocity.y = 0f;
//				velocity.y = 0f;
//				dashDuration -= Time.deltaTime;

//				if (dashDuration <= 0) {
//					EndDash();
//				}
//				break;
//				#endregion dashing


//				#region gliding
//			case ePlayerState.gliding:
				
//				//Attention : la vélocité glide est calculé dans le world space et non dans le local space !

//				//Turn the vertical input of the player into an angle between glideMinAngle and glideMaxAngle
//				targetGlideVerticalAngle = Mathf.Clamp(Mathf.Lerp(glideMinAngle, glideMaxAngle, (inputRaw.z/2) +.5f) + glideBaseAngle, glideMinAngle, glideMaxAngle);
//				//Update the current vertical angle of the player depending on the angle calculated above
//				glideVerticalAngle = Mathf.Lerp(glideVerticalAngle, targetGlideVerticalAngle, (targetGlideVerticalAngle > glideVerticalAngle ? glideVerticalUpAngleControl : glideVerticalDownAngleControl) * Time.deltaTime * (targetGlideVerticalAngle == glideBaseAngle ? glideNoInputImpactCoeff : 1f));

//				//Update the speed of the player
//				if (glideVerticalAngle < glideBaseAngle){
//					currentSpeed = velocity.magnitude - glideUpwardDecelaration.Evaluate(Mathf.Abs((glideVerticalAngle - glideBaseAngle)/(glideMinAngle - glideBaseAngle))) * Time.deltaTime;
//				} else {
//					currentSpeed = Mathf.Lerp(velocity.magnitude, glideBaseSpeed + glideDownwardAcceleration.Evaluate((glideVerticalAngle - glideBaseAngle)/(glideMaxAngle - glideBaseAngle)), glideSpeedSmooth * Time.deltaTime);
//				}

//				//Calculate the velocity of the player with his speed and vertical angle
//				targetVelocity = Quaternion.AngleAxis(glideVerticalAngle, transform.right) * transform.forward * currentSpeed;

//				//Stall when the player is too slow
//				if (currentSpeed < glideStallSpeed){
//					glideVerticalAngle = glideMaxAngle;
//				}

//				//Turn the horizontal input of the player into an angle between glideMinHorizontalAngle and glideMaxHorizontalAngle
//				targetGlideHorizontalAngle = Mathf.Lerp(glideMinHorizontalAngle, glideMaxHorizontalAngle, (inputRaw.x/2) +.5f);
//				//Update the current horizontal angle of the player depending on the angle calculated above
//				glideHorizontalAngle = Mathf.Lerp (glideHorizontalAngle, targetGlideHorizontalAngle, (Mathf.Abs(glideHorizontalAngle) > Mathf.Abs(targetGlideHorizontalAngle) ? glideHorizontalComingBack : glideHorizontalAngleControl) * Time.deltaTime);

//				//Turn the velocity horizontally with the angle calculated above
//				targetVelocity = Quaternion.AngleAxis(glideHorizontalAngle, transform.up) * targetVelocity;

//				/* STRAFE
//			targetGlideStrafeAngle = (rightTrigger - leftTrigger) * glideStrafeMaxAngle;

//			glideStrafeAngle = Mathf.Lerp(glideStrafeAngle, targetGlideStrafeAngle, glideStrafeControlAngle * Time.deltaTime);

//			targetVelocity = Quaternion.AngleAxis(glideStrafeAngle, transform.up) * targetVelocity;
//            */
//				velocity.y = 0f;
//				flatVelocity = targetVelocity;

//				if (pressedSprint) {
//					currentPlayerState = ePlayerState.inAir;
//					EndGlide();
//				}
//				break;
//				#endregion gliding


//				#region sliding
//			case ePlayerState.sliding:
//				permissiveJumpTime = canStillJumpTime;
//				playerMod.UnflagAbility(eAbilityType.DoubleJump);
//				float speed = Mathf.Lerp(gravityStrength, 0, Vector3.Dot(transform.up, controller.collisions.currentGroundNormal));
//				targetVelocity = Quaternion.AngleAxis(90f, Vector3.Cross(transform.up, controller.collisions.currentGroundNormal)) * controller.collisions.currentGroundNormal * speed;
//				targetVelocity = TurnSpaceToLocal(targetVelocity);
//				targetVelocity += inputToSlope;
//				flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, slopeControl * Time.deltaTime * (keepMomentum ? momentumCoeff : 1f));

//				if (pressedJump) {
//					pressedJump = false;
//					velocity.y = 0f;
//					flatVelocity += maxJumpVelocity/2 * TurnSpaceToLocal(controller.collisions.currentGroundNormal) + maxJumpVelocity/2 * Vector3.up;
//					currentPlayerState = ePlayerState.inAir;
//				}
//				break;
//				#endregion sliding


//				#region in wind tunnel
//			case ePlayerState.inWindTunnel:


//				keepMomentum = false;
//				flatVelocity = velocity;
//				velocity.y = 0;
//				//targetVelocity = inputToCamera * characSpeed;
//				flatVelocity = Vector3.Lerp(flatVelocity, windVelocity, airControl * Time.deltaTime);
//				windVelocity = Vector3.zero;
//				if (pressedDash && dashTimer <= 0f && playerMod.CheckAbilityActive(eAbilityType.Dash)) {
//					StartDash();

//				}
//				break;
//				#endregion in wind tunnel

//		}

//		velocity = new Vector3(0, velocity.y, 0);
//		velocity += flatVelocity;

//		#endregion direction calculations


//		//Turns the velocity in world space and calls the controller to check if the calculated velocity will run into walls and stuff, and then move the player
//		turnedVelocity = TurnLocalToSpace(velocity);
//		if (currentPlayerState == ePlayerState.gliding) {
//			velocity = controller.Move (velocity * Time.deltaTime + externalVelocity);
//		} else {
//			velocity = controller.Move (turnedVelocity * Time.deltaTime + externalVelocity);
//		}
//		externalVelocity = Vector3.zero;

//		#region update state

//		switch (currentPlayerState) {
//			default:
//				currentPlayerState = ePlayerState.inAir;
//				break;


//			case ePlayerState.inAir:
//				if (controller.collisions.below) {
//					if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < maxSlopeAngle){
//						camera.SetVerticalOffset(-TurnSpaceToLocal(controller.collisions.initialVelocityOnThisFrame).y * landingCameraOffsetStrength);
//						currentPlayerState = ePlayerState.onGround;
//						if (leftStickAtZero) {
//							velocity = Vector3.zero;
//						} else {
//							velocity = Vector3.ProjectOnPlane(velocity, transform.right);
//						}
//					} else if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < minWallAngle) {
//						currentPlayerState = ePlayerState.sliding;
//					}
//				} else if (inWindTunnel) {
//					currentPlayerState = ePlayerState.inWindTunnel;
//				}
//				break;


//			case ePlayerState.onGround:
//				if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) > maxSlopeAngle && Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < minWallAngle){
//					currentPlayerState = ePlayerState.sliding;
//				} 
//				if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) > minWallAngle || !controller.collisions.below) {
//					currentPlayerState = ePlayerState.inAir;
//				}
//				if (inWindTunnel) {
//					currentPlayerState = ePlayerState.inWindTunnel;
//				}
//				break;


//			case ePlayerState.dashing:
//				break;


//			case ePlayerState.gliding:
//				if (controller.collisions.below) {
//					if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) > maxSlopeAngle){
//						if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < minWallAngle) {
//							currentPlayerState = ePlayerState.sliding;
//							EndGlide();
//						}
//					} else {
//						currentPlayerState = ePlayerState.onGround;
//						EndGlide();
//					}
//				}
//				break;


//			case ePlayerState.sliding:
//				if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < maxSlopeAngle){
//					currentPlayerState = ePlayerState.onGround;
//					keepMomentum = true;
//				}
//				if (!controller.collisions.below) {
//					currentPlayerState = ePlayerState.inAir;
//					keepMomentum = true;
//				}
//				break;

//			case ePlayerState.inWindTunnel:
//				if (!inWindTunnel){
//					currentPlayerState = ePlayerState.inAir;
//					keepMomentum = true;
//				}
//				break;
//		}
//		#endregion update state

//		#region update animator
//		float keyHalf = 0.5f;
//		float m_RunCycleLegOffset = 0.2f;

//		animator.SetBool ("OnGround", controller.collisions.below);
//		animator.SetFloat ("Forward", inputRaw.magnitude);
//		animator.SetFloat ("Turn", Mathf.Lerp (0f, Vector3.SignedAngle (transform.forward, Vector3.ProjectOnPlane (TurnLocalToSpace(inputToCamera), transform.up), transform.up), playerModelTurnSpeed * Time.deltaTime)/7f);
//		animator.SetFloat ("Jump", turnedVelocity.y/5);
//		float runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
//		float jumpLeg = (runCycle < keyHalf ? 1 : -1) * inputRaw.magnitude;
//		if (controller.collisions.below) {
//			animator.SetFloat("JumpLeg", jumpLeg);
//		}

//		windParticles.SetVelocity(velocity);
//		glideParticles.SetVelocity(velocity);
//		#endregion update animator

//	}

//    void OnDestroy()
//    {
//        Game.Utilities.EventManager.OnMenuSwitchedEvent -= HandleEventMenuSwitched;
//        Game.Utilities.EventManager.TeleportPlayerEvent -= HandleEventTeleportPlayer;
//    }

//    void StartDash(){
//		playerMod.FlagAbility (eAbilityType.Dash);
//		currentPlayerState = ePlayerState.dashing;
//		dashDuration = dashSpeed;
//		dashParticles.Play ();
//	}

//	void EndDash(){
//		currentPlayerState = ePlayerState.inAir;
//		dashDuration = 0f;
//		dashTimer = dashCooldown;
//		playerMod.UnflagAbility (eAbilityType.Dash);
//	}

//	void EndGlide(){
//		flatVelocity = TurnSpaceToLocal(flatVelocity);
//		glideParticles.Stop();
//		playerMod.UnflagAbility(eAbilityType.Glide);
//		keepMomentum = true;
//	}



//	#region public utilty functions

//	public void AddExternalVelocity(Vector3 newVelocity, bool worldSpace, bool framerateDependant){
//		if (framerateDependant) {
//			velocity += (worldSpace ? TurnSpaceToLocal(newVelocity) : newVelocity);
//		} else {
//			externalVelocity += (worldSpace ? TurnSpaceToLocal(newVelocity) : newVelocity);
//		}
//	}

//	public void SetVelocity(Vector3 newVelocity, bool worldSpace, bool framerateDependant){
//		velocity = (worldSpace? TurnSpaceToLocal(newVelocity) : newVelocity) * (framerateDependant ? Time.deltaTime : 1);
//	}


//	public void AddWindVelocity(Vector3 newVelocity){
//		inWindTunnel = true;
//		if (windVelocity == Vector3.zero)
//		{
//			windVelocity = newVelocity;
//		} else {
//			windVelocity = Vector3.Lerp((newVelocity), windVelocity, .5f);
//		}
//	}
//	public void ExitWindTunnel(){
//		inWindTunnel = false;
//	}

//	public Vector3 TurnLocalToSpace(Vector3 vector){
//		return (Quaternion.AngleAxis (Vector3.Angle (Vector3.up, transform.up), Vector3.Cross (Vector3.up, transform.up))) * vector;
//	}
//	public Vector3 TurnSpaceToLocal(Vector3 vector){
//		return (Quaternion.AngleAxis (Vector3.Angle (Vector3.up, transform.up), Vector3.Cross (transform.up, Vector3.up))) * vector;
//	}


//	public void ChangeGravityDirection(Vector3 newGravity){
//		gravity = newGravity.normalized;
//		transform.Rotate (Vector3.Cross(transform.up, -gravity), Vector3.SignedAngle(transform.up, -gravity, Vector3.Cross(transform.up, -gravity)),Space.World);
//	}

//	public void ChangeGravityDirection(Vector3 newGravity, Vector3 point){
//		gravity = newGravity.normalized;
//		transform.RotateAround (point, Vector3.Cross(transform.up, -gravity),  Vector3.SignedAngle(transform.up, -gravity, Vector3.Cross(transform.up, -gravity)));
//	}


//	public void InitializePlayer(PlayerModel playmod) {
//        this.isInitialized = true;

//		playerMod = playmod;
//	}

//	#endregion public utilty functions


//}

//public enum ePlayerState {
//	inAir,
//	onGround,
//	gliding,
//	dashing,
//	sliding, 
//	inWindTunnel
//}