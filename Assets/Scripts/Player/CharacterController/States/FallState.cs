using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using Game.Player.CharacterController.EnterArgs;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class FallState : IState
    {
        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.fall; } }

        CharController charController;
        StateMachine stateMachine;
        CharData.FallData fallData;

        float jumpTimer;

        //#############################################################################

        public FallState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            fallData = charController.CharData.Fall;
        }

        //#############################################################################

        public void Enter(BaseEnterArgs enterArgs)
        {
            Debug.Log("Enter State: Fall");

            if (enterArgs.NewState != ePlayerState.fall)
            {
                Debug.LogError("Fall state entered with wrong arguments!");
                return;
            }

            var args = enterArgs as FallEnterArgs;

            if (args.CanJump)
            {
                jumpTimer = args.JumpTimer;
            }
        }

        public void Exit()
        {
            Debug.Log("Exit State: Fall");

            jumpTimer = 0;
        }

        //#############################################################################

        public void HandleInput(PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            if (inputInfo.jumpButtonDown && jumpTimer > 0)
            {
                stateMachine.ChangeState(new JumpEnterArgs(StateId));
            }
            else if (collisionInfo.below)
            {
                stateMachine.ChangeState(new StandEnterArgs(StateId));
            }
        }

        public StateReturnContainer Update(float dt, PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            if (jumpTimer > 0)
            {
                jumpTimer -= dt;
            }

            var result = new StateReturnContainer
            {
                CanTurnPlayer = true,
                MaxSpeed = fallData.MaxSpeed,
                TransitionSpeed = fallData.TransitionSpeed,
                Acceleration = inputInfo.leftStickToCamera * fallData.Speed
            };

            return result;
        }

        //#############################################################################
    }
}