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

        //#############################################################################

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

            verticalAngle = Vector3.Angle(charController.MyTransform.up, TurnLocalToSpace(charController.MovementInfo.velocity)) - 90f;
            horizontalAngle = 0f;
        }

        public void Exit()
        {
            Debug.Log("Exit State: Fall");
        }

        //#############################################################################

        public void HandleInput()
        {
            PlayerInputInfo inputInfo = charController.InputInfo;

            if (inputInfo.sprintButtonDown)
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, false));
            }
        }

        public StateReturnContainer Update(float dt)
        {
            PlayerInputInfo inputInfo = charController.InputInfo;
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            //Turn the vertical input of the player into an angle between glideMinAngle and glideMaxAngle
            float targetVerticalAngle = Mathf.Clamp(
                Mathf.Lerp(glideData.MinAngle,
                glideData.MaxAngle,
                (inputInfo.leftStickRaw.z / 2) + .5f) + glideData.BaseAngle,
                glideData.MinAngle, glideData.MaxAngle
            );

            //Update the current vertical angle of the player depending on the angle calculated above
            float a = (targetVerticalAngle > verticalAngle ? glideData.VertUpAngleCtrl : glideData.VertDownAngleCtrl);
            float b = (targetVerticalAngle == glideData.BaseAngle ? glideData.NoInputImpactCoeff : 1f);

            verticalAngle = Mathf.Lerp(
                verticalAngle,
                targetVerticalAngle,
                a * dt * b
            );

            //Update the speed of the player
            float currentSpeed;

            if (verticalAngle < glideData.BaseAngle)
            {
                currentSpeed = movementInfo.velocity.magnitude - glideData.UpwardDecceleration.Evaluate(Mathf.Abs((verticalAngle - glideData.BaseAngle) / (glideData.MinAngle - glideData.BaseAngle)) /** dt*/);
            }
            else
            {
                currentSpeed = Mathf.Lerp(
                    movementInfo.velocity.magnitude,
                    glideData.BaseSpeed + glideData.DownwardAcceleration.Evaluate((verticalAngle - glideData.BaseAngle) / (glideData.MaxAngle - glideData.BaseAngle)),
                    glideData.SpeedSmooth /** dt*/
                );
            }

            //Calculate the velocity of the player with his speed and vertical angle
            Vector3 targetVelocity = Quaternion.AngleAxis(verticalAngle, charController.MyTransform.right) * charController.MyTransform.forward * currentSpeed;

            //Stall when the player is too slow
            if (currentSpeed < glideData.StallSpeed)
            {
                verticalAngle = glideData.MaxAngle;
            }

            //Turn the horizontal input of the player into an angle between glideMinHorizontalAngle and glideMaxHorizontalAngle
            float targetHorizontalAngle = Mathf.Lerp(glideData.MinHorizAngle, glideData.MaxHorizAngle, (inputInfo.leftStickRaw.x / 2) + .5f);

            //Update the current horizontal angle of the player depending on the angle calculated above
            horizontalAngle = Mathf.Lerp(
                horizontalAngle,
                targetHorizontalAngle,
                (Mathf.Abs(horizontalAngle) > Mathf.Abs(targetHorizontalAngle) ? glideData.HorizComingBack : glideData.HorizAngleCtrl) * dt
            );

            //Turn the velocity horizontally with the angle calculated above
            targetVelocity = Quaternion.AngleAxis(horizontalAngle, charController.MyTransform.up) * targetVelocity;


            var result = new StateReturnContainer
            {
                Acceleration = targetVelocity,
                TransitionSpeed = 8,
                CanTurnPlayer = true
        };

            return result;
        }

        //#############################################################################

        public Vector3 TurnLocalToSpace(Vector3 vector)
        {
            return (Quaternion.AngleAxis(Vector3.Angle(Vector3.up, charController.MyTransform.up), Vector3.Cross(Vector3.up, charController.MyTransform.up))) * vector;
        }
    }
}