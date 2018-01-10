//using System.Collections;
//using System.Collections.Generic;
//using Game.Player.CharacterController.Containers;
//using UnityEngine;

//namespace Game.Player.CharacterController.States
//{
//    public class JumpState : IState
//    {
//        //#############################################################################

//        public ePlayerState StateId { get { return ePlayerState.jump; } }

//        CharController charController;
//        StateMachine stateMachine;
//        CharData.JumpData jumpData;

//        bool firstUpdate;
//        float strength;
//        Vector3 velocity;
//        int remainingAerialJumps;
//        bool isAerialJump;

//        //#############################################################################

//        public JumpState(CharController charController, StateMachine stateMachine)
//        {
//            this.charController = charController;
//            this.stateMachine = stateMachine;
//            jumpData = charController.CharData.Jump;

//            remainingAerialJumps = jumpData.MaxAerialJumps;
//            isAerialJump = false;
//        }

//        public JumpState(CharController charController, StateMachine stateMachine, int remainingAerialJumps, bool isAerialJump)
//        {
//            this.charController = charController;
//            this.stateMachine = stateMachine;
//            jumpData = charController.CharData.Jump;

//            this.remainingAerialJumps = remainingAerialJumps;
//            this.isAerialJump = isAerialJump;
//        }

//        //#############################################################################

//        public void Enter()
//        {
//            Debug.Log("Enter State: Jump");

//            //if(isAerialJump && !charController.PlayerModel.CheckAbilityActive(eAbilityType.DoubleJump))
//            //{
//            //    stateMachine.ChangeState()
//            //}

//            firstUpdate = true;

//            Utilities.EventManager.WindTunnelPartEnteredEvent += OnWindTunnelPartEnteredEventHandler;
//        }

//        public void Exit()
//        {
//            Debug.Log("Exit State: Jump");

//            Utilities.EventManager.WindTunnelPartEnteredEvent -= OnWindTunnelPartEnteredEventHandler;
//        }

//        //#############################################################################

//        public void HandleInput()
//        {
//            PlayerInputInfo inputInfo = charController.InputInfo;
//            PlayerMovementInfo movementInfo = charController.MovementInfo;
//            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

//            if (inputInfo.jumpButtonDown && remainingAerialJumps > 0)
//            {
//                stateMachine.ChangeState(new JumpState(charController, stateMachine, remainingAerialJumps - 1, true));
//            }
//            else if (inputInfo.dashButtonDown && !stateMachine.CheckStateLocked(ePlayerState.dash))
//            {
//                stateMachine.ChangeState(new DashState(charController, stateMachine, movementInfo.forward));
//            }

//            else if (movementInfo.velocity.y <= 0 || collisionInfo.above)
//            {
//                stateMachine.ChangeState(new FallState(charController, stateMachine));
//            }
//        }

//        public StateReturnContainer Update(float dt)
//        {
//            PlayerInputInfo inputInfo = charController.InputInfo;
//            PlayerMovementInfo movementInfo = charController.MovementInfo;

//            var result = new StateReturnContainer();

//            result.CanTurnPlayer = false;

//            float jumpStrength = jumpData.Strength;
//            float minJumpStrength = jumpData.MinStrength;
//            if (isAerialJump)
//            {
//                jumpStrength *= jumpData.AerialJumpCoeff;
//                minJumpStrength *= jumpData.AerialJumpCoeff;
//            }

//            if (firstUpdate)
//            {
//                velocity = movementInfo.velocity;

//                result.Acceleration = (velocity * 0.05f + Vector3.up) * jumpStrength;
//                result.TransitionSpeed = 1 / dt;

//                firstUpdate = false;
//            }
//            else
//            {
//                result.Acceleration = Vector3.Project(inputInfo.leftStickToCamera, movementInfo.forward) * inputInfo.leftStickToCamera.magnitude * jumpData.Speed;

//                if (!inputInfo.jumpButton && movementInfo.velocity.y > minJumpStrength)
//                {
//                    result.Acceleration += charController.GravityDirection * (movementInfo.velocity.y - minJumpStrength) * (0.1f / dt);
//                }
//                //else if(movementInfo.velocity.y > jumpStrength)
//                //{
//                //    result.Acceleration += charController.GravityDirection * (movementInfo.velocity.y - jumpStrength) * (0.1f / dt);
//                //}

//                result.TransitionSpeed = jumpData.TransitionSpeed;
//            }

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