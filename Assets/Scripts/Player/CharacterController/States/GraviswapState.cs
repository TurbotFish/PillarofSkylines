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
            //Debug.Log("Enter State: Graviswap");
            initialVelocity = charController.MovementInfo.velocity;
            charController.SetVelocity(Vector3.zero, true);
        }

        public void Exit()
        {
            
            if (timerGravDefault > 0f)
            {
                charController.ChangeGravityDirection(Vector3.down);
            } /*else
            {
                float xSnap = 0f, ySnap = 0f, zSnap = 0f;
                if (charController.MyTransform.eulerAngles.x % 90 < 10)
                {
                    xSnap = -charController.MyTransform.eulerAngles.x % 90;
                }
                if (charController.MyTransform.eulerAngles.x % 90 > 80)
                {
                    xSnap = 90 - charController.MyTransform.eulerAngles.x % 90;
                }
                if (charController.MyTransform.eulerAngles.y % 90 < 10)
                {
                    ySnap = -charController.MyTransform.eulerAngles.y % 90;
                }
                if (charController.MyTransform.eulerAngles.y % 90 > 80)
                {
                    ySnap = 90 - charController.MyTransform.eulerAngles.y % 90;
                }
                if (charController.MyTransform.eulerAngles.z % 90 < 10)
                {
                    zSnap = -charController.MyTransform.eulerAngles.z % 90;
                }
                if (charController.MyTransform.eulerAngles.z % 90 > 80)
                {
                    zSnap = 90 - charController.MyTransform.eulerAngles.z % 90;
                }
                charController.MyTransform.eulerAngles = new Vector3(charController.MyTransform.eulerAngles.x + xSnap
                                                                    , charController.MyTransform.eulerAngles.y + ySnap
                                                                    , charController.MyTransform.eulerAngles.z + zSnap);
            }*/
            
            
            //Debug.Log("Exit State: Graviswap");
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
            CharData.GraviswapData graviswapdata = charController.CharData.Graviswap;

           // charController.MyTransform.Rotate(charController.MyTransform.forward, inputInfo.leftStickRaw.x * 10, Space.World);
            //charController.MyTransform.Rotate(charController.MyTransform.right, -inputInfo.leftStickRaw.z * 10, Space.World);

			Quaternion quarterBack = Quaternion.AngleAxis (-inputInfo.leftStickRaw.x * 10f * Time.deltaTime * graviswapdata.TurnSpeed, charController.MyTransform.forward) * Quaternion.AngleAxis (inputInfo.leftStickRaw.z * 10f * Time.deltaTime * graviswapdata.TurnSpeed, charController.MyTransform.right);
			charController.ChangeGravityDirection (quarterBack * -charController.MyTransform.up, charController.MyTransform.position + charController.MyTransform.up);


            if (inputInfo.leftStickAtZero)
            {/*
                float xSnap = 0f, ySnap = 0f, zSnap = 0f;
                if (charController.MyTransform.eulerAngles.x % 90 < 45)
                {
                    xSnap = -charController.MyTransform.eulerAngles.x % 90;
                }
                if (charController.MyTransform.eulerAngles.x % 90 >= 45)
                {
                    xSnap = 90 - charController.MyTransform.eulerAngles.x % 90;
                }
                if (charController.MyTransform.eulerAngles.y % 90 < 45)
                {
                    ySnap = -charController.MyTransform.eulerAngles.y % 90;
                }
                if (charController.MyTransform.eulerAngles.y % 90 >= 45)
                {
                    ySnap = 90 - charController.MyTransform.eulerAngles.y % 90;
                }
                if (charController.MyTransform.eulerAngles.z % 90 < 45)
                {
                    zSnap = -charController.MyTransform.eulerAngles.z % 90;
                }
                if (charController.MyTransform.eulerAngles.z % 90 >= 45)
                {
                    zSnap = 90 - charController.MyTransform.eulerAngles.z % 90;
                }

                Debug.Log("x : " + xSnap + " y : " + ySnap + " z : " + zSnap);

                Quaternion quaterSnap = Quaternion.Euler(xSnap, ySnap, zSnap);

                charController.ChangeGravityDirection(quaterSnap * -charController.MyTransform.up, charController.MyTransform.position + charController.MyTransform.up);*/

                Vector3 snapPoint = GetClosestSnap(charController.MyTransform.up);

                Quaternion quaterSnap = Quaternion.AngleAxis((Vector3.Angle(charController.MyTransform.up, snapPoint) < 5f * Time.deltaTime * graviswapdata.SnapSpeed) ? Vector3.Angle(charController.MyTransform.up, snapPoint) : 5f * Time.deltaTime * graviswapdata.SnapSpeed, Vector3.Cross(charController.MyTransform.up, snapPoint));

                charController.ChangeGravityDirection(quaterSnap * -charController.MyTransform.up, charController.MyTransform.position + charController.MyTransform.up);

            }

            

            var result = new StateReturnContainer
            {
                CanTurnPlayer = false,
                IgnoreGravity = true,
                Acceleration = Vector3.zero
            };
            
            return result;
        }

        Vector3 GetClosestSnap(Vector3 vector)
        {
            Vector3 closestVector = Vector3.down;
            float smallestAngle = float.MaxValue;

            if (Vector3.Angle(charController.MyTransform.up, Vector3.up) < smallestAngle)
            {
                if (Vector3.Angle(charController.MyTransform.up, Vector3.up) < 45f)
                    return Vector3.up;
                smallestAngle = Vector3.Angle(charController.MyTransform.up, Vector3.up);
                closestVector = Vector3.up;
            }
            if (Vector3.Angle(charController.MyTransform.up, -Vector3.up) < smallestAngle)
            {
                if (Vector3.Angle(charController.MyTransform.up, -Vector3.up) < 45f)
                    return -Vector3.up;
                smallestAngle = Vector3.Angle(charController.MyTransform.up, -Vector3.up);
                closestVector = -Vector3.up;
            }
            if (Vector3.Angle(charController.MyTransform.up, Vector3.right) < smallestAngle)
            {
                if (Vector3.Angle(charController.MyTransform.up, Vector3.right) < 45f)
                    return Vector3.right;
                smallestAngle = Vector3.Angle(charController.MyTransform.up, Vector3.right);
                closestVector = Vector3.right;
            }
            if (Vector3.Angle(charController.MyTransform.up, -Vector3.right) < smallestAngle)
            {
                if (Vector3.Angle(charController.MyTransform.up, -Vector3.right) < 45f)
                    return -Vector3.right;
                smallestAngle = Vector3.Angle(charController.MyTransform.up, -Vector3.right);
                closestVector = -Vector3.right;
            }
            if (Vector3.Angle(charController.MyTransform.up, Vector3.forward) < smallestAngle)
            {
                if (Vector3.Angle(charController.MyTransform.up, Vector3.forward) < 45f)
                    return Vector3.forward;
                smallestAngle = Vector3.Angle(charController.MyTransform.up, Vector3.forward);
                closestVector = Vector3.forward;
            }
            if (Vector3.Angle(charController.MyTransform.up, -Vector3.forward) < smallestAngle)
            {
                if (Vector3.Angle(charController.MyTransform.up, -Vector3.forward) < 45f)
                    return -Vector3.forward;
                smallestAngle = Vector3.Angle(charController.MyTransform.up, -Vector3.forward);
                closestVector = -Vector3.forward;
            }
            return closestVector;
        }

        //#############################################################################
    }
} //end of namespace