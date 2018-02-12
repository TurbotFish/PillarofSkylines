using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class GraviSwapState : IState
    {
        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.graviswap; } }

        CharController charController;
        StateMachine stateMachine;
        CharData.StandData standData;

        Vector3 initialVelocity;
        float timerGravDefault;

        //#############################################################################

        public GraviSwapState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            standData = charController.CharData.Stand;
        }

        //#############################################################################

        public void Enter()
        {
            timerGravDefault = .5f;
            Debug.Log("Enter State: Graviswap");
            initialVelocity = charController.MovementInfo.velocity;
            charController.SetVelocity(Vector3.zero, true);
        }

        public void Exit()
        {

            Debug.Log("timer : " + timerGravDefault);
            if (timerGravDefault > 0f)
            {
                Debug.Log("hello back to default");
                charController.ChangeGravityDirection(Vector3.down);
            } else
            {
                float xSnap = 0f, ySnap = 0f, zSnap = 0f;
                Debug.Log("eulers : " + charController.MyTransform.eulerAngles);
                if (charController.MyTransform.eulerAngles.x % 90 < 10)
                {
                    xSnap = -charController.MyTransform.eulerAngles.x % 90;
                    Debug.Log("x snap");
                }
                if (charController.MyTransform.eulerAngles.x % 90 > 80)
                {
                    xSnap = 90 - charController.MyTransform.eulerAngles.x % 90;
                    Debug.Log("x snap");
                }
                if (charController.MyTransform.eulerAngles.y % 90 < 10)
                {
                    ySnap = -charController.MyTransform.eulerAngles.y % 90;
                    Debug.Log("y snap");
                }
                if (charController.MyTransform.eulerAngles.y % 90 > 80)
                {
                    ySnap = 90 - charController.MyTransform.eulerAngles.y % 90;
                    Debug.Log("y snap");
                }
                if (charController.MyTransform.eulerAngles.z % 90 < 10)
                {
                    zSnap = -charController.MyTransform.eulerAngles.z % 90;
                    Debug.Log("z snap");
                }
                if (charController.MyTransform.eulerAngles.z % 90 > 80)
                {
                    zSnap = 90 - charController.MyTransform.eulerAngles.z % 90;
                    Debug.Log("z snap");
                }
                charController.MyTransform.eulerAngles = new Vector3(charController.MyTransform.eulerAngles.x + xSnap
                                                                    , charController.MyTransform.eulerAngles.y + ySnap
                                                                    , charController.MyTransform.eulerAngles.z + zSnap);
            }
            
            
            Debug.Log("Exit State: Graviswap");
        }

        //#############################################################################

        public void HandleInput()
        {
            PlayerInputInfo inputInfo = charController.InputInfo;
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;


            if (inputInfo.jumpButtonDown || inputInfo.rightStickButtonDown)
            {
                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.fall);
                charController.SetVelocity(initialVelocity, false);
                stateMachine.ChangeState(state);
            }
        }

        public StateReturnContainer Update(float dt)
        {

            timerGravDefault -= Time.deltaTime;

            PlayerInputInfo inputInfo = charController.InputInfo;

            charController.MyTransform.Rotate(charController.MyTransform.forward, inputInfo.leftStickRaw.x * 10, Space.World);
            charController.MyTransform.Rotate(charController.MyTransform.right, -inputInfo.leftStickRaw.z * 10, Space.World);

            var result = new StateReturnContainer
            {
                CanTurnPlayer = false,
                IgnoreGravity = true,
                Acceleration = Vector3.zero
            };
            
            return result;
        }

        //#############################################################################
    }
} //end of namespace