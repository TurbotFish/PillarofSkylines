using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class SlideState : IState
    {
        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.slide; } }

        CharController charController;
        StateMachine stateMachine;
        CharData.SlideData slideData;

        public SlideState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            slideData = charController.CharData.Slide;
        }

        //#############################################################################

        public void Enter()
        {
            Debug.Log("Enter State: Slide");
        }

        public void Exit()
        {
            Debug.Log("Exit State: Slide");
        }

        //#############################################################################

        public void HandleInput()
        {
            PlayerInputInfo inputInfo = charController.InputInfo;
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            if (inputInfo.jumpButtonDown)
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, true));
            }

            else if (!collisionInfo.below)
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, false));
            }
            else if (Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) < charController.CharData.General.MaxSlopeAngle)
            {
                stateMachine.ChangeState(new StandState(charController, stateMachine));
            }
        }

        public StateReturnContainer Update(float dt)
        {
            var result = new StateReturnContainer
            {
                CanTurnPlayer = false,

                Acceleration = Vector3.zero,
                TransitionSpeed = slideData.TransitionSpeed
            };

            return result;
        }
    }
} //end of namespace