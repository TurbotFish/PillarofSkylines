﻿using Game.Model;
using Game.Player.CharacterController.Containers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
	public class AirState : IState
	{
		public enum eAirStateMode
		{
			fall,
			jump,
			aerialJump}

		;

		//#############################################################################

		#region member variables

		public ePlayerState StateId { get { return ePlayerState.air; } }

		CharController charController;
		StateMachine stateMachine;

		CharData.JumpData jumpData;
		CharData.FallData fallData;

		eAirStateMode mode = eAirStateMode.fall;
		int remainingAerialJumps = 0;
		float jumpTimer = 0;
        float jumpStrengthFromState = 1;
        Vector3 jumpDirection = Vector3.zero;
        float timerAirControl = 0;

		bool initializing;
		bool firstUpdate;

		#endregion member variables

		//#############################################################################

		#region constructor

		/// <summary>
		/// Constructor for AirState. Default mode is "fall".
		/// </summary>
		public AirState(CharController charController, StateMachine stateMachine, eAirStateMode mode) {
			this.charController = charController;
			this.stateMachine = stateMachine;
			jumpData = charController.CharData.Jump;
			fallData = charController.CharData.Fall;

			this.mode = mode;

			initializing = true;
		}

		#endregion constructor

		//#############################################################################

		#region setters


		public void SetJumpTimer(float jumpTimer) {
			if (!initializing) {
				return;
			}

			this.jumpTimer = jumpTimer;
		}

        /// <summary>
        /// Sets the direction of the initial force of a jump.
        /// THIS IS IN WORLDSPACE!
        /// </summary>
        public void SetJumpDirection(Vector3 direction) {
			if (!initializing) {
				return;
			}

			jumpDirection = direction.normalized;
		}

        public void SetTimerAirControl(float timerAirControl)
        {
            if (!initializing)
            {
                return;
            }

            this.timerAirControl = timerAirControl;
        }

        public void SetJumpStrengthModifierFromState(float jumpStrengthModifier)
        {
            if (!initializing)
            {
                return;
            }

            this.jumpStrengthFromState = jumpStrengthModifier;
        }

        #endregion setters

        //#############################################################################

        #region

        public void Enter() {
			//Debug.LogFormat("Enter State: Air - {0}", mode.ToString());

            remainingAerialJumps = stateMachine.CheckRemainingAerialJumps();
			initializing = false;
			firstUpdate = true;

			Utilities.EventManager.WindTunnelPartEnteredEvent += OnWindTunnelPartEnteredEventHandler;
		}

		public void Exit() {
			//Debug.LogFormat("Exit State: Air - {0}", mode.ToString());

			Utilities.EventManager.WindTunnelPartEnteredEvent -= OnWindTunnelPartEnteredEventHandler;
		}

		#endregion

		//#############################################################################

		#region update

		public void HandleInput() {
			PlayerInputInfo inputInfo = charController.InputInfo;
			PlayerMovementInfo movementInfo = charController.MovementInfo;
			CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

			//jump
			if (inputInfo.jumpButtonDown) {
				//if the player is falling but may still jump normally
				if (mode == eAirStateMode.fall && jumpTimer > 0) {
					var state = new AirState(charController, stateMachine, eAirStateMode.jump);
					stateMachine.SetRemainingAerialJumps(remainingAerialJumps);

					stateMachine.ChangeState(state);
				}
                //if the player is falling or is jumping again
                else if (remainingAerialJumps > 0 && charController.PlayerModel.CheckAbilityActive(eAbilityType.DoubleJump)) {
					var state = new AirState(charController, stateMachine, eAirStateMode.aerialJump);
					stateMachine.SetRemainingAerialJumps(remainingAerialJumps - 1);

					stateMachine.ChangeState(state);
				}
			}
            //dash
            else if (inputInfo.dashButtonDown && !stateMachine.CheckStateLocked(ePlayerState.dash)) {
				stateMachine.ChangeState(new DashState(charController, stateMachine, movementInfo.forward));
			}
            //glide
            else if (inputInfo.sprintButtonDown && !stateMachine.CheckStateLocked(ePlayerState.glide)) {
				stateMachine.ChangeState(new GlideState(charController, stateMachine));
			}
            //landing on slope
            else if (collisionInfo.below && (Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) > charController.CharData.General.MaxSlopeAngle 
                || collisionInfo.SlippySlope && Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) > 2f)) {
				stateMachine.ChangeState(new SlideState(charController, stateMachine));
			}
            //landing
            else if (collisionInfo.below) {
				stateMachine.ChangeState(new StandState(charController, stateMachine));
                if (!collisionInfo.SlippySlope)
                    charController.SetVelocity(Vector3.Project(movementInfo.velocity, inputInfo.leftStickToSlope), false);
			}
            //wall- run/drift
            else if (collisionInfo.side && WallRunState.CheckCanEnterWallRun(charController)) {
				stateMachine.ChangeState(new WallRunState(charController, stateMachine));
            }
            else if (inputInfo.rightStickButtonDown && charController.graviswapAvailable)
            {
                stateMachine.ChangeState(new GraviSwapState(charController, stateMachine), true);
            }
        }

		public StateReturnContainer Update(float dt) {
			PlayerInputInfo inputInfo = charController.InputInfo;
			PlayerMovementInfo movementInfo = charController.MovementInfo;

            if (jumpTimer > 0)
            {
                jumpTimer -= dt;
            }
            if (timerAirControl > 0)
            {
                timerAirControl -= dt;
            }

            var result = new StateReturnContainer()
			{
				//keepVerticalMovement = true
			};

			//set wether the player can turn the character
			result.CanTurnPlayer = timerAirControl <= 0 ? true : false;

			//first update for jump (initial force)
			if (firstUpdate && (mode == eAirStateMode.jump || mode == eAirStateMode.aerialJump)) {
				float jumpStrength = jumpData.Strength * stateMachine.jumpMultiplier * jumpStrengthFromState;

				if (mode == eAirStateMode.aerialJump) {
					jumpStrength *= jumpData.AerialJumpCoeff;
					charController.aerialJumpFX.Play();
				}

				Vector3 direction = (Vector3.up + jumpDirection).normalized;

				charController.AddExternalVelocity((direction) * jumpStrength + Vector3.ProjectOnPlane(charController.MovementInfo.velocity, Vector3.up) * jumpData.ImpactOfCurrentSpeed, false, false);
				result.resetVerticalVelocity = true;
                //Debug.Log("velocity added : " + (direction) * jumpStrength + Vector3.ProjectOnPlane(charController.MovementInfo.velocity, Vector3.up) * jumpData.ImpactOfCurrentSpeed);
				//result.Acceleration = (movementInfo.velocity * 0.05f + Vector3.up) * jumpStrength;
				//result.TransitionSpeed = 1 / dt;

				firstUpdate = false;
			}
            //falling
            else if (mode == eAirStateMode.fall || movementInfo.velocity.y < 0) {
				result.MaxSpeed = fallData.MaxSpeed;
				result.TransitionSpeed = fallData.TransitionSpeed;
				result.Acceleration = timerAirControl <= 0 ? inputInfo.leftStickToCamera * fallData.Speed * stateMachine.speedMultiplier : jumpDirection * fallData.Speed * stateMachine.speedMultiplier;
                result.keepVerticalMovement = true;
			}
            //jumping
            else if (mode == eAirStateMode.jump || mode == eAirStateMode.aerialJump) {
				float minJumpStrength = jumpData.MinStrength * stateMachine.jumpMultiplier;

				if (mode == eAirStateMode.aerialJump) {
					minJumpStrength *= jumpData.AerialJumpCoeff * stateMachine.jumpMultiplier;
				}

				result.Acceleration = timerAirControl <= 0 ? inputInfo.leftStickToCamera * jumpData.Speed * stateMachine.speedMultiplier : jumpDirection * jumpData.Speed * stateMachine.speedMultiplier;

				if (!inputInfo.jumpButton && movementInfo.velocity.y > minJumpStrength) {
					charController.SetVelocity(new Vector3(movementInfo.velocity.x, minJumpStrength, movementInfo.velocity.z), false);
					//result.Acceleration += Vector3.down * (movementInfo.velocity.y - minJumpStrength) * (0.1f / dt);
				}

                result.TransitionSpeed = jumpData.TransitionSpeed;
			}
            //error
            else {
				Debug.LogError("error!");
			}


			return result;
		}

		#endregion update

		//#############################################################################

		#region utils

		void OnWindTunnelPartEnteredEventHandler(object sender, Utilities.EventManager.WindTunnelPartEnteredEventArgs args) {
			stateMachine.ChangeState(new WindTunnelState(charController, stateMachine));
		}

		#endregion utils

		//#############################################################################
	}
}