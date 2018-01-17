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
			charController.animator.SetBool("Sliding", true); 
        }

        public void Exit()
        {
            Debug.Log("Exit State: Slide");
			charController.animator.SetBool("Sliding", false); 
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
                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.jump);
                state.SetRemainingAerialJumps(charController.CharData.Jump.MaxAerialJumps);
				state.SetJumpDirection(collisionInfo.currentGroundNormal);
                stateMachine.ChangeState(state);
            }
            //fall
            else if (!collisionInfo.below)
            {
                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.fall);

                stateMachine.ChangeState(state);
            }
            //stop
			else if (Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) < charController.CharData.General.MaxSlopeAngle && !collisionInfo.SlippySlope) 
            {
                stateMachine.ChangeState(new StandState(charController, stateMachine));
            }
        }

        public StateReturnContainer Update(float dt)
        {
			var result = new StateReturnContainer 
				{ 
					CanTurnPlayer = false, 

					TransitionSpeed = slideData.TransitionSpeed, 
					IgnoreGravity = true 
				}; 

			if (Vector3.Angle(charController.CollisionInfo.currentGroundNormal, charController.MovementInfo.up) < charController.CharData.General.MaxSlopeAngle) { 
				result.Acceleration = Vector3.ProjectOnPlane(-charController.MyTransform.up, charController.CollisionInfo.currentGroundNormal).normalized * slideData.MinimalSpeed;
                result.Acceleration += charController.InputInfo.leftStickToSlope * slideData.Control;
            } else {
				result.IgnoreGravity = false;
                result.Acceleration = Vector3.ProjectOnPlane(-charController.MyTransform.up, charController.CollisionInfo.currentGroundNormal).normalized;
            }
            result.PlayerForward = result.Acceleration;

            return result;
        }
    }
} //end of namespace