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

        float timerBeforeJump;

        public SlideState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            slideData = charController.CharData.Slide;
        }

        //#############################################################################

        public void Enter()
        {
            //Debug.Log("Enter State: Slide");
			charController.animator.SetBool("Sliding", true);
            timerBeforeJump = slideData.WaitBeforeJump;
        }

        public void Exit()
        {
            //Debug.Log("Exit State: Slide");
			charController.animator.SetBool("Sliding", false); 
		}

        //#############################################################################

        public void HandleInput()
        {
            PlayerInputInfo inputInfo = charController.InputInfo;
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            //jump
            if (inputInfo.jumpButtonDown && timerBeforeJump<0f)
            {
                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.jump);
                stateMachine.SetRemainingAerialJumps(charController.CharData.Jump.MaxAerialJumps);
				state.SetJumpDirection(Vector3.Lerp(charController.MyTransform.up, Vector3.ProjectOnPlane(collisionInfo.currentGroundNormal, charController.MyTransform.up)
                    , (Quaternion.AngleAxis(Vector3.Angle(charController.MyTransform.up, collisionInfo.currentGroundNormal), Vector3.Cross(charController.MyTransform.up, collisionInfo.currentGroundNormal)) * movementInfo.velocity).y/-12));
                stateMachine.ChangeState(state);
            }
            //fall
            else if (!collisionInfo.below)
            {
                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.fall);
                stateMachine.SetRemainingAerialJumps(charController.CharData.Jump.MaxAerialJumps);
                stateMachine.ChangeState(state);
            }
            //dash
            else if (inputInfo.dashButtonDown && !stateMachine.CheckStateLocked(ePlayerState.dash))
            {
                stateMachine.ChangeState(new DashState(charController, stateMachine, movementInfo.forward));
            }
            //stop
            else if (Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) < charController.CharData.General.MaxSlopeAngle && !collisionInfo.SlippySlope || Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) < 2f) 
            {
                stateMachine.ChangeState(new StandState(charController, stateMachine));
            }
        }

        public StateReturnContainer Update(float dt)
        {
            timerBeforeJump -= Time.deltaTime;
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
                result.Acceleration = Vector3.ProjectOnPlane(-charController.MyTransform.up, charController.CollisionInfo.currentGroundNormal).normalized * slideData.MinimalSpeed;
                result.Acceleration += charController.InputInfo.leftStickToSlope * slideData.Control;
            }
            result.PlayerForward = result.Acceleration.normalized;

            return result;
        }
    }
} //end of namespace