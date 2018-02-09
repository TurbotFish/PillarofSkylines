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
            Debug.Log("Enter State: Graviswap");
            initialVelocity = charController.MovementInfo.velocity;
            charController.SetVelocity(Vector3.zero, true);
        }

        public void Exit()
        {
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
                charController.SetVelocity(initialVelocity, true);
                stateMachine.ChangeState(state);
            }
        }

        public StateReturnContainer Update(float dt)
        {

            PlayerInputInfo inputInfo = charController.InputInfo;

            charController.MyTransform.Rotate(charController.MyTransform.forward, -inputInfo.leftStickRaw.x * 15, Space.World);
            charController.MyTransform.Rotate(charController.MyTransform.right, -inputInfo.leftStickRaw.z * 15, Space.World);

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