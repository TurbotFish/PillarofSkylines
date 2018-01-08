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

        //#############################################################################

        /// <summary>
        /// The controller checking if there's collisions on the way.
        /// </summary>
        CharacControllerRecu tempPhysicsHandler;
        CharacControllerRecu.CollisionInfo tempCollisionInfo;
        public CharacControllerRecu.CollisionInfo CollisionInfo { get { return tempCollisionInfo; } }

        /// <summary>
        /// The animator of the character.
        /// </summary>
        Animator animator;

        //#############################################################################       

        public PlayerModel PlayerModel { get; private set; }
        public CharData CharData { get; private set; }
        public PlayerController PlayerController { get; private set; }

        Transform myTransform;
        StateMachine stateMachine;
        public ePlayerState CurrentState { get { if (stateMachine != null) return stateMachine.CurrentState; return ePlayerState.empty; } }

        bool isInitialized;
        bool isHandlingInput;

        Vector3 velocity;
        Vector3 externalVelocity;
        Vector3 lastPositionDelta;




        Vector3 gravityDirection = -Vector3.up;
        public Vector3 GravityDirection { get { return gravityDirection; } }

        List<WindTunnelPart> windTunnelPartList = new List<WindTunnelPart>();
        public List<WindTunnelPart> WindTunnelPartList { get { return new List<WindTunnelPart>(windTunnelPartList); } }

        PlayerInputInfo inputInfo = new PlayerInputInfo();
        public PlayerInputInfo InputInfo { get { return inputInfo; } }

        PlayerMovementInfo movementInfo = new PlayerMovementInfo();
        public PlayerMovementInfo MovementInfo { get { return movementInfo; } }

        //#############################################################################

        #region initialization

        public void Initialize(GameControl.IGameControllerBase gameController)
        {
            tempPhysicsHandler = GetComponent<CharacControllerRecu>();
            animator = GetComponentInChildren<Animator>();

            PlayerModel = gameController.PlayerModel;
            CharData = Resources.Load<CharData>("ScriptableObjects/CharData");
            PlayerController = gameController.PlayerController;

            myTransform = transform;
            stateMachine = new StateMachine(this);

            //*******************************************

            stateMachine.RegisterAbility(ePlayerState.dash, eAbilityType.Dash);

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

            //*******************************************

            movementInfo.position = myTransform.position;
            movementInfo.forward = myTransform.forward;
            movementInfo.up = myTransform.up;
            movementInfo.side = myTransform.right;
            movementInfo.velocity = velocity;

            //*******************************************
            //handling input

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

                var toCameraAngle = Quaternion.AngleAxis(Vector3.Angle(transform.up, Vector3.up), Vector3.Cross(transform.up, Vector3.up));
                inputInfo.leftStickToCamera = toCameraAngle * (rotator.right * stickH + rotator.forward * stickV);

                var toSlopeAngle = Quaternion.AngleAxis(Vector3.Angle(transform.up, tempCollisionInfo.currentGroundNormal), Vector3.Cross(transform.up, tempCollisionInfo.currentGroundNormal));
                inputInfo.leftStickToSlope = toSlopeAngle * inputInfo.leftStickToCamera;

                inputInfo.dashButton = Input.GetButton("Dash");
                inputInfo.dashButtonDown = Input.GetButtonDown("Dash");
                inputInfo.dashButtonUp = Input.GetButtonUp("Dash");

                inputInfo.jumpButton = Input.GetButton("Jump");
                inputInfo.jumpButtonDown = Input.GetButtonDown("Jump");
                inputInfo.jumpButtonUp = Input.GetButtonUp("Jump");

                inputInfo.sprintButton = Input.GetButton("Sprint");
                inputInfo.sprintButtonDown = Input.GetButtonDown("Sprint");
                inputInfo.sprintButtonUp = Input.GetButtonUp("Sprint");

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
                myTransform.forward = stateReturn.PlayerForward;
            }

            if (stateReturn.PlayerUpSet)
            {
                myTransform.up = stateReturn.PlayerUp;
            }

            //Debug.Log("================");
            //Debug.LogFormat("initial velocity: {0}", velocity);
            //Debug.LogFormat("desiredVelocity={0}", stateReturn.DesiredVelocity.magnitude.ToString());

            //computing new velocity
            var newVelocity = velocity * (1 - Time.deltaTime * transitionSpeed) + (acceleration + externalVelocity) * (Time.deltaTime * transitionSpeed);

            //Debug.LogFormat("new velocity: {0}", newVelocity);

            //adding gravity
            if (!stateReturn.IgnoreGravity)
            {
                newVelocity += gravityDirection * (CharData.General.GravityStrength * Time.deltaTime);
            }

            //Debug.LogFormat("after gravity: {0}", newVelocity);

            //clamping speed
            if (newVelocity.magnitude >= maxSpeed)
            {
                //newVelocity = newVelocity.normalized * maxSpeed;

                //Debug.LogFormat("clamped velocity: {0}", newVelocity);
            }

            //
            velocity = newVelocity;

            //*******************************************
            //physics update

            var turnedVelocity = TurnLocalToSpace(newVelocity);

            if (stateMachine.CurrentState == ePlayerState.glide)
            {
                lastPositionDelta = tempPhysicsHandler.Move(newVelocity * Time.deltaTime);
            }
            else
            {
                lastPositionDelta = tempPhysicsHandler.Move(turnedVelocity * Time.deltaTime);
            }
            //Debug.LogFormat("after physics: {0}", newVelocity);

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
                if (CurrentState != ePlayerState.glide)
                {
                    myTransform.Rotate(
                        myTransform.up,
                        Mathf.Lerp(
                            0f,
                            Vector3.SignedAngle(myTransform.forward, Vector3.ProjectOnPlane(TurnLocalToSpace(inputInfo.leftStickToCamera), myTransform.up), myTransform.up),
                            CharData.General.TurnSpeed * Time.deltaTime
                        ),
                        Space.World);
                }
                else
                {
                    myTransform.Rotate(myTransform.up, CharData.Glide.HorizontalAngle, Space.World);
                }
            }

            //*******************************************

            #region update animator
            float keyHalf = 0.5f;
            float m_RunCycleLegOffset = 0.2f;
            float forward = lastPositionDelta.magnitude / (8 * Time.deltaTime);
            if (forward <= 0.2f)
            {
                forward = 0;
            }
            float turn = (inputInfo.leftStickAtZero ? 0f : Mathf.Lerp(0f, Vector3.SignedAngle(transform.forward, Vector3.ProjectOnPlane(TurnLocalToSpace(inputInfo.leftStickToCamera), transform.up), transform.up), CharData.General.TurnSpeed * Time.deltaTime) / 7f);
            if (!canTurnPlayer)
            {
                turn = 0;
            }

            animator.SetBool("OnGround", tempCollisionInfo.below);
            animator.SetFloat("Forward", forward);
            animator.SetFloat("Turn", turn);
            animator.SetFloat("Jump", turnedVelocity.y / 5);
            float runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
            float jumpLeg = (runCycle < keyHalf ? 1 : -1) * inputInfo.leftStickRaw.magnitude;
            if (tempCollisionInfo.below)
            {
                animator.SetFloat("JumpLeg", jumpLeg);
            }

            //windParticles.SetVelocity(velocity);
            //glideParticles.SetVelocity(velocity);
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
            myTransform.position = args.Position;

            if (args.IsNewScene)
            {
                myTransform.rotation = args.Rotation;
                velocity = Vector3.zero;
                stateMachine.ChangeState(new FallState(this, stateMachine));
                ChangeGravityDirection(Vector3.down);
            }
        }

        void OnWindTunnelPartEnteredEventHandler(object sender, Utilities.EventManager.WindTunnelPartEnteredEventArgs args)
        {
            if (!windTunnelPartList.Contains(args.WindTunnelPart))
            {
                windTunnelPartList.Add(args.WindTunnelPart);
            }

            //if (stateMachine.CurrentState != ePlayerState.windTunnel)
            //{
            //    stateMachine.ChangeState(new WindTunnelState(this, stateMachine));
            //}
        }

        void OnWindTunnelPartExitedEventHandler(object sender, Utilities.EventManager.WindTunnelPartExitedEventArgs args)
        {
            windTunnelPartList.Remove(args.WindTunnelPart);
        }

        #endregion event handlers

        //#############################################################################

        #region utility methods

        Vector3 TurnLocalToSpace(Vector3 vector)
        {
            return (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, myTransform.up), Vector3.Cross(Vector3.up, myTransform.up))) * vector;
        }

        Vector3 TurnSpaceToLocal(Vector3 vector)
        {
            return (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, myTransform.up), Vector3.Cross(myTransform.up, Vector3.up))) * vector;
        }

        #endregion utility methods

        //#############################################################################

        #region cancer

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

        public void ChangeGravityDirection(Vector3 newGravity)
        {
            gravityDirection = newGravity.normalized;
            myTransform.Rotate(Vector3.Cross(myTransform.up, -gravityDirection), Vector3.SignedAngle(myTransform.up, -gravityDirection, Vector3.Cross(myTransform.up, -gravityDirection)), Space.World);
        }

        public void ChangeGravityDirection(Vector3 newGravity, Vector3 point)
        {
            gravityDirection = newGravity.normalized;
            myTransform.RotateAround(point, Vector3.Cross(myTransform.up, -gravityDirection), Vector3.SignedAngle(myTransform.up, -gravityDirection, Vector3.Cross(myTransform.up, -gravityDirection)));
        }

        #endregion cancer

        //#############################################################################
    }
} //end of namespace