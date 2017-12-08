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
        }

        public void Exit()
        {
            Debug.Log("Exit State: Fall");
        }

        //#############################################################################

        public void HandleInput(PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {

        }

        public StateReturnContainer Update(float dt, PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            var result = new StateReturnContainer
            {
                CanTurnPlayer = true,

                DesiredVelocity = inputInfo.leftStickRaw * fallData.Speed * dt,

                MaxSpeed = fallData.MaxSpeed,
                TransitionSpeed = fallData.TransitionSpeed
            };

            if (collisionInfo.below)
            {
                stateMachine.ChangeState(new StandEnterArgs(StateId));
            }

            return result;
        }

        //#############################################################################
    }
}