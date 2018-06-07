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

		float verticalAngle;
		float horizontalAngle;
        bool glidingDown = false;
        bool recovering = false;
        float currentSpeed;

        //#############################################################################

        public GlideState(CharController charController, StateMachine stateMachine) {
			this.charController = charController;
			this.stateMachine = stateMachine;
			glideData = charController.CharData.Glide;
		}

		//#############################################################################

		public void Enter() {
			//Debug.Log("Enter State: Glide");
			charController.animator.SetBool("Gliding", true);
			//charController.glideParticles.Play();
			verticalAngle = Vector3.Angle(Vector3.up, (charController.MovementInfo.velocity)) - 90f;
            //Debug.Log("velocity entering : " + charController.MovementInfo.velocity + " vertical angle : " + verticalAngle);
            horizontalAngle = 0f;

			charController.fxManager.GlidePlay ();
		}

		public void Exit() {
			//Debug.Log("Exit State: Glide");
			//charController.glideParticles.Stop();
			charController.animator.SetBool("Gliding", false);

			charController.fxManager.GlideStop ();

		}

		//#############################################################################

		public void HandleInput() {
			PlayerInputInfo inputInfo = charController.InputInfo;
			CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

			//stop gliding
			if (inputInfo.glideButtonUp) {
                AirState state = new AirState(charController, stateMachine, AirState.eAirStateMode.fall);
                state.SetTimerAirControl(glideData.ExitInertiaTime);
                stateMachine.ChangeState(state);
			}
            //landing
            else if (collisionInfo.below) {
				stateMachine.ChangeState(new StandState(charController, stateMachine));
			}
            //landing
            else if (collisionInfo.below) {
				stateMachine.ChangeState(new StandState(charController, stateMachine));
			}
        }

		public StateReturnContainer Update(float dt) {
			PlayerInputInfo inputInfo = charController.InputInfo;
			PlayerMovementInfo movementInfo = charController.MovementInfo;
			CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;
            
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

            verticalAngle = Mathf.Lerp(
				verticalAngle,
				targetVerticalAngle,
				a * dt * b
			);

            //Debug.Log("verticalangle : " + verticalAngle);
            
            if (verticalAngle > glideData.BaseAngle)
            {
                glidingDown = false;
                currentSpeed = 0.1f;
            }

            //SPEED DEPENDING ON VERTICAL

            //Update the speed of the player
            if (!glidingDown)
            {
                if (verticalAngle < glideData.BaseAngle)
                {
                    currentSpeed = movementInfo.velocity.magnitude - glideData.UpwardDecceleration.Evaluate(Mathf.Abs((verticalAngle - glideData.BaseAngle) / (glideData.MinAngle - glideData.BaseAngle)) /** dt*/);
                }
                else
                {
                    currentSpeed = Mathf.Lerp(
                        movementInfo.velocity.magnitude,
                        (glideData.BaseSpeed + glideData.DownwardAcceleration.Evaluate((verticalAngle - glideData.BaseAngle) / (glideData.MaxAngle - glideData.BaseAngle))) * stateMachine.glideMultiplier,
                        glideData.SpeedSmooth /** dt*/
                    );
                }
            }

            //Debug.Log("vertical angle : " + verticalAngle + "speed : " + currentSpeed);

			//Calculate the velocity of the player with his speed and vertical angle
			Vector3 targetVelocity = Quaternion.AngleAxis(verticalAngle, charController.MyTransform.right) * charController.MyTransform.forward * currentSpeed;

            //Debug.Log("target velocity : " + targetVelocity + " forward : " + charController.MyTransform.forward);



            //Stall when the player is too slow
            if (currentSpeed < glideData.StallSpeed) {
				verticalAngle = glideData.MaxAngle;
			}


            if (currentSpeed < 0f)
                glidingDown = true;

            if (glidingDown)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, glideData.StaticSpeed, glideData.SpeedSmooth);
                targetVelocity = Quaternion.AngleAxis(90f - glideData.MaxAngle, -charController.MyTransform.right) * (- charController.MyTransform.up * currentSpeed);
            
                targetVelocity = Quaternion.AngleAxis(Mathf.Lerp(0f, glideData.MaxAngle - glideData.BaseAngle, verticalAngle+90f/glideData.BaseAngle+90f), -charController.MyTransform.right) * targetVelocity;
                 
            }

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
            targetVelocity = Quaternion.AngleAxis(horizontalAngle, charController.MyTransform.up) * targetVelocity;


            //Debug.Log("velocity : " + movementInfo.velocity);

            //Turn the player horizontally with the angle calculated above
            //charController.MyTransform.Rotate(Vector3.up, horizontalAngle, Space.Self);


            
            var result = new StateReturnContainer
            {
                Acceleration = TurnSpaceToLocal(targetVelocity),
                TransitionSpeed = 8,
                CanTurnPlayer = false,
                IgnoreGravity = true
			};

            charController.MyTransform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(targetVelocity, charController.MyTransform.up), charController.MyTransform.up);

			//Animator 
			charController.animator.SetFloat("GlideHorizontal", horizontalAngle);
			charController.animator.SetFloat("GlideVertical", (verticalAngle > 0 ? verticalAngle / glideData.MaxAngle : verticalAngle / -glideData.MinAngle));

			//FX
			charController.fxManager.receivedVelocity = charController.MovementInfo.velocity.magnitude;
			charController.fxManager.GlideUpdate();

			return result;
		}

		//#############################################################################

		public Vector3 TurnLocalToSpace(Vector3 vector) {
			return (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, charController.MyTransform.up), (Vector3.Cross(Vector3.up, charController.MyTransform.up) != Vector3.zero ? Vector3.Cross(Vector3.up, charController.MyTransform.up) : Vector3.forward))) * vector;
		}

        public Vector3 TurnSpaceToLocal(Vector3 vector)
        {
            return (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, charController.MyTransform.up), (Vector3.Cross(charController.MyTransform.up, Vector3.up) != Vector3.zero ? Vector3.Cross(charController.MyTransform.up, Vector3.up) : Vector3.forward))) * vector;
        }
    }
}