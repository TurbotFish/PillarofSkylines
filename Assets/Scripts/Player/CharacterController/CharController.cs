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

        PlayerModel playerModel;
        CharData charData;

        Transform myTransform;
        StateMachine stateMachine;

        bool isInitialized;
        bool isHandlingInput;
        bool canTurnPlayer;

        Vector3 velocity;

        //#############################################################################

        #region initialization

        public void Initialize(GameControl.IGameControllerBase gameController)
        {
            playerModel = gameController.PlayerModel;
            charData = Resources.Load<CharData>("ScriptableObjects/CharData");

            myTransform = transform;
            stateMachine = new StateMachine(this, playerModel);

            //*******************************************

            //stateMachine.Add(ePlayerState.dash, n)

            //*******************************************

            Utilities.EventManager.OnMenuSwitchedEvent += OnMenuSwitchedEventHandler;

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
            //handling input

            var inputInfo = new PlayerInputInfo();

            if (isHandlingInput)
            {
                inputInfo.leftStickRaw = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

                if (inputInfo.leftStickRaw.magnitude < charData.General.LeftStickDeadMaxValue)
                {
                    inputInfo.leftStickAtZero = true;
                }

                var stickToCamera = rotator.forward * Input.GetAxisRaw("Vertical") + rotator.right * Input.GetAxisRaw("Horizontal");
                inputInfo.leftStickToCamera = (Quaternion.AngleAxis(Vector3.Angle(transform.up, Vector3.up), Vector3.Cross(transform.up, Vector3.up))) * stickToCamera;

                //inputInfo.leftStickToSlope = (Quaternion.AngleAxis(Vector3.Angle(transform.up, controller.collisions.currentGroundNormal), Vector3.Cross(transform.up, controller.collisions.currentGroundNormal))) * inputToCamera;

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
                stateMachine.HandleInput(inputInfo);
            }

            //*******************************************
            //state update

            var movementInfo = new PlayerMovementInfo();

            movementInfo.position = myTransform.position;
            movementInfo.forward = myTransform.forward;
            movementInfo.up = myTransform.up;
            movementInfo.side = myTransform.right;
            movementInfo.velocity = velocity;

            //
            var stateReturn = stateMachine.Update(Time.deltaTime, inputInfo, movementInfo);

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

            //computing new velocity
            float transitionSpeed = stateReturn.TransitionSpeedSet ? stateReturn.TransitionSpeed : charData.General.DefaultTransitionSpeed;

            movementInfo.velocity = velocity * (1 - Time.deltaTime * transitionSpeed) + stateReturn.DesiredVelocity * (Time.deltaTime * transitionSpeed);

            //adding gravity
            movementInfo.velocity += -myTransform.up * (charData.General.GravityStrength * Time.deltaTime);

            //clamping speed
            if (movementInfo.velocity.magnitude > charData.General.DefaultMaxSpeed)
            {
                movementInfo.velocity.Normalize();
                movementInfo.velocity *= charData.General.DefaultMaxSpeed;
            }

            //*******************************************
            //physics update

            //*******************************************
            //moving player

            //*******************************************
            //turning player

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

        #endregion event handlers

        //#############################################################################
    }
} //end of namespace