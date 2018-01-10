using Game.Player.CharacterController.Containers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class AirState : IState
    {
        enum eAirState { fall, jump, aerialJump };

        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.air; } }

        CharController charController;
        StateMachine stateMachine;

        CharData.JumpData jumpData;
        CharData.FallData fallData;

        eAirState state = eAirState.fall;
        int remainingAerialJumps = 0;
        float jumpTimer = 0;

        bool firstUpdate;

        //#############################################################################

        /// <summary>
        /// Constructor for a normal fall or jump.
        /// </summary>
        public AirState(CharController charController, StateMachine stateMachine, bool jump)
        {
            Init(charController, stateMachine);

            if (jump)
            {
                state = eAirState.jump;
                remainingAerialJumps = jumpData.MaxAerialJumps;
            }
            else
            {
                state = eAirState.fall;
            }
        }

        /// <summary>
        /// Constructor for a fall with a possibility to jump.
        /// </summary>
        public AirState(CharController charController, StateMachine stateMachine, float jumpTimer)
        {
            Init(charController, stateMachine);

            state = eAirState.fall;
            this.jumpTimer = jumpTimer;
        }

        /// <summary>
        /// Constructor for an aerial jump.
        /// </summary>
        private AirState(CharController charController, StateMachine stateMachine, int remainingAerialJumps)
        {
            Init(charController, stateMachine);

            state = eAirState.aerialJump;
            this.remainingAerialJumps = remainingAerialJumps;
        }

        private void Init(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            jumpData = charController.CharData.Jump;
            fallData = charController.CharData.Fall;
        }

        //#############################################################################

        public void Enter()
        {
            Debug.Log("Enter State: Air");

            firstUpdate = true;

            Utilities.EventManager.WindTunnelPartEnteredEvent += OnWindTunnelPartEnteredEventHandler;
        }

        public void Exit()
        {
            Debug.Log("Exit State: Air");

            Utilities.EventManager.WindTunnelPartEnteredEvent -= OnWindTunnelPartEnteredEventHandler;
        }

        //#############################################################################

        public void HandleInput()
        {
            PlayerInputInfo inputInfo = charController.InputInfo;
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            //jump
            if (inputInfo.jumpButtonDown)
            {
                if ((state == eAirState.aerialJump || state == eAirState.jump)
                    && remainingAerialJumps > 0
                    && charController.PlayerModel.CheckAbilityActive(eAbilityType.DoubleJump)
                   )
                {
                    stateMachine.ChangeState(new AirState(charController, stateMachine, remainingAerialJumps - 1));
                }
                else if (state == eAirState.fall && jumpTimer > 0)
                {
                    stateMachine.ChangeState(new AirState(charController, stateMachine, jumpData.MaxAerialJumps));
                }
            }
            //dash
            else if (inputInfo.dashButtonDown && !stateMachine.CheckStateLocked(ePlayerState.dash))
            {
                stateMachine.ChangeState(new DashState(charController, stateMachine, movementInfo.forward));
            }
            //landing
            else if (collisionInfo.below)
            {
                stateMachine.ChangeState(new StandState(charController, stateMachine));
            }
        }

        public StateReturnContainer Update(float dt)
        {
            PlayerInputInfo inputInfo = charController.InputInfo;
            PlayerMovementInfo movementInfo = charController.MovementInfo;

            if (jumpTimer > 0)
            {
                jumpTimer -= dt;
            }

            var result = new StateReturnContainer();

            if (state == eAirState.fall)
            {
                result.CanTurnPlayer = true;
                result.MaxSpeed = fallData.MaxSpeed;
                result.TransitionSpeed = fallData.TransitionSpeed;
                result.Acceleration = inputInfo.leftStickToCamera * fallData.Speed;
            }
            else
            {
                result.CanTurnPlayer = false;

                float jumpStrength = jumpData.Strength;
                float minJumpStrength = jumpData.MinStrength;

                if (state == eAirState.aerialJump)
                {
                    jumpStrength *= jumpData.AerialJumpCoeff;
                    minJumpStrength *= jumpData.AerialJumpCoeff;
                }

                if (firstUpdate)
                {
                    result.Acceleration = (movementInfo.velocity * 0.05f + Vector3.up) * jumpStrength;
                    result.TransitionSpeed = 1 / dt;

                    firstUpdate = false;
                }
                else
                {
                    result.Acceleration = Vector3.Project(inputInfo.leftStickToCamera, movementInfo.forward) * inputInfo.leftStickToCamera.magnitude * jumpData.Speed;

                    if (!inputInfo.jumpButton && movementInfo.velocity.y > minJumpStrength)
                    {
                        result.Acceleration += charController.GravityDirection * (movementInfo.velocity.y - minJumpStrength) * (0.1f / dt);
                    }

                    result.TransitionSpeed = jumpData.TransitionSpeed;
                }
            }

            return result;
        }

        //#############################################################################

        void OnWindTunnelPartEnteredEventHandler(object sender, Utilities.EventManager.WindTunnelPartEnteredEventArgs args)
        {
            stateMachine.ChangeState(new WindTunnelState(charController, stateMachine));
        }

        //#############################################################################
    }
}