﻿using Game.Player.CharacterController.Containers;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
	public class PhantomState : IState
	{
		//#############################################################################

		public ePlayerState StateId { get { return ePlayerState.phantom; } }

		CharController charController;
		StateMachine stateMachine;
		CharData.PhantomData phantomData;
        PhantomController phantomController;
        
		float verticalAngle;
		float horizontalAngle;

		//#############################################################################

		public PhantomState(CharController charController, StateMachine stateMachine) {
			this.charController = charController;
			this.stateMachine = stateMachine;
			phantomData = charController.CharData.Phantom;
		}

		//#############################################################################

		public void Enter() {

            phantomController = charController.phantomController;
            verticalAngle = 0f;
            horizontalAngle = 0f;
            phantomController.gameObject.SetActive(true);
            charController.rotator.SetParent(charController.phantomController.transform);
            charController.PlayerController.InteractionController.currentEcho.MyTransform.SetParent(charController.phantomController.transform);

            Time.timeScale = 0.05f;
		}

		public void Exit() {

            charController.PlayerController.InteractionController.currentEcho.MyTransform.SetParent(null);
            phantomController.gameObject.SetActive(false);
            charController.phantomController.myTransform.localPosition = new Vector3(0f, 1.45f, 0f);
            charController.rotator.SetParent(charController.MyTransform);
            charController.phantomController.myTransform.localRotation = Quaternion.identity;

            charController.ResetEchoInputTime();

            Time.timeScale = 1f;
        }

		//#############################################################################

		public void HandleInput() {
			PlayerInputInfo inputInfo = charController.InputInfo;
			CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            //stop phantoming
            if (inputInfo.echoButtonUp || (charController.MyTransform.position - phantomController.myTransform.position).sqrMagnitude > phantomData.MaxDistance * phantomData.MaxDistance)
            {
                AirState state = new AirState(charController, stateMachine, AirState.eAirStateMode.fall);
                stateMachine.ChangeState(state);
			}
        }

		public StateReturnContainer Update(float dt)
        {

            dt = Time.unscaledDeltaTime;
            PlayerInputInfo inputInfo = charController.InputInfo;


            //---------VERTICAL


            //Turn the vertical input of the player into an angle between glideMinAngle and glideMaxAngle
            float targetVerticalAngle;
            if (inputInfo.leftStickRaw.z > 0)
            {
                targetVerticalAngle = Mathf.Lerp(phantomData.BaseAngle, phantomData.MaxAngle, (inputInfo.leftStickRaw.z));
            } else
            {
                targetVerticalAngle = Mathf.Lerp(phantomData.MinAngle, phantomData.BaseAngle, (inputInfo.leftStickRaw.z + 1f));
            }


			//Update the current vertical angle of the player depending on the angle calculated above
			float a = (targetVerticalAngle > verticalAngle ? phantomData.VertUpAngleCtrl : phantomData.VertDownAngleCtrl);
			float b = (inputInfo.leftStickAtZero ? phantomData.NoInputImpactCoeff : 1f);

            verticalAngle = Mathf.Lerp(verticalAngle, targetVerticalAngle, a * dt * b);
            
            
            //Debug.Log("vertical angle : " + verticalAngle + "speed : " + currentSpeed);

			//Calculate the velocity of the player with his speed and vertical angle
			Vector3 targetVelocity = Quaternion.AngleAxis(verticalAngle, phantomController.myTransform.right) * phantomController.myTransform.forward * phantomData.Speed;
            

            //---------HORIZONTAL

            //Transform the horizontal input of the player into an angle between glideMinHorizontalAngle and glideMaxHorizontalAngle
            float targetHorizontalAngle = Mathf.Lerp(phantomData.MinHorizAngle, phantomData.MaxHorizAngle, (inputInfo.leftStickRaw.x / 2) + .5f);

			//Update the current horizontal angle of the player depending on the angle calculated above
			horizontalAngle = Mathf.Lerp(
				horizontalAngle,
				targetHorizontalAngle,
				(Mathf.Abs(horizontalAngle) > Mathf.Abs(targetHorizontalAngle) ? phantomData.HorizComingBack : phantomData.HorizAngleCtrl) * dt
			);

            //Turn the player horizontally with the angle calculated above

            phantomController.myTransform.Rotate(charController.MyTransform.up, horizontalAngle);
            //Debug.Log("velocity : " + movementInfo.velocity);

            //Turn the player horizontally with the angle calculated above
            //charController.MyTransform.Rotate(Vector3.up, horizontalAngle, Space.Self);

            targetVelocity = Quaternion.AngleAxis(horizontalAngle, phantomController.myTransform.up) * targetVelocity;

            phantomController.Move(targetVelocity * Time.unscaledDeltaTime);

            var result = new StateReturnContainer
            {
                Acceleration = Vector3.zero,
                TransitionSpeed = 8,
                CanTurnPlayer = false,
                IgnoreGravity = true
			};

			return result;
		}

		//#############################################################################

		public Vector3 TurnLocalToSpace(Vector3 vector) {
			return (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, phantomController.myTransform.up), Vector3.Cross(Vector3.up, phantomController.myTransform.up))) * vector;
		}

        public Vector3 TurnSpaceToLocal(Vector3 vector)
        {
            return (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, phantomController.myTransform.up), Vector3.Cross(phantomController.myTransform.up, Vector3.up))) * vector;
        }
    }
}