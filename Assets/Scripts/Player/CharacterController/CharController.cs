using Game.Player.CharacterController.Containers;
using Game.Player.CharacterController.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController
{
    public class CharController : MonoBehaviour
    {
        //#############################################################################

        /// <summary>
        /// The rotator used to turn the camera.
        /// </summary>
        [SerializeField]
        Transform rotator;
        [SerializeField]
        public Transform myCameraTransform;
        [SerializeField]
        public PoS_Camera myCamera;

        /// <summary>
        /// The controller checking if there's collisions on the way.
        /// </summary>
        [HideInInspector]
        public CharacControllerRecu tempPhysicsHandler;
        CharacControllerRecu.CollisionInfo tempCollisionInfo;

        public CharacControllerRecu.CollisionInfo CollisionInfo { get { return tempCollisionInfo; } }

        /// <summary>
        /// The animator of the character.
        /// </summary>
        [HideInInspector]
        public Animator animator;
        [Space(10)]
        [Header("Animation stuff")]
        public float animationRunSpeed;
        public float animationJumpSpeed;

        public PlayerModel PlayerModel { get; private set; }

        public CharData CharData { get; private set; }

        public PlayerController PlayerController { get; private set; }

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

        /// <summary>
        /// This is set to false if the player has opened a menu, true otherwise.
        /// </summary>
        bool isHandlingInput;

        Vector3 velocity;
        Vector3 externalVelocity;

        List<WindTunnelPart> windTunnelPartList = new List<WindTunnelPart>();

        public List<WindTunnelPart> WindTunnelPartList { get { return new List<WindTunnelPart>(windTunnelPartList); } }
        
        PlayerInputInfo inputInfo = new PlayerInputInfo();

        public PlayerInputInfo InputInfo { get { return inputInfo; } }

        PlayerMovementInfo movementInfo = new PlayerMovementInfo();

        public PlayerMovementInfo MovementInfo { get { return movementInfo; } }

        //#############################################################################

        [Space(10)]
        [Header("Particles/FX")]
        public ParticlesManager dashParticles;
        public ParticlesManager windParticles;
        public ParticlesManager glideParticles;
        public ParticleSystem aerialJumpFX;

        //#############################################################################


        #region initialization

        public void Initialize(GameControl.IGameControllerBase gameController)
        {
            tempPhysicsHandler = GetComponent<CharacControllerRecu>();
            animator = GetComponentInChildren<Animator>();
            myCamera = FindObjectOfType<PoS_Camera>();
            myCameraTransform = myCamera.transform;


            PlayerModel = gameController.PlayerModel;
            CharData = Resources.Load<CharData>("ScriptableObjects/CharData");
            PlayerController = gameController.PlayerController;

            MyTransform = transform;

            //*******************************************

            stateMachine = new StateMachine(this);

            stateMachine.RegisterAbility(ePlayerState.dash, eAbilityType.Dash);
            stateMachine.RegisterAbility(ePlayerState.glide, eAbilityType.Glide);
            stateMachine.RegisterAbility(ePlayerState.wallDrift, eAbilityType.WallRun);
            stateMachine.RegisterAbility(ePlayerState.wallClimb, eAbilityType.WallRun);
            stateMachine.RegisterAbility(ePlayerState.wallRun, eAbilityType.WallRun);
            stateMachine.RegisterAbility(ePlayerState.hover, eAbilityType.Hover);

            stateMachine.ChangeState(new AirState(this, stateMachine, AirState.eAirStateMode.fall));

            //*******************************************

            Utilities.EventManager.OnMenuSwitchedEvent += OnMenuSwitchedEventHandler;
            Utilities.EventManager.TeleportPlayerEvent += OnTeleportPlayerEventHandler;
            Utilities.EventManager.WindTunnelPartEnteredEvent += OnWindTunnelPartEnteredEventHandler;
            Utilities.EventManager.WindTunnelExitedEvent += OnWindTunnelPartExitedEventHandler;

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
            Utilities.EventManager.WindTunnelPartEnteredEvent -= OnWindTunnelPartEnteredEventHandler;
            Utilities.EventManager.WindTunnelExitedEvent -= OnWindTunnelPartExitedEventHandler;
        }

        #endregion monobehaviour methods

        //#############################################################################

        #region update

        // Update is called once per frame
        void Update()
        {
            if (!isInitialized)
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
           
            inputInfo.Reset();

            if (isHandlingInput)
            {
                bool sprintDownLastFrame = inputInfo.sprintButton;

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

                var toCameraAngle = Quaternion.AngleAxis(Vector3.Angle(transform.up, Vector3.up), Vector3.Cross(transform.up, Vector3.up));
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

                inputInfo.sprintButton = (Input.GetAxis("Left Trigger") > .9f) || Input.GetButton("Sprint");
                inputInfo.sprintButtonDown = (inputInfo.sprintButton && !sprintDownLastFrame) || Input.GetButtonDown("Sprint");
                inputInfo.sprintButtonUp = (!inputInfo.sprintButton && sprintDownLastFrame) || Input.GetButtonUp("Sprint");

                //
                stateMachine.HandleInput();
            }

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
                Debug.Log("forward is : " + stateReturn.PlayerForward);
                MyTransform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(stateReturn.PlayerForward, MyTransform.up), MyTransform.up);
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
                velocity.y = 0;
            }

            Vector3 tempVertical = new Vector3();
            Vector3 newVelocity = new Vector3();


            if (stateReturn.keepVerticalMovement)
            {
                tempVertical = new Vector3(0, velocity.y, 0);
                newVelocity = Vector3.Lerp(Vector3.ProjectOnPlane(velocity, Vector3.up), acceleration, Time.deltaTime * transitionSpeed);
                newVelocity += tempVertical;
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

            //
            newVelocity += externalVelocity;

            //*******************************************
            //physics update

            var turnedVelocity = TurnLocalToSpace(newVelocity);
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
                    Debug.LogError("aaa");
                }
                else if(!inputInfo.leftStickAtZero)
                {
                    Vector3 to = Vector3.ProjectOnPlane(TurnLocalToSpace(inputInfo.leftStickToCamera), MyTransform.up);
                    float angle = Mathf.Lerp(0f, Vector3.SignedAngle(MyTransform.forward, to, MyTransform.up), CharData.General.TurnSpeed * Time.deltaTime);

                    MyTransform.Rotate(MyTransform.up, angle, Space.World);
                    Debug.LogErrorFormat("bbb, angle={0}", angle);
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
            animator.SetBool("OnGround", tempCollisionInfo.below || stateMachine.CurrentState == ePlayerState.hover);
            animator.SetFloat("Speed", Vector3.ProjectOnPlane(velocity, Vector3.up).magnitude / animationRunSpeed);
            //animator.SetFloat("Turn", turn);
            animator.SetFloat("VerticalSpeed", velocity.y / animationJumpSpeed);

            windParticles.SetVelocity(velocity);
            glideParticles.SetVelocity(velocity);

            #endregion update animator

            //*******************************************
        }

        #endregion update

        //#############################################################################

        #region event handlers

        void OnMenuSwitchedEventHandler(object sender, Utilities.EventManager.OnMenuSwitchedEventArgs args)
        {
            if (args.NewUiState == UI.eUiState.HUD)
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

        void OnWindTunnelPartEnteredEventHandler(object sender, Utilities.EventManager.WindTunnelPartEnteredEventArgs args)
        {
            if (!windTunnelPartList.Contains(args.WindTunnelPart))
            {
                print("eventreceived");
                windTunnelPartList.Add(args.WindTunnelPart);
            }
        }

        void OnWindTunnelPartExitedEventHandler(object sender, Utilities.EventManager.WindTunnelPartExitedEventArgs args)
        {
            print("partremoved");
            windTunnelPartList.Remove(args.WindTunnelPart);
        }

        #endregion event handlers

        //#############################################################################

        #region utility methods

        public Vector3 TurnLocalToSpace(Vector3 vector)
        {
            return (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, MyTransform.up), Vector3.Cross(Vector3.up, MyTransform.up))) * vector;
        }

        public Vector3 TurnSpaceToLocal(Vector3 vector)
        {
            return (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, MyTransform.up), Vector3.Cross(MyTransform.up, Vector3.up))) * vector;
        }

        #endregion utility methods

        //#############################################################################

        #region cancer

        public void AddExternalVelocity(Vector3 newVelocity, bool worldSpace, bool lerped)
        {
            if (lerped)
            {
                velocity += (worldSpace ? TurnSpaceToLocal(newVelocity) : newVelocity);
            }
            else
            {
                externalVelocity += (worldSpace ? TurnSpaceToLocal(newVelocity) : newVelocity);
            }
        }

        public void ImmediateMovement(Vector3 newVelocity, bool worldSpace)
        {
            tempPhysicsHandler.Move((worldSpace ? TurnSpaceToLocal(newVelocity) : newVelocity));
        }
     
        public void SetVelocity(Vector3 newVelocity, bool worldSpace)
        {
            velocity = (worldSpace ? TurnSpaceToLocal(newVelocity) : newVelocity);
        }

        public void ChangeGravityDirection(Vector3 newGravity)
        {
            MyTransform.Rotate(Vector3.Cross(MyTransform.up, -newGravity), Vector3.SignedAngle(MyTransform.up, -newGravity, Vector3.Cross(MyTransform.up, -newGravity)), Space.World);
        }

        public void ChangeGravityDirection(Vector3 newGravity, Vector3 point)
        {
            MyTransform.RotateAround(point, Vector3.Cross(MyTransform.up, -newGravity), Vector3.SignedAngle(MyTransform.up, -newGravity, Vector3.Cross(MyTransform.up, -newGravity)));
        }

        #endregion cancer

        //#############################################################################
    }
}
//end of namespace