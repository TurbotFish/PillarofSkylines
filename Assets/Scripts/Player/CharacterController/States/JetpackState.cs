using Game.Model;
using Game.Player.CharacterController.Containers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
	public class JetpackState : IState
	{

		//#############################################################################

		#region member variables

		public ePlayerState StateId { get { return ePlayerState.jetpack; } }

		CharController charController;
		StateMachine stateMachine;

		CharData.JetpackData jetpackData;
        CharData.GeneralData generalData;
        
		bool initializing;
		bool firstUpdate;
        
		#endregion member variables

		//#############################################################################

		#region constructor

		/// <summary>
		/// Constructor for AirState. Default mode is "fall".
		/// </summary>
		public JetpackState(CharController charController, StateMachine stateMachine) {
			this.charController = charController;
			this.stateMachine = stateMachine;
			jetpackData = charController.CharData.Jetpack;
            generalData = charController.CharData.General;
            
			initializing = true;
		}

		#endregion constructor
        //#############################################################################

        #region

        public void Enter()
        {
            Debug.LogFormat("Enter State: Jetpack");
            
		}

		public void Exit()
        {
            Debug.LogFormat("Exit State: Jetpack");
            
		}

		#endregion

		//#############################################################################

		#region update

		public void HandleInput() {
			PlayerInputInfo inputInfo = charController.InputInfo;
			PlayerMovementInfo movementInfo = charController.MovementInfo;
			CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

			//jump
			if (inputInfo.jetpackButtonUp || !inputInfo.jetpackButton || stateMachine.jetpackFuel < 0) {
				//if the player is falling but may still jump normally
				var state = new AirState(charController, stateMachine, AirState.eAirStateMode.fall);

				stateMachine.ChangeState(state);
            }
            //dash
            else if (inputInfo.dashButtonDown && !stateMachine.CheckStateLocked(ePlayerState.dash)) {
				stateMachine.ChangeState(new DashState(charController, stateMachine, movementInfo.forward));
			}
            //glide
            else if (inputInfo.glideButtonDown && !stateMachine.CheckStateLocked(ePlayerState.glide)) {
				stateMachine.ChangeState(new GlideState(charController, stateMachine));
			}
        }

		public StateReturnContainer Update(float dt) {
			PlayerInputInfo inputInfo = charController.InputInfo;
			PlayerMovementInfo movementInfo = charController.MovementInfo;



            var result = new StateReturnContainer()
            {
                IgnoreGravity = true
            };

            Debug.Log("fuel : " + stateMachine.jetpackFuel);
            result.Acceleration = inputInfo.leftStickToCamera * jetpackData.HorizontalSpeed * stateMachine.speedMultiplier;
            
            if (movementInfo.velocity.y < jetpackData.MaxSpeed)
            {
                result.Acceleration += new Vector3(0f, movementInfo.velocity.y + jetpackData.Strength * Time.deltaTime * (movementInfo.velocity.y < 0 ? jetpackData.FallingCoeff : 1) * stateMachine.jumpMultiplier, 0f);   
            }
            
			return result;
		}

		#endregion update
        
		//#############################################################################
	}
}