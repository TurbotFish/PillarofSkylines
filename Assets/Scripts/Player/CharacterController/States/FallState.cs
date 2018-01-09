//using System.Collections;
//using System.Collections.Generic;
//using Game.Player.CharacterController.Containers;
//using UnityEngine;

//namespace Game.Player.CharacterController.States
//{
//    public class FallState : IState
//    {
//        //#############################################################################

//        public ePlayerState StateId { get { return ePlayerState.fall; } }

//        CharController charController;
//        StateMachine stateMachine;
//        CharData.FallData fallData;

//        float jumpTimer;

//        //#############################################################################

//        public FallState(CharController charController, StateMachine stateMachine, float jumpTimer = 0)
//        {
//            this.charController = charController;
//            this.stateMachine = stateMachine;
//            fallData = charController.CharData.Fall;

//            this.jumpTimer = jumpTimer;
//        }

//        //#############################################################################

//        public void Enter()
//        {
//            Debug.Log("Enter State: Fall");

//            Utilities.EventManager.WindTunnelPartEnteredEvent += OnWindTunnelPartEnteredEventHandler;
//        }

//        public void Exit()
//        {
//            Debug.Log("Exit State: Fall");

//            Utilities.EventManager.WindTunnelPartEnteredEvent -= OnWindTunnelPartEnteredEventHandler;
//        }

//        //#############################################################################

//        public void HandleInput()
//        {
//            PlayerInputInfo inputInfo = charController.InputInfo;
//            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

//            if (inputInfo.jumpButtonDown && jumpTimer > 0)
//            {
//                stateMachine.ChangeState(new JumpState(charController, stateMachine));
//            }
//            else if (collisionInfo.below)
//            {
//                stateMachine.ChangeState(new StandState(charController, stateMachine));
//            }
//        }

//        public StateReturnContainer Update(float dt)
//        {
//            PlayerInputInfo inputInfo = charController.InputInfo;

//            if (jumpTimer > 0)
//            {
//                jumpTimer -= dt;
//            }

//            var result = new StateReturnContainer
//            {
//                CanTurnPlayer = true,
//                MaxSpeed = fallData.MaxSpeed,
//                TransitionSpeed = fallData.TransitionSpeed,
//                Acceleration = inputInfo.leftStickToCamera * fallData.Speed
//            };

//            return result;
//        }

//        //#############################################################################

//        void OnWindTunnelPartEnteredEventHandler(object sender, Utilities.EventManager.WindTunnelPartEnteredEventArgs args)
//        {
//            stateMachine.ChangeState(new WindTunnelState(charController, stateMachine));
//        }

//        //#############################################################################
//    }
//}