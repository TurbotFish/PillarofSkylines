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
            strength = 18;
        }

        public void Exit()
        {
            Debug.Log("Exit State: Jump");
        }

        //#############################################################################

        public void HandleInput(PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            if (inputInfo.jumpButtonUp)
            {
                stateMachine.ChangeState(new FallEnterArgs(StateId));
            }
        }

        public StateReturnContainer Update(float dt, PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            var result = new StateReturnContainer();

            result.CanTurnPlayer = false;

            if (firstUpdate)
            {
                velocity = movementInfo.velocity;

                result.DesiredVelocity = (velocity*0.25f) + (Vector3.up * strength*dt);
                result.TransitionSpeed = 8;
            }
            else
            {
                result.DesiredVelocity = velocity + (-charController.GravityDirection * charController.CharData.General.GravityStrength);
                result.TransitionSpeed = 2;
            }


            //if (!firstUpdate && movementInfo.velocity.y < 0)
            //{
            //    stateMachine.ChangeState(new FallEnterArgs(StateId));
            //}

            if (firstUpdate)
            {
                firstUpdate = false;
            }

            return result;
        }

        //#############################################################################
    }
}