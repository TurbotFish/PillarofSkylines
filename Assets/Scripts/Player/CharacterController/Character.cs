using UnityEngine;
using Game.Player;

namespace Game.Player.CharacterController
{
    [RequireComponent(typeof(CharacControllerRecu))]
    public class Character : MonoBehaviour
    {

        #region general
        [Header("General")]
        /// <summary>
        /// The strength of the gravity.
        /// </summary>
        [Tooltip("The strength of the grabity (duh)")]
        public float gravityStrength;
        /// <summary>
        /// The maximum falling speed of the player.
        /// </summary>
        [Tooltip("The maximum speed the player can go vertically. (up or down)")]
        public float maxFallSpeed = 100f;
        /// <summary>
        /// The angle at which the player starts having trouble climbing up.
        /// </summary>
        [Tooltip("The angle at which the player starts having trouble climbing up.")]
        public float minSlopeAngle = 30f;
        /// <summary>
        /// The max walkable slope angle.
        /// </summary>
        [Tooltip("The max walkable slope angle.")]
        public float maxSlopeAngle = 45f;
        /// <summary>
        /// The angle at which the ground is considered a wall.
        /// </summary>
        [Tooltip("The angle at which the ground is considered a wall.")]
        public float minWallAngle = 75f;
        /// <summary>
        /// The speed at which the player model turns around (only visual, no gameplay incidence).
        /// </summary>
        [Tooltip("The speed at which the player model turns around (only visual, no gameplay incidence).")]
        public float playerModelTurnSpeed = 4f;
        /// <summary>
        /// The speed at which the player model turns around in the air (only visual, no gameplay incidence).
        /// </summary>
        [Tooltip("The speed at which the player model turns around in the air (only visual, no gameplay incidence).")]
        public float playerModelAerialTurnSpeed = 4f;
        /// <summary>
        /// The strength of the camera offset when the player lands on ground.
        /// </summary>
        [Tooltip("The strength of the camera offset when the player lands on ground.")]
        public float landingCameraOffsetStrength = .5f;
        /// <summary>
        /// The maximum height of step the player can climb.
        /// </summary>
        [Tooltip("The maximum height of step the player can climb.")]
        public float maxStepHeight = 1f;
        /// <summary>
        /// The distance at which the player stops when encountering a cliff with no input.
        /// </summary>
        [Tooltip("The distance at which the player stops when encountering a cliff with no input.")]
        public float distanceStoppingCliff = .1f;
        #endregion general

        #region speed and controls variables
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
        /// The value by which the speed update of the player is multiplied when there's no input on the left stick.
        /// </summary>
        [Tooltip("The value by which the speed update of the player is multiplied when there's no input on the left stick.")]
        public float groundNoInputCoeff = 5f;
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
        /// How much being on slanted terrain impacts the player's speed (1 means the player will double their speed down a MaxSlopeAngle slope).
        /// </summary>
        [Tooltip("How much being on slanted terrain impacts the player's speed (1 means the player will double their speed down a MaxSlopeAngle slope).")]
        public float slopeCoeff = .1f;
        #endregion speed and controls variables

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

        [Header("Aerial Jumps")]
        /// <summary>
        /// The number of jumps the player can do while in the air.
        /// </summary>
        [Tooltip("The number of jumps the player can do while in the air.")]
        public int numberOfAerialJumps = 0;
        /// <summary>
        /// The efficiency of the aerial jump compared to the regular jump (2 makes it 2 times stronger, 0.5 makes it 2 times weaker (kinda)).
        /// </summary>
        [Tooltip("The efficiency of the aerial jump compared to the regular jump (2 makes it 2 times stronger, 0.5 makes it 2 times weaker (kinda)).")]
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

        Vector3 jumpDirection;

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
        /// The speed at which the player's vertical angle changes upwards when gliding.
        /// </summary>
        [Tooltip("The speed at which the player's vertical angle changes upwards when gliding.")]
        public float glideVerticalUpAngleControl = 1f;


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
        /// <summary>
        /// The target angle of the glide when the player has no input.
        /// </summary>
        public float glideBaseAngle = 10f;
        /// <summary>
        /// The target speed at which the player will naturally come back to when gliding at glideBaseAngle.
        /// </summary>
        public float glideBaseSpeed = 20f;
        /// <summary>
        /// The speed under which the player will stall.
        /// </summary>
        public float glideStallSpeed = 5f;
        /// <summary>
        /// The speed at which the player accelerates
        /// </summary>
        public float glideSpeedSmooth = .1f;
        /// <summary>
        /// The speed added to the target speed depending on the player's angle when gliding downwards.
        /// </summary>
        public AnimationCurve glideDownwardAcceleration;
        /// <summary>
        /// The speed removed from the player's when gliding upwards.
        /// </summary>
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

        public float dashSpeed = .2f;
        public float dashTime = .5f;
        public float dashCooldown = 1f;
        float dashTimer = 0f;
        float dashDuration = 0f;

        #endregion dash variables

        #region joli variables
        [Header("FX")]
        public ParticlesManager windParticles;
        public ParticlesManager glideParticles;
        public GameObject jumpParticles;
        public ParticlesManager dashParticles;
        #endregion joli variables

        #region other variables
        [Space(20)]
        /// <summary>
        /// The rotator used to turn the camera.
        /// </summary>
        public Transform rotator;

        /// <summary>
        /// The script to get info about abilities.
        /// </summary>
        PlayerModel playerMod;

        /// <summary>
        /// The controller checking if there's collisions on the way.
        /// </summary>
        CharacControllerRecu controller;
        /// <summary>
        /// The animator of the character.
        /// </summary>
        Animator animator;

        new PoS_Camera camera;

        //[HideInInspector]
        public ePlayerState currentPlayerState;

        /// <summary>
        /// The velocity calculated each frame and sent to the controller.
        /// </summary>
        [HideInInspector]
        public Vector3 velocity = Vector3.zero;
        Vector3 externalVelocity = Vector3.zero;

        public Vector3 gravity = -Vector3.up;

        #endregion other variables

        #region private variables
        bool readingInputs = true;
        /// <summary>
        /// Indicates whether may be turned in response to (left stick) input.
        /// </summary>
        bool canTurnPlayer = true;
        bool pressedJump = false;
        bool releasedJump = false;
        bool pressedDash = false;
        bool pressedSprint = false;
        bool pressingSprint = false;

        float leftTrigger;
        float rightTrigger;

        bool leftStickAtZero = false;

        bool keepMomentum = false;
        bool inWindTunnel = false;

        Vector3 inputRaw;
        Vector3 inputToCamera;
        Vector3 inputToSlope;

        Vector3 flatVelocity;
        Vector3 turnedVelocity;
        Vector3 windVelocity;

        float currentSpeed;

        bool isInitialized = false;

        /// <summary>
        /// When this timer is > 0, the left stick input is ignored.
        /// </summary>
        float ignoreLeftStickTimer = 0f;

        /// <summary>
        /// When this timer is > 0, gravity is ignored.
        /// </summary>
        float ignoreGravityTimer = 0f;

        /// <summary>
        /// Timer for the vertical wall run.
        /// </summary>
        float wallRunVerticalTimer = 0f;

        #endregion private variables

        //#############################################################################

        #region initialization

        void Start()
        {
            controller = GetComponent<CharacControllerRecu>();
            animator = GetComponentInChildren<Animator>();
            camera = FindObjectOfType<PoS_Camera>();

            currentPlayerState = ePlayerState.inAir;

            maxJumpVelocity = maxJumpHeight;
            minJumpVelocity = minJumpHeight;
            maxAerialJumpVelocity = maxJumpVelocity * coeffAerialJumpEfficiency;
            minAerialJumpVelocity = minJumpVelocity * coeffAerialJumpEfficiency;
            permissiveJumpTime = canStillJumpTime;

            gravity = gravity.normalized;

            velocity = Vector3.zero;

            Game.Utilities.EventManager.OnMenuSwitchedEvent += HandleEventMenuSwitched;
            Game.Utilities.EventManager.TeleportPlayerEvent += HandleEventTeleportPlayer;
        }

        public void InitializePlayer(PlayerModel playmod)
        {
            this.isInitialized = true;

            playerMod = playmod;
        }

        void OnDestroy()
        {
            Game.Utilities.EventManager.OnMenuSwitchedEvent -= HandleEventMenuSwitched;
            Game.Utilities.EventManager.TeleportPlayerEvent -= HandleEventTeleportPlayer;
        }

        #endregion initialization

        //#############################################################################

        #region event handling

        void HandleEventMenuSwitched(object sender, Game.Utilities.EventManager.OnMenuSwitchedEventArgs args)
        {
            if (args.NewUiState == Game.UI.eUiState.HUD)
            {
                readingInputs = true;
            }
            else
            {
                readingInputs = false;
            }
        }

        void HandleEventTeleportPlayer(object sender, Game.Utilities.EventManager.TeleportPlayerEventArgs args)
        {
            transform.position = args.Position;

            if (args.IsNewScene)
            {
                transform.rotation = args.Rotation;
                velocity = Vector3.zero;
                currentPlayerState = ePlayerState.inAir;
                ChangeGravityDirection(Vector3.down);
            }
        }

        #endregion event handling

        //#############################################################################

        void Update()
        {
            if (!this.isInitialized)
            {
                return;
            }

            //*******************************************

            #region update timers

            dashTimer -= Time.deltaTime;

            if (ignoreLeftStickTimer >= 0)
            {
                ignoreLeftStickTimer -= Time.deltaTime;
            }

            if (ignoreGravityTimer >= 0)
            {
                ignoreGravityTimer -= Time.deltaTime;
            }

            if (wallRunVerticalTimer >= 0)
            {
                wallRunVerticalTimer -= Time.deltaTime;
            }

            #endregion update timers

            //*******************************************
            //*******************************************

            #region turn the player
            //Turn the player in the direction of his input
            if (canTurnPlayer
                && ignoreLeftStickTimer <= 0
                && inputRaw.magnitude > 0f)
            {
                if (currentPlayerState != ePlayerState.gliding)
                {
                    transform.Rotate(transform.up, Mathf.Lerp(0f, Vector3.SignedAngle(transform.forward, Vector3.ProjectOnPlane(TurnLocalToSpace(inputToCamera), transform.up), transform.up),
                        (currentPlayerState == ePlayerState.gliding ? playerModelAerialTurnSpeed : playerModelTurnSpeed) * Time.deltaTime), Space.World);
                }
                else
                {
                    transform.Rotate(transform.up, glideHorizontalAngle, Space.World);
                }
            }
            #endregion turn the player

            //*******************************************
            //*******************************************

            #region input detection

            leftStickAtZero = false;
            pressedJump = false;
            releasedJump = false;
            pressedDash = false;
            pressedSprint = false;
            pressingSprint = false;
            if (readingInputs)
            {

                inputRaw = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                if (inputRaw.magnitude < .1f)
                {
                    leftStickAtZero = true;
                }
                inputToCamera = rotator.forward * Input.GetAxisRaw("Vertical") + rotator.right * Input.GetAxisRaw("Horizontal");
                inputToCamera = (Quaternion.AngleAxis(Vector3.Angle(transform.up, Vector3.up), Vector3.Cross(transform.up, Vector3.up))) * inputToCamera;
                inputToSlope = (Quaternion.AngleAxis(Vector3.Angle(transform.up, controller.collisions.currentGroundNormal), Vector3.Cross(transform.up, controller.collisions.currentGroundNormal))) * inputToCamera;
                //			Debug.Log("Inputs = raw : " + inputRaw + " to camera = : " + inputToCamera + " to slope = " + inputToSlope + " slope " + Vector3.Angle(transform.up, controller.collisions.currentGroundNormal));

                leftTrigger = Input.GetAxisRaw("Left Trigger");
                rightTrigger = Input.GetAxisRaw("Right Trigger");

                if (Input.GetButtonDown("Jump"))
                {
                    pressedJump = true;
                }
                if (Input.GetButtonUp("Jump"))
                {
                    releasedJump = true;
                }
                if (Input.GetButtonDown("Dash"))
                {
                    pressedDash = true;
                }
                if (Input.GetButtonDown("Sprint"))
                {
                    pressedSprint = true;
                }
                if (Input.GetButton("Sprint"))
                {
                    pressingSprint = true;
                }


            }
            else
            {
                inputRaw = Vector3.zero;
                inputToCamera = Vector3.zero;
                inputToSlope = Vector3.zero;
                leftTrigger = 0f;
                rightTrigger = 0f;
            }
            #endregion input detection

            //*******************************************
            //*******************************************

            //Here we react to player input and timers
            #region direction calculations

            flatVelocity = velocity;
            flatVelocity.y = 0;

            Vector3 targetVelocity = Vector3.zero;
            //		Debug.Log("state : " + currentPlayerState + " momentum : " + keepMomentum);
            switch (currentPlayerState)
            {
                #region direction calculations - in air

                case ePlayerState.inAir:

                    permissiveJumpTime -= Time.deltaTime;

                    if (flatVelocity.magnitude < characSpeed)
                        keepMomentum = false;

                    targetVelocity = inputToCamera * characSpeed;

                    //gravity
                    if (ignoreGravityTimer <= 0)
                    {
                        velocity.y -= gravityStrength * Time.deltaTime;
                    }

                    velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, maxFallSpeed);
                    flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, airControl * Time.deltaTime * (keepMomentum ? momentumCoeff : 1f));

                    if (releasedJump)
                    {
                        releasedJump = false;
                        if (lastJumpAerial)
                        {
                            if (velocity.y > minAerialJumpVelocity)
                            {
                                velocity.y = minAerialJumpVelocity;
                            }
                        }
                        else
                        {
                            if (velocity.y > minJumpVelocity)
                            {
                                velocity.y = minJumpVelocity;
                            }
                        }
                    }

                    if (pressedDash && dashTimer <= 0f && playerMod.CheckAbilityActive(eAbilityType.Dash))
                    {
                        QuitStateInAir();

                        EnterStateDash();

                        break;
                    }

                    if (pressedJump)
                    {
                        if (permissiveJumpTime > 0f)
                        {
                            pressedJump = false;
                            velocity.y = maxJumpVelocity;
                            currentPlayerState = ePlayerState.inAir;
                            lastJumpAerial = false;
                        }
                        else if (rmngAerialJumps > 0 && playerMod.CheckAbilityActive(eAbilityType.DoubleJump))
                        {
                            velocity.y = maxAerialJumpVelocity;
                            rmngAerialJumps--;
                            lastJumpAerial = true;
                            playerMod.FlagAbility(eAbilityType.DoubleJump);
                            Instantiate(jumpParticles, transform.position, Quaternion.identity, transform);
                        }
                    }

                    if (pressedSprint && playerMod.CheckAbilityActive(eAbilityType.Glide))
                    {
                        glideParticles.Play();
                        glideVerticalAngle = Vector3.Angle(transform.up, TurnLocalToSpace(velocity)) - 90f;
                        glideHorizontalAngle = 0f;
                        currentPlayerState = ePlayerState.gliding;
                        playerMod.FlagAbility(eAbilityType.Glide);

                        break;
                    }

                    break;

                #endregion direction calculations - in air

                #region direction calculations - on ground

                case ePlayerState.onGround:

                    permissiveJumpTime = canStillJumpTime;


                    if (flatVelocity.magnitude < characSpeed)
                        keepMomentum = false;

                    rmngAerialJumps = numberOfAerialJumps;
                    playerMod.UnflagAbility(eAbilityType.DoubleJump);


                    flatVelocity = velocity;
                    velocity.y = 0f;
                    targetVelocity = inputToSlope * characSpeed * (pressingSprint ? sprintCoeff : 1);
                    flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, groundControl * Time.deltaTime * (keepMomentum ? momentumCoeff : 1f) * (leftStickAtZero ? groundNoInputCoeff : 1f));
                    // Detects if the player is moving up or down the slope, and multiply their speed based on that slope
                    if (Vector3.Angle(transform.up, controller.collisions.currentGroundNormal) > minSlopeAngle)
                    {
                        //					print("Current velocity : " + flatVelocity);
                        flatVelocity *= .99f + slopeCoeff * Vector3.Angle(transform.forward, Vector3.ProjectOnPlane(transform.forward, controller.collisions.currentGroundNormal)) * (Vector3.Dot(transform.forward, controller.collisions.currentGroundNormal) > 0 ? 1 : -1) / (maxSlopeAngle);
                        //					print("New velocity : " + flatVelocity + " modified by : " + (slopeCoeff * Vector3.Angle (transform.forward, Vector3.ProjectOnPlane (transform.forward, controller.collisions.currentGroundNormal)) * (Vector3.Dot (transform.forward, controller.collisions.currentGroundNormal) > 0 ? 1 : -1) / (maxSlopeAngle)));
                    }

                    if (pressedJump)
                    {
                        QuitStateOnGround();

                        pressedJump = false;
                        velocity.y = 0f;
                        flatVelocity += maxJumpVelocity * Vector3.up + flatVelocity / 10f;
                        lastJumpAerial = false;

                        EnterStateInAir();
                    }
                    else if (pressedDash && dashTimer <= 0f && playerMod.CheckAbilityActive(eAbilityType.Dash))
                    {
                        QuitStateOnGround();

                        EnterStateDash();
                        //If the player is going off a cliff when the left stick is at zero, stop the player
                    }
                    else if (leftStickAtZero)
                    {
                        if (!Physics.Raycast(transform.position + transform.up * controller.skinWidth + (Vector3.ProjectOnPlane(transform.forward, controller.collisions.currentGroundNormal).normalized * distanceStoppingCliff), -controller.collisions.currentGroundNormal, maxStepHeight, controller.collisionMask))
                        {
                            flatVelocity = Vector3.zero;
                        }
                    }
                    break;


                #endregion direction calculations - on ground

                #region direction calculations - dashing

                case ePlayerState.dashing:

                    flatVelocity = transform.forward * dashSpeed;
                    flatVelocity = TurnSpaceToLocal(flatVelocity);
                    flatVelocity.y = 0f;
                    velocity.y = 0f;
                    dashDuration -= Time.deltaTime;

                    if (dashDuration <= 0)
                    {
                        QuitStateDash();

                        EnterStateInAir();
                    }

                    break;

                #endregion direction calculations - dashing

                #region direction calculations - gliding

                case ePlayerState.gliding:

                    //Attention : la vélocité glide est calculé dans le world space et non dans le local space !

                    //Turn the vertical input of the player into an angle between glideMinAngle and glideMaxAngle
                    targetGlideVerticalAngle = Mathf.Clamp(Mathf.Lerp(glideMinAngle, glideMaxAngle, (inputRaw.z / 2) + .5f) + glideBaseAngle, glideMinAngle, glideMaxAngle);
                    //Update the current vertical angle of the player depending on the angle calculated above
                    glideVerticalAngle = Mathf.Lerp(glideVerticalAngle, targetGlideVerticalAngle, (targetGlideVerticalAngle > glideVerticalAngle ? glideVerticalUpAngleControl : glideVerticalDownAngleControl) * Time.deltaTime * (targetGlideVerticalAngle == glideBaseAngle ? glideNoInputImpactCoeff : 1f));

                    //Update the speed of the player
                    if (glideVerticalAngle < glideBaseAngle)
                    {
                        currentSpeed = velocity.magnitude - glideUpwardDecelaration.Evaluate(Mathf.Abs((glideVerticalAngle - glideBaseAngle) / (glideMinAngle - glideBaseAngle))) * Time.deltaTime;
                    }
                    else
                    {
                        currentSpeed = Mathf.Lerp(velocity.magnitude, glideBaseSpeed + glideDownwardAcceleration.Evaluate((glideVerticalAngle - glideBaseAngle) / (glideMaxAngle - glideBaseAngle)), glideSpeedSmooth * Time.deltaTime);
                    }

                    //Calculate the velocity of the player with his speed and vertical angle
                    targetVelocity = Quaternion.AngleAxis(glideVerticalAngle, transform.right) * transform.forward * currentSpeed;

                    //Stall when the player is too slow
                    if (currentSpeed < glideStallSpeed)
                    {
                        glideVerticalAngle = glideMaxAngle;
                    }

                    //Turn the horizontal input of the player into an angle between glideMinHorizontalAngle and glideMaxHorizontalAngle
                    targetGlideHorizontalAngle = Mathf.Lerp(glideMinHorizontalAngle, glideMaxHorizontalAngle, (inputRaw.x / 2) + .5f);
                    //Update the current horizontal angle of the player depending on the angle calculated above
                    glideHorizontalAngle = Mathf.Lerp(glideHorizontalAngle, targetGlideHorizontalAngle, (Mathf.Abs(glideHorizontalAngle) > Mathf.Abs(targetGlideHorizontalAngle) ? glideHorizontalComingBack : glideHorizontalAngleControl) * Time.deltaTime);

                    //Turn the velocity horizontally with the angle calculated above
                    targetVelocity = Quaternion.AngleAxis(glideHorizontalAngle, transform.up) * targetVelocity;

                    ////STRAFE
                    //targetGlideStrafeAngle = (rightTrigger - leftTrigger) * glideStrafeMaxAngle;

                    //glideStrafeAngle = Mathf.Lerp(glideStrafeAngle, targetGlideStrafeAngle, glideStrafeControlAngle * Time.deltaTime);

                    //targetVelocity = Quaternion.AngleAxis(glideStrafeAngle, transform.up) * targetVelocity;

                    velocity.y = 0f;
                    flatVelocity = targetVelocity;

                    if (pressedSprint)
                    {
                        QuitStateGliding();

                        EnterStateInAir();
                    }

                    break;

                #endregion direction calculations - gliding

                #region direction calculations - sliding

                case ePlayerState.sliding:

                    permissiveJumpTime = canStillJumpTime;
                    playerMod.UnflagAbility(eAbilityType.DoubleJump);
                    float speed = Mathf.Lerp(gravityStrength, 0, Vector3.Dot(transform.up, controller.collisions.currentGroundNormal));
                    targetVelocity = Quaternion.AngleAxis(90f, Vector3.Cross(transform.up, controller.collisions.currentGroundNormal)) * controller.collisions.currentGroundNormal * speed;
                    targetVelocity = TurnSpaceToLocal(targetVelocity);
                    targetVelocity += inputToSlope;
                    flatVelocity = Vector3.Lerp(flatVelocity, targetVelocity, slopeControl * Time.deltaTime * (keepMomentum ? momentumCoeff : 1f));

                    if (pressedJump)
                    {
                        QuitStateSliding();

                        pressedJump = false;
                        velocity.y = 0f;
                        flatVelocity += maxJumpVelocity / 2 * TurnSpaceToLocal(controller.collisions.currentGroundNormal) + maxJumpVelocity / 2 * Vector3.up;

                        EnterStateInAir();
                    }

                    break;

                #endregion direction calculations - sliding

                #region direction calculations - in wind tunnel

                case ePlayerState.inWindTunnel:

                    keepMomentum = false;
                    flatVelocity = velocity;
                    velocity.y = 0;
                    //targetVelocity = inputToCamera * characSpeed;
                    flatVelocity = Vector3.Lerp(flatVelocity, windVelocity, airControl * Time.deltaTime);
                    windVelocity = Vector3.zero;
                    if (pressedDash && dashTimer <= 0f && playerMod.CheckAbilityActive(eAbilityType.Dash))
                    {
                        QuitStateInWindTunnel();

                        EnterStateDash();

                        break;
                    }

                    break;

                #endregion direction calculations - in wind tunnel

                #region direction calculations - wall drifting

                case ePlayerState.WallDrifting:
                    //jump
                    if (pressedJump)
                    {
                        lastJumpAerial = false;

                        QuitStateWallDrift();

                        var dir = controller.collisions.currentWallNormal;
                        transform.forward = dir;

                        jumpDirection = (transform.up + dir * 2).normalized;

                        flatVelocity = playerMod.AbilityData.WallRun.WallJump.Strength * jumpDirection;
                        velocity.y = 0f;

                        ignoreGravityTimer = playerMod.AbilityData.WallRun.WallJump.IgnoreGravityDuration;
                        ignoreLeftStickTimer = playerMod.AbilityData.WallRun.WallJump.IgnoreStickDuration;

                        EnterStateInAir();

                        break;
                    }
                    //interrupt
                    if (!CheckWallRunStick())
                    {
                        QuitStateWallDrift();

                        EnterStateInAir();

                        break;
                    }
                    //default
                    float wallDriftSpeed = Mathf.Lerp(velocity.magnitude, playerMod.AbilityData.WallRun.WallDrift.TargetSpeed, playerMod.AbilityData.WallRun.WallDrift.LerpFactor * Time.deltaTime);

                    flatVelocity = -transform.up * wallDriftSpeed;
                    velocity.y = 0f;

                    break;

                #endregion direction calculations - wall drifting

                #region direction calculation - wall run horizontal

                case ePlayerState.WallRunningHorizontal:
                    //jump
                    if (pressedJump)
                    {
                        lastJumpAerial = false;

                        QuitStateWallRunHorizontal();

                        var dir = (controller.collisions.currentWallNormal + transform.forward).normalized;
                        transform.forward = new Vector3(dir.x, 0, dir.z).normalized;

                        jumpDirection = (transform.up + dir * 2).normalized;

                        velocity.y = 0f;
                        flatVelocity = playerMod.AbilityData.WallRun.WallJump.Strength * jumpDirection;

                        ignoreGravityTimer = playerMod.AbilityData.WallRun.WallJump.IgnoreGravityDuration;
                        ignoreLeftStickTimer = playerMod.AbilityData.WallRun.WallJump.IgnoreStickDuration;

                        EnterStateInAir();

                        break;
                    }
                    //interrupt
                    if (!CheckWallRunStick())
                    {
                        QuitStateWallRunHorizontal();

                        EnterStateInAir();

                        break;
                    }
                    //time over
                    if (flatVelocity.magnitude < playerMod.AbilityData.WallRun.WallRunHorizontal.MinSpeed)
                    {
                        QuitStateWallRunHorizontal();

                        Debug.LogWarning("Entering wall drift without check!");
                        EnterStateWallDrift();

                        break;
                    }
                    //default
                    var forward = transform.forward * playerMod.AbilityData.WallRun.WallRunHorizontal.ForwardSpeed;
                    var gravity = -Vector3.up * playerMod.AbilityData.WallRun.WallRunHorizontal.Gravity;
                    var momentum = velocity * playerMod.AbilityData.WallRun.WallRunHorizontal.Momentum;
                    var acceleration = momentum + gravity + forward;

                    flatVelocity = acceleration;
                    velocity = Vector3.zero;

                    //velocity.y = -gravityStrength * playerMod.AbilityData.WallRun.WallRunHorizontal.GravityMultiplier * Time.deltaTime;
                    //flatVelocity = transform.forward * Mathf.Lerp(flatVelocity.magnitude, playerMod.AbilityData.WallRun.WallRunHorizontal.TargetSpeed, 0.6f * Time.deltaTime);

                    break;

                #endregion direction calculation - wall run horizontal

                #region direction calculation - wall run vertical

                case ePlayerState.WallRunningVertical:
                    //jump
                    if (pressedJump)
                    {
                        lastJumpAerial = false;

                        QuitStateWallRunVertical();

                        var dir = controller.collisions.currentWallNormal;
                        transform.forward = dir;

                        jumpDirection = (transform.up + dir * 2).normalized;

                        flatVelocity = playerMod.AbilityData.WallRun.WallJump.Strength * jumpDirection;
                        velocity.y = 0f;

                        ignoreGravityTimer = playerMod.AbilityData.WallRun.WallJump.IgnoreGravityDuration;
                        ignoreLeftStickTimer = playerMod.AbilityData.WallRun.WallJump.IgnoreStickDuration;

                        EnterStateInAir();

                        break;
                    }
                    //interrupt
                    if (!CheckWallRunStick())
                    {
                        QuitStateWallRunVertical();

                        EnterStateInAir();

                        break;
                    }
                    //time's over
                    if (wallRunVerticalTimer <= 0)
                    {
                        QuitStateWallRunVertical();

                        Debug.LogWarning("Entering wall drift without check!");
                        EnterStateWallDrift();

                        break;
                    }
                    //default
                    float wallRunSpeed = playerMod.AbilityData.WallRun.WallRunVertical.TargetSpeed;
                    float factor = playerMod.AbilityData.WallRun.WallRunVertical.SlowdownFactor;
                    if (velocity.magnitude <= wallRunSpeed)
                    {
                        factor = playerMod.AbilityData.WallRun.WallRunVertical.AccelerationFactor;
                    }
                    wallRunSpeed = Mathf.Lerp(velocity.magnitude, wallRunSpeed, factor);

                    flatVelocity = transform.up * wallRunSpeed;
                    velocity.y = 0f;

                    break;

                #endregion direction calculation - wall run vertical

                default:
                    Debug.LogWarning("pas de player state >:c");
                    break;
            }

            velocity = new Vector3(0, velocity.y, 0);
            velocity += flatVelocity;

            #endregion direction calculations

            //*******************************************
            //*******************************************

            //Turns the velocity in world space and calls the controller to check if the calculated velocity will run into walls and stuff, and then move the player
            #region physics

            turnedVelocity = TurnLocalToSpace(velocity);
            if (currentPlayerState == ePlayerState.gliding)
            {
                velocity = controller.Move(velocity * Time.deltaTime + externalVelocity);
            }
            else
            {
                velocity = controller.Move(turnedVelocity * Time.deltaTime + externalVelocity);
            }
            externalVelocity = Vector3.zero;

            #endregion physics

            //*******************************************
            //*******************************************

            //Here we react to the environment (Collisions, ...)
            #region update state

            switch (currentPlayerState)
            {
                default:
                    currentPlayerState = ePlayerState.inAir;
                    break;

                #region update state - in air

                case ePlayerState.inAir:
                    //land on the ground
                    if (controller.collisions.below)
                    {
                        //start sliding
                        if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) > maxSlopeAngle
                            && Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < minWallAngle)
                        {
                            QuitStateInAir();

                            EnterStateSliding();

                            break;
                        }
                        //or not
                        QuitStateInAir();

                        EnterStateOnGround();

                        break;
                    }
                    //enter wind tunnel
                    if (inWindTunnel)
                    {
                        QuitStateInAir();

                        EnterStateInWindTunnel();

                        break;
                    }
                    //start horizontal wall run
                    if (CheckCanStartWallRunHorizontal())
                    {
                        QuitStateInAir();

                        EnterStateWallRunHorizontal();

                        break;
                    }
                    //start wall drifting
                    if (CheckCanStartWallDrift(true))
                    {
                        QuitStateInAir();

                        EnterStateWallDrift();

                        break;
                    }

                    break;

                #endregion update state - in air

                #region update state - on ground

                case ePlayerState.onGround:
                    //start sliding
                    if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) > maxSlopeAngle
                        && Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < minWallAngle)
                    {
                        QuitStateOnGround();

                        EnterStateSliding();

                        break;
                    }
                    //start falling
                    if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) > minWallAngle || !controller.collisions.below)
                    {
                        QuitStateOnGround();

                        EnterStateInAir();

                        break;
                    }
                    //enter wind tunnel
                    if (inWindTunnel)
                    {
                        QuitStateOnGround();

                        EnterStateInWindTunnel();

                        break;
                    }
                    //start a vertical wall run
                    if (CheckCanStartWallRunVertical())
                    {
                        QuitStateOnGround();

                        EnterStateWallRunVertical();

                        break;
                    }

                    break;

                #endregion update state - on ground

                #region update state - dashing

                case ePlayerState.dashing:
                    //touching a wall
                    if (controller.collisions.side) //this is redundant but saves on doing all the checks when the player is not touching a wall
                    {
                        //start a vertical wall run
                        if (CheckCanStartWallRunVertical())
                        {
                            QuitStateDash();

                            EnterStateWallRunVertical();

                            break;
                        }
                        //start a horizontal wall run
                        if (CheckCanStartWallRunHorizontal())
                        {
                            QuitStateDash();

                            EnterStateWallRunHorizontal();

                            break;
                        }
                        //start a wall drift
                        if (CheckCanStartWallDrift(true))
                        {
                            QuitStateDash();

                            EnterStateWallDrift();

                            break;
                        }
                    }

                    break;

                #endregion update state - dashing

                #region update state - gliding

                case ePlayerState.gliding:
                    //landing on the ground
                    if (controller.collisions.below)
                    {
                        //start sliding
                        if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) > maxSlopeAngle
                            && Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < minWallAngle)
                        {
                            QuitStateGliding();

                            EnterStateSliding();

                            break;
                        }
                        //
                        else
                        {
                            QuitStateGliding();

                            EnterStateOnGround();

                            break;
                        }
                    }

                    break;

                #endregion update state - gliding

                #region update state - sliding

                case ePlayerState.sliding:
                    //landing on the ground
                    if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < maxSlopeAngle)
                    {
                        QuitStateSliding();

                        EnterStateOnGround();

                        break;
                    }
                    //its a trap
                    if (!controller.collisions.below)
                    {
                        QuitStateSliding();

                        EnterStateInAir();

                        break;
                    }

                    break;

                #endregion update state - sliding

                #region update state - in wind tunnel

                case ePlayerState.inWindTunnel:

                    if (!inWindTunnel)
                    {
                        QuitStateInWindTunnel();

                        EnterStateInAir();

                        break;
                    }

                    break;

                #endregion update state - in wind tunnel

                #region update state - wall drift

                case ePlayerState.WallDrifting:
                    //landing on the ground
                    if (controller.collisions.below)
                    {
                        if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < maxSlopeAngle)
                        {
                            QuitStateWallDrift();

                            EnterStateOnGround();

                            break;
                        }
                        else if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < minWallAngle)
                        {
                            QuitStateWallDrift();

                            EnterStateSliding();

                            break;
                        }
                    }
                    //not touching a wall anymore
                    if (!controller.collisions.side)
                    {
                        QuitStateWallDrift();

                        EnterStateInAir();

                        break;
                    }

                    break;

                #endregion update state - wall drift

                #region update state - wall run horizontal

                case ePlayerState.WallRunningHorizontal:
                    //landing on the ground
                    if (controller.collisions.below)
                    {
                        //start sliding
                        if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) > maxSlopeAngle
                            && Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < minWallAngle)
                        {
                            QuitStateWallRunHorizontal();

                            EnterStateSliding();

                            break;
                        }
                        //or not
                        QuitStateWallRunHorizontal();

                        EnterStateOnGround();

                        break;
                    }
                    //not touching a wall anymore
                    if (!controller.collisions.side)
                    {
                        QuitStateWallRunHorizontal();

                        EnterStateInAir();

                        break;
                    }

                    break;

                #endregion update state - wall run horizontal

                #region update state - wall run vertical

                case ePlayerState.WallRunningVertical:
                    //landing on the ground
                    if (controller.collisions.below)
                    {
                        if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < maxSlopeAngle)
                        {
                            QuitStateWallRunVertical();

                            EnterStateOnGround();

                            break;
                        }
                        else if (Vector3.Angle(controller.collisions.currentGroundNormal, transform.up) < minWallAngle)
                        {
                            QuitStateWallRunVertical();

                            EnterStateSliding();

                            break;
                        }
                    }
                    //not touching a wall anymore
                    if (!controller.collisions.side)
                    {
                        QuitStateWallRunVertical();

                        EnterStateInAir();

                        break;
                    }

                    break;

                    #endregion update state - wall run vertical
            }
            #endregion update state

            //*******************************************
            //*******************************************

            #region update animator
            float keyHalf = 0.5f;
            float m_RunCycleLegOffset = 0.2f;

            animator.SetBool("OnGround", controller.collisions.below);
            animator.SetFloat("Forward", velocity.magnitude / characSpeed);
            animator.SetFloat("Turn", (leftStickAtZero ? 0f : Mathf.Lerp(0f, Vector3.SignedAngle(transform.forward, Vector3.ProjectOnPlane(TurnLocalToSpace(inputToCamera), transform.up), transform.up), playerModelTurnSpeed * Time.deltaTime) / 7f));
            animator.SetFloat("Jump", turnedVelocity.y / 5);
            float runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
            float jumpLeg = (runCycle < keyHalf ? 1 : -1) * inputRaw.magnitude;
            if (controller.collisions.below)
            {
                animator.SetFloat("JumpLeg", jumpLeg);
            }

            windParticles.SetVelocity(velocity);
            glideParticles.SetVelocity(velocity);
            #endregion update animator

        }

        //#############################################################################

        #region state change methods

        //*******************************************

        #region state change - in air

        /// <summary>
        /// Sets the state of the player to <see cref="ePlayerState.inAir"/>.
        /// </summary>
        void EnterStateInAir()
        {
            //Debug.Log("EnterStateInAir");

            currentPlayerState = ePlayerState.inAir;
        }

        /// <summary>
        /// Quits the state <see cref="ePlayerState.inAir"/>.
        /// </summary>
        void QuitStateInAir()
        {
            //Debug.Log("QuitStateInAir");
        }

        #endregion state change - in air

        //*******************************************

        #region state change - on ground

        /// <summary>
        /// Sets the state of the player to <see cref="ePlayerState.onGround"/>.
        /// </summary>
        void EnterStateOnGround()
        {
            //Debug.Log("EnterStateOnGround");

            camera.SetVerticalOffset(-TurnSpaceToLocal(controller.collisions.initialVelocityOnThisFrame).y * landingCameraOffsetStrength);
            currentPlayerState = ePlayerState.onGround;
            if (leftStickAtZero)
            {
                velocity = Vector3.zero;
            }
            else
            {
                velocity = Vector3.ProjectOnPlane(velocity, transform.right);
            }
        }

        /// <summary>
        /// Quits the state <see cref="ePlayerState.onGround"/>.
        /// </summary>
        void QuitStateOnGround()
        {
            //Debug.Log("QuitStateOnGround");
        }

        #endregion state change - on ground

        //*******************************************

        #region state change - gliding

        void EnterStateGliding()
        {
            //Debug.Log("EnterStateGliding");

            currentPlayerState = ePlayerState.gliding;
        }

        void QuitStateGliding()
        {
            //Debug.Log("QuitStateGliding");

            flatVelocity = TurnSpaceToLocal(flatVelocity);
            glideParticles.Stop();
            playerMod.UnflagAbility(eAbilityType.Glide);
            keepMomentum = true;
        }

        #endregion state change - gliding

        //*******************************************

        #region state change - dash

        void EnterStateDash()
        {
            //Debug.Log("EnterStateDash");

            playerMod.FlagAbility(eAbilityType.Dash);
            currentPlayerState = ePlayerState.dashing;
            canTurnPlayer = false;

            dashDuration = dashTime;
            dashParticles.Play();
        }

        void QuitStateDash()
        {
            //Debug.Log("QuitStateDash");

            dashDuration = 0f;
            dashTimer = dashCooldown;

            canTurnPlayer = true;
            //currentPlayerState = ePlayerState.inAir;
            playerMod.UnflagAbility(eAbilityType.Dash);
        }

        #endregion state change - dash

        //*******************************************

        #region state change - sliding

        /// <summary>
        /// Sets the state of the player to <see cref="ePlayerState.sliding"/>.
        /// </summary>
        void EnterStateSliding()
        {
            //Debug.Log("EnterStateSliding");

            currentPlayerState = ePlayerState.sliding;
        }

        void QuitStateSliding()
        {
            //Debug.Log("QuitStateSliding");

            keepMomentum = true;
        }

        #endregion state change - sliding

        //*******************************************

        #region state change - in wind tunnel

        /// <summary>
        /// Sets the state of the player to <see cref="ePlayerState.inWindTunnel"/>.
        /// </summary>
        void EnterStateInWindTunnel()
        {
            //Debug.Log("EnterStateInWindTunnel");

            currentPlayerState = ePlayerState.inWindTunnel;
        }

        /// <summary>
        /// Quits the state <see cref="ePlayerState.inWindTunnel"/>.
        /// </summary>
        void QuitStateInWindTunnel()
        {
            //Debug.Log("QuitStateInWindTunnel");

            keepMomentum = true;
        }

        #endregion state change - in wind tunnel

        //*******************************************

        #region state change - wall drift

        /// <summary>
        /// Sets the state of the player to <see cref="ePlayerState.WallDrifting"/>.
        /// </summary>
        void EnterStateWallDrift()
        {
            //Debug.Log("EnterStateWallDrift");

            currentPlayerState = ePlayerState.WallDrifting;
            playerMod.FlagAbility(eAbilityType.WallRun);

            canTurnPlayer = false;
            transform.forward = Vector3.ProjectOnPlane(-controller.collisions.currentWallNormal, transform.up);
        }

        /// <summary>
        /// Quits the state <see cref="ePlayerState.WallDrifting"/>.
        /// </summary>
        void QuitStateWallDrift()
        {
            //Debug.Log("QuitStateWallDrift");

            playerMod.UnflagAbility(eAbilityType.WallRun);

            canTurnPlayer = true;
        }

        #endregion state change - wall drift

        //*******************************************

        #region state change - wall run horizontal

        /// <summary>
        /// Sets the state of the player to <see cref="ePlayerState.WallRunningHorizontal"/>.
        /// </summary>
        void EnterStateWallRunHorizontal()
        {
            Debug.Log("EnterWallRunHorizontal");

            currentPlayerState = ePlayerState.WallRunningHorizontal;
            playerMod.FlagAbility(eAbilityType.WallRun);

            canTurnPlayer = false;

            var wallNormal = controller.collisions.currentWallNormal;
            var playerForward = transform.forward;

            var simpleWallNormal = new Vector3(wallNormal.x, 0, wallNormal.z).normalized;
            var simplePlayerForward = new Vector3(playerForward.x, 0, playerForward.z).normalized;
            var simpleCross = Vector3.Cross(simpleWallNormal, simplePlayerForward);

            transform.forward = -Vector3.Cross(wallNormal, simpleCross);
        }

        /// <summary>
        /// Quits the state <see cref="ePlayerState.WallRunningHorizontal"/>.
        /// </summary>
        void QuitStateWallRunHorizontal()
        {
            Debug.Log("QuitStateWallRunHorizontal");

            playerMod.UnflagAbility(eAbilityType.WallRun);

            canTurnPlayer = true;
        }

        #endregion state change - wall run horizontal

        //*******************************************

        #region state change - wall run vertical

        /// <summary>
        /// Sets the state of the player to <see cref="ePlayerState.WallRunningVertical"/>.
        /// </summary>
        void EnterStateWallRunVertical()
        {
            //Debug.Log("EnterStateWallRunVertical");

            currentPlayerState = ePlayerState.WallRunningVertical;
            playerMod.FlagAbility(eAbilityType.WallRun);

            canTurnPlayer = false;
            transform.forward = -controller.collisions.currentWallNormal;

            wallRunVerticalTimer = playerMod.AbilityData.WallRun.WallRunVertical.BaseDuration + velocity.magnitude * playerMod.AbilityData.WallRun.WallRunVertical.DurationMultiplier;
        }

        /// <summary>
        /// Quits the state <see cref="ePlayerState.WallRunningVertical"/>.
        /// </summary>
        void QuitStateWallRunVertical()
        {
            //Debug.Log("QuitStateWallRunVertical");

            playerMod.UnflagAbility(eAbilityType.WallRun);

            canTurnPlayer = true;
            wallRunVerticalTimer = 0f;
        }

        #endregion state change - wall run vertical

        //*******************************************

        void StartJump()
        {

        }

        //*******************************************

        #endregion state change methods

        //#############################################################################

        #region ability checks

        /// <summary>
        /// Checks whether a wall drift can be started.
        /// </summary>
        bool CheckCanStartWallDrift(bool checkPlayerForward)
        {
            //
            bool isAbilityActive = playerMod.CheckAbilityActive(eAbilityType.WallRun);

            //
            bool isTouchingWall = controller.collisions.side;

            //
            bool isFalling = velocity.y <= 0;

            //
            bool directionOK = true;
            if (checkPlayerForward)
            {
                directionOK = CheckWallRunPlayerForward();
            }

            //
            bool stickOK = CheckWallRunStick();

            //
            //			print ("isAbilityActive " + isAbilityActive + " isTouchingWall " + isTouchingWall +" isFalling " + isFalling + " directionOK " + directionOK + " stickOK " + stickOK);
            return (isAbilityActive && isTouchingWall && isFalling && directionOK && stickOK);
        }

        /// <summary>
        /// Checks whether a horizontal wall run can be started.
        /// </summary>
        /// <returns></returns>
        bool CheckCanStartWallRunHorizontal()
        {
            //
            bool isAbilityActive = playerMod.CheckAbilityActive(eAbilityType.WallRun);

            //
            bool isTouchingWall = controller.collisions.side;

            //
            bool hasDashed = (dashTimer > 0 || dashDuration > 0);

            //
            bool directionOK = CheckWallRunPlayerForward(-0.95f);

            //
            bool stickOK = CheckWallRunStick();

            //
            return (isAbilityActive && isTouchingWall && hasDashed && directionOK && stickOK);
        }

        /// <summary>
        /// Checks whether a vertical wall run can be started.
        /// </summary>
        /// <returns></returns>
        bool CheckCanStartWallRunVertical()
        {
            //
            bool isAbilityActive = playerMod.CheckAbilityActive(eAbilityType.WallRun);

            //
            bool isTouchingWall = controller.collisions.side;

            //
            bool isOnTheGround = controller.collisions.below;

            //
            bool isFastEnough = (velocity.magnitude <= playerMod.AbilityData.WallRun.WallRunVertical.MinTriggerSpeed);

            //
            bool directionOK = CheckWallRunPlayerForward();

            //
            bool stickOK = CheckWallRunStick();

            //
            return (isAbilityActive && isTouchingWall && isOnTheGround && isFastEnough && directionOK && stickOK);
        }

        /// <summary>
        /// 
        /// </summary>
        bool CheckWallRunPlayerForward(float minDotProduct = -1)
        {
            float dotProduct = Vector3.Dot(transform.forward, controller.collisions.currentWallNormal);
            return (minDotProduct <= dotProduct && dotProduct <= playerMod.AbilityData.WallRun.General.DirectionTrigger);
        }

        /// <summary>
        /// Checks whether the player is holding the left stick in position to activate or stay in wall run mode.
        /// </summary>
        bool CheckWallRunStick()
        {
            var stick = inputToCamera;
            float dotproduct = Vector3.Dot(inputToCamera, transform.forward);
            return (dotproduct >= playerMod.AbilityData.WallRun.General.StickTrigger && !leftStickAtZero);
        }

        #endregion ability checks

        //#############################################################################

        #region public utilty functions

        public void AddExternalVelocity(Vector3 newVelocity, bool worldSpace, bool framerateDependant)
        {
            if (framerateDependant)
            {
                velocity += (worldSpace ? TurnSpaceToLocal(newVelocity) : newVelocity);
            }
            else
            {
                externalVelocity += (worldSpace ? TurnSpaceToLocal(newVelocity) : newVelocity);
            }
        }

        public void SetVelocity(Vector3 newVelocity, bool worldSpace, bool framerateDependant)
        {
            velocity = (worldSpace ? TurnSpaceToLocal(newVelocity) : newVelocity) * (framerateDependant ? Time.deltaTime : 1);
        }

        public void AddWindVelocity(Vector3 newVelocity)
        {
            inWindTunnel = true;
            if (windVelocity == Vector3.zero)
            {
                windVelocity = newVelocity;
            }
            else
            {
                windVelocity = Vector3.Lerp((newVelocity), windVelocity, .5f);
            }
        }

        public void ExitWindTunnel()
        {
            inWindTunnel = false;
        }

        public Vector3 TurnLocalToSpace(Vector3 vector)
        {
            return (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, transform.up), Vector3.Cross(Vector3.up, transform.up))) * vector;
        }
        public Vector3 TurnSpaceToLocal(Vector3 vector)
        {
            return (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, transform.up), Vector3.Cross(transform.up, Vector3.up))) * vector;
        }

        public void ChangeGravityDirection(Vector3 newGravity)
        {
            gravity = newGravity.normalized;
            transform.Rotate(Vector3.Cross(transform.up, -gravity), Vector3.SignedAngle(transform.up, -gravity, Vector3.Cross(transform.up, -gravity)), Space.World);
        }

        public void ChangeGravityDirection(Vector3 newGravity, Vector3 point)
        {
            gravity = newGravity.normalized;
            transform.RotateAround(point, Vector3.Cross(transform.up, -gravity), Vector3.SignedAngle(transform.up, -gravity, Vector3.Cross(transform.up, -gravity)));
        }

        #endregion public utilty functions

        //#############################################################################
    } //end of class
} //end of namespace