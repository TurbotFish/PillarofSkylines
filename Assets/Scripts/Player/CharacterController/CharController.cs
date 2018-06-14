using Game.Model;
using Game.Player.CharacterController.Containers;
using Game.Player.CharacterController.States;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Player.CharacterController
{
    public class CharController : MonoBehaviour
    {
        //#############################################################################

        /// <summary>
        /// The rotator used to turn the camera.
        /// </summary>
        public Transform rotator;

        public Transform myCameraTransform;
        public PoS_Camera myCamera;

        //public float gameSpeed = 10f;

        /// <summary>
        /// The controller checking if there's collisions on the way.
        /// </summary>
        [HideInInspector]
        public CharacControllerRecu tempPhysicsHandler;
        public PhantomController phantomController;
        CharacControllerRecu.CollisionInfo tempCollisionInfo;

        public CharacControllerRecu.CollisionInfo CollisionInfo { get { return tempCollisionInfo; } }


        [Space(10)]
        [Header("Prefabs")]
        public GroundRise groundRisePrefab;


        /// <summary>
        /// The animator of the character.
        /// </summary>
        [HideInInspector]
        public Animator animator;
        [Space(10)]
        [Header("Animation stuff")]
        public float animationRunSpeed;
        public float animationJumpSpeed;

        public Renderer playerRenderer;

        public PlayerModel PlayerModel { get; private set; }

        public CharData CharData { get; private set; }

        public PlayerController PlayerController { get; private set; }

        public GameControl.GameController gameController;

        public Transform MyTransform { get; private set; }

        public StateMachine stateMachine;

        public ePlayerState CurrentState
        {
            get
            {
                if (stateMachine == null)
                {
                    return ePlayerState.stand;
                }
                return stateMachine.CurrentState;
            }
        }

        bool isInitialized;
        bool gamePaused = true;

        [HideInInspector]
        public bool isInsideNoRunZone;
        [HideInInspector]
        public bool createdEchoOnThisInput;

        /// <summary>
        /// This is set to true if the player is allowed to read inputs, false if not.
        /// </summary>
        public bool isHandlingInput;

        Vector3 velocity;
        Vector3 externalVelocity;
        
        PlayerInputInfo inputInfo = new PlayerInputInfo();

        public PlayerInputInfo InputInfo { get { return inputInfo; } }

        PlayerMovementInfo movementInfo = new PlayerMovementInfo();

        public PlayerMovementInfo MovementInfo { get { return movementInfo; } }
        

        public bool IsGrounded { get { return (CurrentState & (ePlayerState.move | ePlayerState.slide | ePlayerState.stand)) != 0; } }

        private List<Cloth> ClothComponentList;

        //#############################################################################

        [Space(10)]
        [Header("Particles/FX")]
        public ParticlesManager dashParticles;
        public ParticlesManager windParticles;
        public ParticlesManager glideParticles;
        public ParticleSystem aerialJumpFX;
        public ParticleSystem hoverFX;

		public FXManager fxManager;


        [Header("Sound")]
        public AudioClip jumpClip;
        [Range(0, 2)] public float volumeJump = 1f;
        public bool addRandomisationJump = false;
        public AudioClip doubleJumpClip;
        [Range(0, 2)] public float volumeDoubleJump = 1f;
        public bool addRandomisationDoubleJump = false;
        public float soundMinDistance = 10f;
        public float soundMaxDistance = 50f;
        public float clipDuration = 0f;

        //#############################################################################


        #region initialization

        public void Initialize(GameControl.GameController gameController)
        {
            tempPhysicsHandler = GetComponent<CharacControllerRecu>();
            animator = GetComponentInChildren<Animator>();
            myCamera = FindObjectOfType<PoS_Camera>();
            myCameraTransform = myCamera.transform;

            GetComponentInChildren<EchoSystem.EchoParticleSystem>().InitializeEchoParticleSystem(gameController);

            ClothComponentList = GetComponentsInChildren<Cloth>().ToList();

            this.gameController = gameController;
            PlayerModel = gameController.PlayerModel;
            CharData = Resources.Load<CharData>("ScriptableObjects/CharData");
            PlayerController = gameController.PlayerController;

            MyTransform = transform;

            //*******************************************

            stateMachine = new StateMachine(this);

            stateMachine.RegisterAbility(ePlayerState.dash, AbilityType.Dash);
            stateMachine.RegisterAbility(ePlayerState.glide, AbilityType.Glide);
            stateMachine.RegisterAbility(ePlayerState.wallRun, AbilityType.WallRun);
            stateMachine.RegisterAbility(ePlayerState.hover, AbilityType.Hover);
            stateMachine.RegisterAbility(ePlayerState.jetpack, AbilityType.Jetpack);
            stateMachine.RegisterAbility(ePlayerState.graviswap, AbilityType.Graviswap);
            stateMachine.RegisterAbility(ePlayerState.phantom, AbilityType.Phantom);

            stateMachine.jetpackFuel = CharData.Jetpack.MaxFuel;

            stateMachine.ChangeState(new AirState(this, stateMachine, AirState.eAirStateMode.fall));

            //*******************************************

            Utilities.EventManager.OnMenuSwitchedEvent += OnMenuSwitchedEventHandler;
            Utilities.EventManager.TeleportPlayerEvent += OnTeleportPlayerEventHandler;
            Utilities.EventManager.GamePausedEvent += OnGamePausedEventHandler;
            Utilities.EventManager.SceneChangedEvent += OnSceneChangedEventHandler;
            Utilities.EventManager.PillarMarkStateChangedEvent += OnPillarMarkStateChanged;

            isInitialized = true;
            isHandlingInput = true;
        }

        #endregion initialization

        //#############################################################################

        #region monobehaviour methods

        // Use this for initialization
        void Start()
        {
        }

        void OnDestroy()
        {
            Utilities.EventManager.OnMenuSwitchedEvent -= OnMenuSwitchedEventHandler;
            Utilities.EventManager.TeleportPlayerEvent -= OnTeleportPlayerEventHandler;
            Utilities.EventManager.GamePausedEvent -= OnGamePausedEventHandler;
        }

        #endregion monobehaviour methods

        //#############################################################################

        #region update

        // Update is called once per frame
        void Update()
        {
            if (!isInitialized || gamePaused)
            {
                return;
            }
            

            if (Input.GetKeyDown(KeyCode.F6))
            {
                this.ChangeGravityDirection(Vector3.left);
            }

            //*******************************************

            movementInfo.position = MyTransform.position;
            movementInfo.forward = MyTransform.forward;
            movementInfo.up = MyTransform.up;
            movementInfo.side = MyTransform.right;
            movementInfo.velocity = velocity;

            //*******************************************
            //handling input
            bool sprintDownLastFrame = inputInfo.sprintButton;
            bool glideDownLastFrame = inputInfo.glideButton;
            bool echoUpLastFrame = inputInfo.echoButtonUp;
            inputInfo.Reset();
            if (isHandlingInput)
            {
                
                float stickH = Input.GetAxisRaw("Horizontal");
                float stickV = Input.GetAxisRaw("Vertical");

                inputInfo.leftStickRaw = new Vector3(stickH, 0, stickV);

                if (inputInfo.leftStickRaw.magnitude < CharData.General.StickDeadMaxVal)
                {
                    inputInfo.leftStickAtZero = true;
                }
                else
                {
                    inputInfo.leftStickAtZero = false;
                }

                var toCameraAngle = Quaternion.AngleAxis(Vector3.Angle(transform.up, Vector3.up), (Vector3.Cross(transform.up, Vector3.up) != Vector3.zero ? Vector3.Cross(transform.up, Vector3.up) : Vector3.forward));
                inputInfo.leftStickToCamera = toCameraAngle * (rotator.right * stickH + rotator.forward * stickV);

                var toSlopeAngle = Quaternion.AngleAxis(Vector3.Angle(transform.up, tempCollisionInfo.currentGroundNormal), Vector3.Cross(transform.up, tempCollisionInfo.currentGroundNormal));
                inputInfo.leftStickToSlope = toSlopeAngle * (rotator.right * stickH + rotator.forward * stickV);
                inputInfo.leftStickToSlope = toCameraAngle * inputInfo.leftStickToSlope;


                inputInfo.dashButton = Input.GetButton("Dash");
                inputInfo.dashButtonDown = Input.GetButtonDown("Dash");
                inputInfo.dashButtonUp = Input.GetButtonUp("Dash");

                inputInfo.jumpButton = Input.GetButton("Jump");
                inputInfo.jumpButtonDown = Input.GetButtonDown("Jump");
                inputInfo.jumpButtonUp = Input.GetButtonUp("Jump");

                inputInfo.sprintButton = (Input.GetAxis("Right Trigger") > .9f) || Input.GetButton("Sprint") && tempPhysicsHandler.currentGravifloor == null;
                inputInfo.sprintButtonDown = (inputInfo.sprintButton && !sprintDownLastFrame) || Input.GetButtonDown("Sprint") && tempPhysicsHandler.currentGravifloor == null;
                inputInfo.sprintButtonUp = (!inputInfo.sprintButton && sprintDownLastFrame) || Input.GetButtonUp("Sprint") && tempPhysicsHandler.currentGravifloor == null;

                inputInfo.glideButton = (Input.GetAxis("Left Trigger") > .9f) || Input.GetButton("Sprint");
                inputInfo.glideButtonDown = (inputInfo.glideButton && !glideDownLastFrame) || Input.GetButtonDown("Sprint");
                inputInfo.glideButtonUp = (!inputInfo.glideButton && glideDownLastFrame) || Input.GetButtonUp("Sprint");

                inputInfo.echoButton = Input.GetButton("Interact");
                inputInfo.echoButtonDown = Input.GetButtonDown("Interact");
                inputInfo.echoButtonUp = Input.GetButtonUp("Interact");

                /*
                inputInfo.jetpackButton = Input.GetButton("Jetpack");
                inputInfo.jetpackButtonDown = Input.GetButtonDown("Jetpack");
                inputInfo.jetpackButtonUp = Input.GetButtonUp("Jetpack");*/

                inputInfo.rightStickButtonDown = Input.GetButtonDown("RightStickClick");
                
                if (inputInfo.echoButton)
                {
                    inputInfo.echoButtonTimePressed += Time.deltaTime;
                }
                if (echoUpLastFrame)
                {
                    inputInfo.ResetTimeEcho();
                }

                if (Input.GetButtonDown("GroundRise"))
                {
                    CreateGroundRise();
                }
                
            }
            stateMachine.HandleInput();

            //*******************************************
            //state update           
            //call state update
            var stateReturn = stateMachine.Update(Time.deltaTime);

            //handling return
            bool canTurnPlayer = stateReturn.CanTurnPlayerSet ? stateReturn.CanTurnPlayer : true;
            float transitionSpeed = stateReturn.TransitionSpeedSet ? stateReturn.TransitionSpeed : CharData.General.TransitionSpeed;
            float maxSpeed = stateReturn.MaxSpeedSet ? stateReturn.MaxSpeed : CharData.General.MaxSpeed;
            var acceleration = stateReturn.AccelerationSet ? stateReturn.Acceleration : velocity;


            if (stateReturn.PlayerForwardSet)
            {
                //Debug.Log("forward is : " + stateReturn.PlayerForward);
                if (stateReturn.PlayerForward != Vector3.zero)
                {
                    MyTransform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(stateReturn.PlayerForward, MyTransform.up), MyTransform.up);
                }
                else
                {
                    print("Trying to apply zero as player forward >:c");
                    MyTransform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(MyTransform.forward, MyTransform.up), MyTransform.up);
                }
            }

            if (stateReturn.PlayerUpSet)
            {
                Debug.LogError("Should not be used for now!");
                //MyTransform.up = stateReturn.PlayerUp;
            }

            //computing new velocity
            //var newVelocity = velocity * (1 - Time.deltaTime * transitionSpeed) + (acceleration + externalVelocity) * (Time.deltaTime * transitionSpeed);
            if (stateReturn.resetVerticalVelocity)
            {
                ResetVerticalVelocity();
            }

            float tempVertical;
            Vector3 newVelocity = new Vector3();


            if (stateReturn.keepVerticalMovement)
            {
                tempVertical = velocity.y;
                newVelocity = Vector3.Lerp(Vector3.ProjectOnPlane(velocity, Vector3.up), acceleration, Time.deltaTime * transitionSpeed);
                newVelocity = new Vector3(newVelocity.x, tempVertical, newVelocity.z);
            }
            else
            {
                newVelocity = Vector3.Lerp(velocity, acceleration, Time.deltaTime * transitionSpeed);
            }

            //adding gravity
            if (!stateReturn.IgnoreGravity)
            {
                newVelocity += Vector3.down * (CharData.General.GravityStrength * (stateReturn.GravityMultiplierSet? stateReturn.GravityMultiplier : 1) * Time.deltaTime);
            }

            //clamping speed
            if (newVelocity.magnitude >= maxSpeed)
            {
                newVelocity = newVelocity.normalized * maxSpeed;

                //Debug.LogFormat("clamped velocity: {0}", newVelocity);
            }

            
            newVelocity += externalVelocity;

            //*******************************************
            //physics update
            //Debug.Log("velocity before : " + newVelocity);
            var turnedVelocity = TurnLocalToSpace(newVelocity);
            //Debug.Log("velocity turned : " + turnedVelocity);
            Vector3 lastPositionDelta;

            /*
            if (stateMachine.CurrentState == ePlayerState.glide)
            {
                lastPositionDelta = tempPhysicsHandler.Move(newVelocity * Time.deltaTime);
            }
            else
            {*/
                lastPositionDelta = tempPhysicsHandler.Move(turnedVelocity * Time.deltaTime);
            //}
            velocity = lastPositionDelta / Time.deltaTime;
            //Debug.Log("velocity after : " + velocity);


            externalVelocity = Vector3.zero;
            tempCollisionInfo = tempPhysicsHandler.collisions;

            //*******************************************
            //moving player

            /*
             *  moving the player is currently handled by the (temp!) physics handler
             */

            //*******************************************
            //turning player

            if (canTurnPlayer && !inputInfo.leftStickAtZero)
            {
                if (stateReturn.RotationSet)
                {
                    MyTransform.Rotate(MyTransform.up, stateReturn.Rotation, Space.World);
                }
                else if(!inputInfo.leftStickAtZero)
                {
                    Vector3 to = Vector3.ProjectOnPlane(TurnLocalToSpace(inputInfo.leftStickToCamera), MyTransform.up);
                    float angle = Mathf.Lerp(0f, Vector3.SignedAngle(MyTransform.forward, to, MyTransform.up), CharData.General.TurnSpeed * Time.deltaTime);

                    MyTransform.Rotate(MyTransform.up, angle, Space.World);
                }
            }

            //*******************************************

            #region update animator
            //----------OLD--------------
            //            float keyHalf = 0.5f;
            //            float m_RunCycleLegOffset = 0.2f;
            //            float forward = lastPositionDelta.magnitude / (8 * Time.deltaTime);
            //            if (forward <= 0.2f)
            //            {
            //                forward = 0;
            //            }
            //            float turn = (inputInfo.leftStickAtZero ? 0f : Mathf.Lerp(0f, Vector3.SignedAngle(transform.forward, Vector3.ProjectOnPlane(TurnLocalToSpace(inputInfo.leftStickToCamera), transform.up), transform.up), CharData.General.TurnSpeed * Time.deltaTime) / 7f);
            //            if (!canTurnPlayer)
            //            {
            //                turn = 0;
            //            }
            //
            //            animator.SetBool("OnGround", tempCollisionInfo.below);
            //            animator.SetFloat("Forward", forward);
            //            animator.SetFloat("Turn", turn);
            //            animator.SetFloat("Jump", turnedVelocity.y / 5);
            //            float runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
            //            float jumpLeg = (runCycle < keyHalf ? 1 : -1) * inputInfo.leftStickRaw.magnitude;
            //            if (tempCollisionInfo.below)
            //            {
            //                animator.SetFloat("JumpLeg", jumpLeg);
            //            }
            //
            //            //windParticles.SetVelocity(velocity);
            //            //glideParticles.SetVelocity(velocity);
            //-------FIN OLD-----------

            float turn = (inputInfo.leftStickAtZero ? 0f : Mathf.Lerp(0f, Vector3.SignedAngle(transform.forward, Vector3.ProjectOnPlane(TurnLocalToSpace(inputInfo.leftStickToCamera), transform.up), transform.up), CharData.General.TurnSpeed * Time.deltaTime) / 7f);
            if (!canTurnPlayer)
            {
                turn = 0;
            }
            animator.SetFloat("Turn", turn);
            animator.SetBool("OnGround", (tempCollisionInfo.below && Vector3.Angle(tempCollisionInfo.currentGroundNormal, movementInfo.up) < CharData.General.MinWallAngle) || stateMachine.CurrentState == ePlayerState.hover);
            animator.SetFloat("Speed", Vector3.ProjectOnPlane(velocity, Vector3.up).magnitude / animationRunSpeed);
            //animator.SetFloat("Turn", turn);
            animator.SetFloat("VerticalSpeed", velocity.y / animationJumpSpeed);

            windParticles.SetVelocity(velocity);
            //glideParticles.SetVelocity(velocity);

            #endregion update animator

            //*******************************************
        }

        #endregion update

        //#############################################################################

        #region event handlers

        void OnMenuSwitchedEventHandler(object sender, Utilities.EventManager.OnMenuSwitchedEventArgs args)
        {
            if (args.NewUiState == UI.MenuType.HUD)
            {
                isHandlingInput = true;
            }
            else
            {
                isHandlingInput = false;
            }
        }

        void OnTeleportPlayerEventHandler(object sender, Utilities.EventManager.TeleportPlayerEventArgs args)
        {
            MyTransform.position = args.Position;

            foreach(var cloth_component in ClothComponentList)
            {
                cloth_component.ClearTransformMotion();
            }

            if (args.TakeRotation)
            {
                velocity = args.Rotation * Quaternion.Inverse(MyTransform.rotation) * velocity;
                MyTransform.rotation = args.Rotation;
            }

            if (args.IsNewScene)
            {
                {
                    velocity = Vector3.zero;
                    stateMachine.ChangeState(new AirState(this, stateMachine, AirState.eAirStateMode.fall));
                    ChangeGravityDirection(Vector3.down);

                }
            }
        }

        private void OnGamePausedEventHandler(object sender, Utilities.EventManager.GamePausedEventArgs args)
        {
            gamePaused = args.PauseActive;

            if (gamePaused)
            {
                animator.SetFloat("Turn", 0);
                animator.SetBool("OnGround", true);
                animator.SetFloat("Speed", 0);
                animator.SetFloat("VerticalSpeed", 0);
            }
        }

        private void OnSceneChangedEventHandler(object sender, Utilities.EventManager.SceneChangedEventArgs args)
        {
            isHandlingInput = true;
        }

        private void OnPillarMarkStateChanged(object sender, Utilities.EventManager.PillarMarkStateChangedEventArgs args)
        {
            playerRenderer.sharedMaterial.SetFloat("_Mark_Apparition", PlayerModel.GetActivePillarMarkCount());
        }

        #endregion event handlers

        //#############################################################################

        #region utility methods

        public Vector3 TurnLocalToSpace(Vector3 vector)
        {
            return (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, MyTransform.up), (Vector3.Cross(Vector3.up, MyTransform.up) != Vector3.zero ? Vector3.Cross(Vector3.up, MyTransform.up) : Vector3.forward))) * vector;
        }

        public Vector3 TurnSpaceToLocal(Vector3 vector)
        {
            return (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, MyTransform.up), (Vector3.Cross(MyTransform.up, Vector3.up) != Vector3.zero ? Vector3.Cross(MyTransform.up, Vector3.up) : Vector3.forward))) * vector;
        }

        public void ResetEchoInputTime()
        {
            inputInfo.ResetTimeEcho();
        }

        #endregion utility methods

        //#############################################################################

        #region cancer

        void CreateGroundRise()
        {
            if (PlayerModel.CheckAbilityActive(AbilityType.GroundRise))
            {
                GroundRise grRise = Instantiate(groundRisePrefab);
                grRise.Initialize(MyTransform.position, MyTransform.up, this, velocity);
            }
        }


        public void SetHandlingInput(bool value)
        {
            isHandlingInput = value;
        }
        public void KillPillarEye()
        {
            isHandlingInput = false;
            animator.SetTrigger("Kill Philippe");
        }


        public void ResetVerticalVelocity(bool onlyDownVelocity = false)
        {
            if (!onlyDownVelocity || velocity.y <= 0f)
                velocity.y = 0;
        }

        public void AddExternalVelocity(Vector3 newVelocity, bool worldSpace, bool lerped)
        {
            if (lerped)
            {
                velocity += (worldSpace ? TurnSpaceToLocal(newVelocity) : newVelocity);
            }
            else
            {
                newVelocity = (worldSpace ? TurnSpaceToLocal(newVelocity) : newVelocity);
                externalVelocity += newVelocity;
            }
        }

        public void ImmediateMovement(Vector3 newVelocity, bool worldSpace, bool addToVelocity = false)
        {
            Vector3 temp = (!worldSpace ? TurnLocalToSpace(newVelocity) : newVelocity);
            newVelocity = tempPhysicsHandler.Move(temp);
            if (addToVelocity)
                velocity += newVelocity/Time.deltaTime;
        }
     
        public void SetVelocity(Vector3 newVelocity, bool worldSpace)
        {
            velocity = (worldSpace ? TurnSpaceToLocal(newVelocity) : newVelocity);
        }

        public void ChangeGravityDirection(Vector3 newGravity)
        {
            MyTransform.Rotate(Vector3.Cross(MyTransform.up, -newGravity), Vector3.SignedAngle(MyTransform.up, -newGravity, Vector3.Cross(MyTransform.up, -newGravity)), Space.World);
            myCamera.UpdateGravity();
        }

        public void ChangeGravityDirection(Vector3 newGravity, Vector3 point)
        {
            MyTransform.RotateAround(point, Vector3.Cross(MyTransform.up, -newGravity), Vector3.SignedAngle(MyTransform.up, -newGravity, Vector3.Cross(MyTransform.up, -newGravity)));
            myCamera.UpdateGravity();
        }

        #endregion cancer

        //#############################################################################
    }
}
//end of namespace