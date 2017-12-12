using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using Game.Player.CharacterController.EnterArgs;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class JumpState : IState
    {
        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.jump; } }

        CharController charController;
        StateMachine stateMachine;
        CharData.JumpData jumpData;

        bool firstUpdate;
        float strength;
        Vector3 velocity;

        //#############################################################################

        public JumpState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            jumpData = charController.CharData.Jump;
        }

        //#############################################################################

        public void Enter(BaseEnterArgs enterArgs)
        {
            Debug.Log("Enter State: Jump");

            firstUpdate = true;
        }

        public void Exit()
        {
            Debug.Log("Exit State: Jump");
        }

        //#############################################################################

        public void HandleInput(PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            //if (inputInfo.jumpButtonUp)
            //{
            //    stateMachine.ChangeState(new FallEnterArgs(StateId));
            //}
        }

        public StateReturnContainer Update(float dt, PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            var result = new StateReturnContainer();

            result.CanTurnPlayer = false;
            result.MaxSpeed = 32;

            if (firstUpdate)
            {
                velocity = movementInfo.velocity;

                result.Acceleration = (velocity * 0.25f) + (Vector3.up * 18);
                result.TransitionSpeed = 1 / dt;

                firstUpdate = false;
            }
            else
            {
                result.Acceleration = velocity;
                result.TransitionSpeed = 1;

                if (movementInfo.velocity.y < 0)
                {
                    stateMachine.ChangeState(new FallEnterArgs(StateId));
                }
            }

            return result;
        }

        //#############################################################################
    }
}