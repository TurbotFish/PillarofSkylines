using Game.Player.CharacterController.Containers;
using Game.Player.CharacterController.EnterArgs;
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

        /// <summary>
        /// The animator of the character.
        /// </summary>
        Animator animator;

        //#############################################################################       

        PlayerModel playerModel;
        public CharData CharData { get; private set; }

        Transform myTransform;
        StateMachine stateMachine;
        public ePlayerState CurrentState { get { if (stateMachine != null) return stateMachine.CurrentState; return ePlayerState.empty; } }

        bool isInitialized;
        bool isHandlingInput;
        bool canTurnPlayer;

        [System.Obsolete]
        public bool CanTurnPlayer { get { return canTurnPlayer; } }

        Vector3 velocity;

        [System.Obsolete]
        public Vector3 Velocity { get { return velocity; } }

        Vector3 externalVelocity;

        Vector3 gravityDirection = -Vector3.up;
        public Vector3 GravityDirection { get { return gravityDirection; } }

        //#############################################################################

        #region initialization

        public void Initialize(GameControl.IGameControllerBase gameController)
        {
            tempPhysicsHandler = GetComponent<CharacControllerRecu>();
            animator = GetComponentInChildren<Animator>();

            playerModel = gameController.PlayerModel;
            CharData = Resources.Load<CharData>("ScriptableObjects/CharData");

            myTransform = transform;
            stateMachine = new StateMachine(this, playerModel);

            //*******************************************

            stateMachine.Add(ePlayerState.move, new MoveState(this, stateMachine));
            stateMachine.Add(ePlayerState.stand, new StandState(this, stateMachine));

            stateMachine.ChangeState(new StandEnterArgs(ePlayerState.empty));

            //*******************************************

            Utilities.EventManager.OnMenuSwitchedEvent += OnMenuSwitchedEventHandler;
            Utilities.EventManager.TeleportPlayerEvent += OnTeleportPlayerEventHandler;

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
            var inputInfo = new PlayerInputInfo();

            var movementInfo = new PlayerMovementInfo
            {
                position = myTransform.position,
                forward = myTransform.forward,
                up = myTransform.up,
                side = myTransform.right,
                velocity = velocity
            };

            //*******************************************
            //handling input

            if (isHandlingInput)
            {
                inputInfo.leftStickRaw = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

                if (inputInfo.leftStickRaw.magnitude < CharData.General.StickDeadMaxVal)
                {
                    inputInfo.leftStickAtZero = true;
                }

                var stickToCamera = rotator.forward * Input.GetAxisRaw("Vertical") + rotator.right * Input.GetAxisRaw("Horizontal");
                inputInfo.leftStickToCamera = (Quaternion.AngleAxis(Vector3.Angle(transform.up, Vector3.up), Vector3.Cross(transform.up, Vector3.up))) * stickToCamera;

                inputInfo.leftStickToSlope = (Quaternion.AngleAxis(Vector3.Angle(transform.up, tempCollisionInfo.currentGroundNormal), Vector3.Cross(transform.up, tempCollisionInfo.currentGroundNormal))) * inputInfo.leftStickToCamera;

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
                stateMachine.HandleInput(inputInfo, movementInfo, tempCollisionInfo);
            }

            //*******************************************
            //state update           

            //call state update
            var stateReturn = stateMachine.Update(Time.deltaTime, inputInfo, movementInfo, tempCollisionInfo);

            //handling return
            if (stateReturn.CanTurnPlayerSet)
            {
                canTurnPlayer = stateReturn.CanTurnPlayer;
            }

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

            //computing new velocity
            float transitionSpeed = stateReturn.TransitionSpeedSet ? stateReturn.TransitionSpeed : CharData.General.TransitionSpeed;

            var newVelocity = velocity * (1 - Time.deltaTime * transitionSpeed) + stateReturn.DesiredVelocity * (Time.deltaTime * transitionSpeed);

            //Debug.LogFormat("new velocity: {0}", newVelocity);

            //adding gravity
            newVelocity += gravityDirection * (CharData.General.GravityStrength * Time.deltaTime);

            //Debug.LogFormat("after gravity: {0}", newVelocity);

            //clamping speed
            if (newVelocity.magnitude > CharData.General.MaxSpeed)
            {
                newVelocity.Normalize();
                newVelocity *= CharData.General.MaxSpeed;

                //Debug.LogFormat("clamped velocity: {0}", newVelocity);
            }

            //*******************************************
            //physics update

            var turnedVelocity = TurnLocalToSpace(newVelocity);

            if (stateMachine.CurrentState == ePlayerState.glide)
            {
                newVelocity = tempPhysicsHandler.Move(newVelocity /*+ externalVelocity*/);
            }
            else
            {
                newVelocity = tempPhysicsHandler.Move(turnedVelocity /*+ externalVelocity*/);
            }
            //Debug.LogFormat("after physics: {0}", newVelocity);

            externalVelocity = Vector3.zero;
            tempCollisionInfo = tempPhysicsHandler.collisions;

            //*******************************************
            //moving player

            /*
             *  moving the player is currently handled by the (temp!) physics handler
             */

            velocity = newVelocity;

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

            animator.SetBool("OnGround", tempCollisionInfo.below);
            animator.SetFloat("Forward", velocity.magnitude / CharData.Move.Speed);
            animator.SetFloat("Turn", (inputInfo.leftStickAtZero ? 0f : Mathf.Lerp(0f, Vector3.SignedAngle(transform.forward, Vector3.ProjectOnPlane(TurnLocalToSpace(inputInfo.leftStickToCamera), transform.up), transform.up), CharData.General.TurnSpeed * Time.deltaTime) / 7f));
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
                stateMachine.ChangeState(new StandEnterArgs(ePlayerState.empty));
                ChangeGravityDirection(Vector3.down);
            }
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