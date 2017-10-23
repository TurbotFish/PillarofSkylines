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
	public float airControl = .3f;
	/// <summary>
	/// The speed at which the player's speed changes in the air.
	/// </summary>
	[Tooltip("The speed at which the player's speed changes in the air.")]
	public float groundControl = .8f;
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
	public float turnSmoothTime_default = 10f;
	/// <summary>
	/// The time it takes for the player to turn towards the player's input when turning back (make it instant).
	/// </summary>
	[Tooltip("The time it takes for the player to turn towards the player's input when turning back (make it instant).")]
	public float turnSmoothTime_uTurn = 60f;
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
	/// The minimal speed at the start of the glide.
	/// </summary>
	[Tooltip("The minimal speed at the start of the glide")]
	public float glideMinimalInitialSpeed = 5f;
	/// <summary>
	/// The maximal speed when gliding.
	/// </summary>
	[Tooltip("The maximal speed when gliding.")]
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
	/// The speed at which the player turns when gliding.
	/// </summary>
	[Tooltip("The speed at which the player turns when gliding.")]
	public float glideTurnSpeed = 10f;
	/// <summary>
	/// How much does gliding slow down the speed.
	/// </summary>
	[Tooltip("How much does gliding slow down the speed.")]
	public float glideDrag = .2f;
	/// <summary>
	/// The speed at which the velocity is evolving when gliding (The higher it is, the slower it goes).
	/// </summary>
	[Tooltip("The speed at which the velocity is going up when gliding (The higher it is, the slower it goes).")]
	public float glideAccelerate;
	/// <summary>
	/// The speed at which the velocity is evolving when gliding (The higher it is, the slower it goes).
	/// </summary>
	[Tooltip("The speed at which the velocity is slowing down when gliding (The higher it is, the slower it goes).")]
	public float glideDecelerate;
	/// <summary>
	/// The speed minimum speed required to continue gliding.
	/// </summary>
	[Tooltip("The speed minimum speed required to continue gliding.")]
	public float glideLimitSpeed = 2;
	/// <summary>
	/// The time the player has to wait after failing a glide (by going too slow).
	/// </summary>
	[Tooltip("The time the player has to wait after failing a glide (by going too slow).")]
	public float timeBetweenGlides = 3f;
	/// <summary>
	/// The timer after the player failed a glide.
	/// </summary>
	float glideTimer = 0f;
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
	/// The controller checking if there's collisions on the way.
	/// </summary>
	CharacControllerRecu controller;

	/// <summary>
	/// The animator of the character.
	/// </summary>
	Animator animator;

	/// <summary>
	/// The rotator used to turn the player.
	/// </summary>
	public Transform rotator;

	/// <summary>
	/// The script to get info about abilities.
	/// </summary>
	public PlayerModel playerMod;

	bool readingInputs = true;

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

		#region input detection

		Vector3 inputRaw = new Vector3(Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		Vector3 inputToCamera = rotator.forward * Input.GetAxisRaw ("Vertical") + rotator.right * Input.GetAxisRaw ("Horizontal");
		Vector3 flatVelocity = velocity;
		flatVelocity.y = 0;

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
			if (Input.GetButtonUp ("Jump")) {
				if (velocity.y > minJumpVelocity) {
					velocity.y = minJumpVelocity;
				}
			}
			break;


		case ePlayerState.onGround:
			flatVelocity = (Quaternion.AngleAxis(Vector3.Angle(transform.up, controller.collisions.currentGroundNormal), Vector3.Cross(controller.collisions.currentGroundNormal, transform.up))) * velocity;
			Debug.Log("velocity : " + velocity + " to normal : " + flatVelocity);
			velocity.y = 0f;
			targetVelocity = inputToCamera * characSpeed;
			flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, groundControl * Time.deltaTime);
			flatVelocity = (Quaternion.AngleAxis(Vector3.Angle(transform.up, controller.collisions.currentGroundNormal), Vector3.Cross(transform.up, controller.collisions.currentGroundNormal))) * flatVelocity;

			if (Input.GetButtonDown("Jump")) {
				velocity.y = maxJumpVelocity;
				currentPlayerState = ePlayerState.inAir;
			}
			break;


		case ePlayerState.dashing:
			velocity.y -= gravity;
			velocity = Vector3.Lerp(velocity, targetVelocity, airControl);
			break;


		case ePlayerState.gliding:
			velocity.y -= gravity;
			velocity = Vector3.Lerp(velocity, targetVelocity, airControl);
			break;


		case ePlayerState.sliding:
			velocity.y -= gravity;
			velocity = Vector3.Lerp(velocity, targetVelocity, airControl);
			break;
		}
		#endregion direction

		velocity = new Vector3(0, velocity.y, 0);
		velocity += flatVelocity;
		//Calls the controller to check if the calculated velocity will run into walls and stuff
		velocity = controller.Move ((Quaternion.AngleAxis(Vector3.Angle(Vector3.up, transform.up), Vector3.Cross(Vector3.up, transform.up))) * velocity / 10f);

		#region update animator
		float keyHalf = 0.5f;
		float m_RunCycleLegOffset = 0.2f;

		animator.transform.LookAt(transform.position + flatVelocity, transform.up);
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