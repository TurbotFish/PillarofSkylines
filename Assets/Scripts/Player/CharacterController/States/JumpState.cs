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

            if (firstUpdate)
            {
                velocity = movementInfo.velocity;

                result.Acceleration = (velocity * 0.035f + Vector3.up) * jumpData.Strength;
                result.TransitionSpeed = 1 / dt;

                firstUpdate = false;
            }
            else
            {
                //result.Acceleration = velocity;
                //result.Acceleration = movementInfo.forward * inputInfo.leftStickToCamera.y * 4;
                result.Acceleration = Vector3.Project(inputInfo.leftStickToCamera, movementInfo.forward).normalized * 4;

                result.TransitionSpeed = 1.5f;

                if (movementInfo.velocity.y < 0 || collisionInfo.above)
                {
                    stateMachine.ChangeState(new FallEnterArgs(StateId));
                }
            }

            return result;
        }

        //#############################################################################
    }
}