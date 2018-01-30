using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
	public class MoveState : IState
	{
		//#############################################################################

		public ePlayerState StateId { get { return ePlayerState.move; } }

		CharController charController;
		StateMachine stateMachine;
		CharData.MoveData moveData;

		//#############################################################################

		public MoveState(CharController charController, StateMachine stateMachine) {
			this.charController = charController;
			this.stateMachine = stateMachine;
			moveData = charController.CharData.Move;
		}

		//#############################################################################

		public void Enter() {
			Debug.Log("Enter State: Move");
		}

		public void Exit() {
			Debug.Log("Exit State: Move");
		}

		//#############################################################################

		public void HandleInput() {
			PlayerInputInfo inputInfo = charController.InputInfo;
			PlayerMovementInfo movementInfo = charController.MovementInfo;
			CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            if (inputInfo.jumpButtonDown) {
                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.jump);
				stateMachine.SetRemainingAerialJumps(charController.CharData.Jump.MaxAerialJumps);

				stateMachine.ChangeState(state);
			} else if (inputInfo.dashButtonDown && !stateMachine.CheckStateLocked(ePlayerState.dash)) {
				stateMachine.ChangeState(new DashState(charController, stateMachine, movementInfo.forward));
            } else if(inputInfo.sprintButton && collisionInfo.cornerNormal && !stateMachine.CheckStateLocked(ePlayerState.hover))
            {
                stateMachine.ChangeState(new HoverState(charController, stateMachine));
            } else if ((Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) > charController.CharData.General.MaxSlopeAngle 
                || collisionInfo.SlippySlope && Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) > 2f) 
                && !collisionInfo.cornerNormal) {
                stateMachine.ChangeState(new SlideState(charController, stateMachine));
            } else if (inputInfo.leftStickAtZero) {
				stateMachine.ChangeState(new StandState(charController, stateMachine));
			} else if (!collisionInfo.below)
            {
                Debug.Log("in air " + inputInfo.sprintButton);
                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.fall);
                stateMachine.SetRemainingAerialJumps(charController.CharData.Jump.MaxAerialJumps);
                state.SetJumpTimer(moveData.CanStillJumpTimer);

                stateMachine.ChangeState(state);
                
			}
        }

		public StateReturnContainer Update(float dt) {
			PlayerInputInfo inputInfo = charController.InputInfo;


            var result = new StateReturnContainer
            {
                CanTurnPlayer = true,

                Acceleration = inputInfo.leftStickToSlope * moveData.Speed * (inputInfo.sprintButton ? moveData.SprintCoefficient : 1) * stateMachine.speedMultiplier,

                IgnoreGravity = true,
				MaxSpeed = moveData.MaxSpeed,
				TransitionSpeed = moveData.TransitionSpeed
			};

            if (charController.CollisionInfo.SlippySlope)
                result.TransitionSpeed = moveData.SlipperyGroundTransitionSpeed;
                
            return result;
		}

		//#############################################################################
	}
}
 //end of namespace