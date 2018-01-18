using Game.Player.CharacterController.Containers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class WallDriftState : IState
    {
        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.wallDrift; } }

        CharController charController;
        StateMachine stateMachine;
        CharData.WallDriftData driftData;

        //#############################################################################

        public WallDriftState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            driftData = charController.CharData.WallDrift;
        }

        //#############################################################################

        public void Enter()
        {
            Debug.Log("Enter State: Wall Drift");

            //if the player should not be able to walldrift he starts falling again
            if (!CanEnterWallDrift(charController))
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
            }
        }

        public void Exit()
        {
            Debug.Log("Exit State: Wall Drift");
        }

        //#############################################################################

        public void HandleInput()
        {
            PlayerInputInfo inputInfo = charController.InputInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            //land on ground
            if (collisionInfo.below)
            {
                stateMachine.ChangeState(new StandState(charController, stateMachine));
            }
            //no wall or stick released => fall
            else if (!collisionInfo.side)
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
            }
            else if (!WallRunState.CheckWallRunStick(charController))
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
            }
            //jump
            else if (inputInfo.jumpButtonDown)
            {
                Debug.LogWarning("check 1");

                charController.MyTransform.forward = Vector3.ProjectOnPlane(collisionInfo.currentWallNormal, charController.MyTransform.up);
                Vector3 jumpDirection = (Vector3.up + (Vector3.back * 2)).normalized;

                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.jump);
                state.SetJumpDirection(jumpDirection);
                state.SetAirControl(false);

                stateMachine.ChangeState(state);
            }
        }

        public StateReturnContainer Update(float dt)
        {
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            //compute the acceleration so that the player's speed tends towards the target speed
            float acceleration = (driftData.TargetSpeed - movementInfo.velocity.magnitude) * 2f;
            Vector3 driftAcceleration = -Vector3.up * acceleration;

            //a bit of force so that the player stays glued to the wall
            Vector3 wallHugging = Vector3.zero;// Vector3.forward * 2f;

            var result = new StateReturnContainer()
            {
                CanTurnPlayer = false,
                PlayerForward = Vector3.ProjectOnPlane(-collisionInfo.currentWallNormal, charController.MyTransform.up),
                Acceleration = wallHugging + driftAcceleration,
                TransitionSpeed = driftData.TransitionSpeed
            };

            return result;
        }

        //#############################################################################

        public static bool CanEnterWallDrift(CharController charController)
        {
            bool isAbilityActive = charController.PlayerModel.CheckAbilityActive(eAbilityType.WallRun);

            bool isTouchingWall = charController.CollisionInfo.side;

            bool isFalling = charController.MovementInfo.velocity.y <= 0;

            //you can only wall drift if the player is facing the wall
            bool directionOK = WallRunState.CheckPlayerForward(charController, 0f, charController.CharData.WallDrift.MaxTriggerAngle);

            bool stickOK = WallRunState.CheckWallRunStick(charController);

            //Debug.Log("isAbilityActive " + isAbilityActive + " isTouchingWall " + isTouchingWall + " isFalling " + isFalling + " directionOK " + directionOK + " stickOK " + stickOK);
            return (isAbilityActive && isTouchingWall && isFalling && directionOK && stickOK);
        }


        //#############################################################################
    }
}