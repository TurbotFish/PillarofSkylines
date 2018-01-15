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
            if (!CanEnterWallDrift(charController, true))
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, false));
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
            else if (!collisionInfo.side || !WallRunState.CheckWallRunStick(charController))
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, false));
            }
            //jump
            else if (inputInfo.jumpButtonDown)
            {
                charController.MyTransform.forward = Vector3.ProjectOnPlane(collisionInfo.currentWallNormal, charController.MyTransform.up);
                Vector3 jumpDirection = (charController.MyTransform.up + collisionInfo.currentWallNormal * 2).normalized;

                stateMachine.ChangeState(new AirState(charController, stateMachine, true));
            }
        }

        public StateReturnContainer Update(float dt)
        {
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            float wallDriftSpeed = Mathf.Lerp(
                movementInfo.velocity.magnitude,
                charController.PlayerModel.AbilityData.WallRun.WallDrift.TargetSpeed,
                charController.PlayerModel.AbilityData.WallRun.WallDrift.LerpFactor * Time.deltaTime
            );

            var result = new StateReturnContainer()
            {
                CanTurnPlayer = false,
                PlayerForward = Vector3.ProjectOnPlane(-collisionInfo.currentWallNormal, charController.MyTransform.up),
                Acceleration = -charController.MyTransform.up * wallDriftSpeed
            };



            return result;
        }

        //#############################################################################

        public static bool CanEnterWallDrift(CharController charController, bool checkPlayerForward)
        {
            bool isAbilityActive = charController.PlayerModel.CheckAbilityActive(eAbilityType.WallRun);

            bool isTouchingWall = charController.CollisionInfo.side;

            bool isFalling = charController.MovementInfo.velocity.y <= 0;

            bool directionOK = true;
            if (checkPlayerForward)
            {
                directionOK = WallRunState.CheckWallRunPlayerForward(charController);
            }

            bool stickOK = WallRunState.CheckWallRunStick(charController);

            return false;
        }


        //#############################################################################
    }
}