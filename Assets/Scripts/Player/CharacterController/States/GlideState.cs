using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class GlideState : IState
    {
        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.glide; } }

        CharController charController;
        StateMachine stateMachine;
        CharData.GlideData glideData;

        public GlideState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            glideData = charController.CharData.Glide;
        }

        //#############################################################################

        public void Enter()
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
            if (inputInfo.sprintButtonDown)
            {
                stateMachine.ChangeState(new FallState(charController, stateMachine));
            }
        }

        public StateReturnContainer Update(float dt, PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            var result = new StateReturnContainer
            {

            };

            return result;
        }

        //#############################################################################
    }
}