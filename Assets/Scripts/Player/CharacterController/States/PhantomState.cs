using Game.Player.CharacterController.Containers;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
	public class PhantomState : IState
	{
		//#############################################################################

		public ePlayerState StateId { get { return ePlayerState.phantom; } }

		CharController charController;
		StateMachine stateMachine;
		CharData.GlideData glideData;
        PhantomController phantomController;

        float currentSpeed = 10f;
		float verticalAngle;
		float horizontalAngle;

		//#############################################################################

		public PhantomState(CharController charController, StateMachine stateMachine) {
			this.charController = charController;
			this.stateMachine = stateMachine;
			glideData = charController.CharData.Glide;
		}

		//#############################################################################

		public void Enter() {
			Debug.Log("Enter State: Phantom");
			//charController.glideParticles.Play();
			verticalAngle = Vector3.Angle(Vector3.up, (charController.MovementInfo.velocity)) - 90f;

            horizontalAngle = 0f;

            charController.rotator.SetParent(charController.phantomController.transform);

            Time.timeScale = 0.05f;
		}

		public void Exit() {
			Debug.Log("Exit State: Phantom");
            //charController.glideParticles.Stop();

            charController.phantomController.myTransform.localPosition = new Vector3(0f, 1.45f, 0f);
            charController.rotator.SetParent(charController.MyTransform);
            charController.phantomController.myTransform.rotation = Quaternion.identity;

            

            Time.timeScale = 1f;
        }

		//#############################################################################

		public void HandleInput() {
			PlayerInputInfo inputInfo = charController.InputInfo;
			CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

			//stop gliding
			if (inputInfo.echoButtonUp) {
                AirState state = new AirState(charController, stateMachine, AirState.eAirStateMode.fall);
                stateMachine.ChangeState(state);
			}
        }

		public StateReturnContainer Update(float dt)
        {
            dt = Time.unscaledDeltaTime;
            PlayerInputInfo inputInfo = charController.InputInfo;
            phantomController = charController.phantomController;


            //---------VERTICAL


            //Turn the vertical input of the player into an angle between glideMinAngle and glideMaxAngl
            float targetVerticalAngle;
            if (inputInfo.leftStickRaw.z > 0)
            {
                targetVerticalAngle = Mathf.Lerp(glideData.BaseAngle, glideData.MaxAngle, (inputInfo.leftStickRaw.z));
            } else
            {
                targetVerticalAngle = Mathf.Lerp(glideData.MinAngle, glideData.BaseAngle, (inputInfo.leftStickRaw.z + 1f));
            }


			//Update the current vertical angle of the player depending on the angle calculated above
			float a = (targetVerticalAngle > verticalAngle ? glideData.VertUpAngleCtrl : glideData.VertDownAngleCtrl);
			float b = (inputInfo.leftStickAtZero ? glideData.NoInputImpactCoeff : 1f);

            verticalAngle = Mathf.Lerp(verticalAngle, targetVerticalAngle, a * dt * b);
            
            
            //Debug.Log("vertical angle : " + verticalAngle + "speed : " + currentSpeed);

			//Calculate the velocity of the player with his speed and vertical angle
			Vector3 targetVelocity = Quaternion.AngleAxis(verticalAngle, phantomController.myTransform.right) * phantomController.myTransform.forward * currentSpeed;
            

            //---------HORIZONTAL

            //Transform the horizontal input of the player into an angle between glideMinHorizontalAngle and glideMaxHorizontalAngle
            float targetHorizontalAngle = Mathf.Lerp(glideData.MinHorizAngle, glideData.MaxHorizAngle, (inputInfo.leftStickRaw.x / 2) + .5f);

			//Update the current horizontal angle of the player depending on the angle calculated above
			horizontalAngle = Mathf.Lerp(
				horizontalAngle,
				targetHorizontalAngle,
				(Mathf.Abs(horizontalAngle) > Mathf.Abs(targetHorizontalAngle) ? glideData.HorizComingBack : glideData.HorizAngleCtrl) * dt
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