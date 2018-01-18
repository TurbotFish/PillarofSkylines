using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class WallRunState : IState
    {
        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.wallRun; } }

        CharController charController;
        StateMachine stateMachine;
        CharData.WallRunData wallRunData;

        //#############################################################################

        public WallRunState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            wallRunData = charController.CharData.WallRun;
        }

        //#############################################################################

        public void Enter()
        {
            Debug.LogWarning("Enter State: Wall Run");

            //if the player should not be able to walldrift he starts falling again
            if (!CheckCanEnterWallRun(charController))
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
            }
        }

        public void Exit()
        {
            Debug.LogWarning("Exit State: Wall Run");
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
            else if (!collisionInfo.side /*|| !WallRunState.CheckWallRunStick(charController)*/)
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
            }
            //jump
            else if (inputInfo.jumpButtonDown)
            {
                //charController.MyTransform.forward = Vector3.ProjectOnPlane(collisionInfo.currentWallNormal, charController.MyTransform.up);
                //Vector3 jumpDirection = (Vector3.up + collisionInfo.currentWallNormal * 2).normalized;

                //var state = new AirState(charController, stateMachine, AirState.eAirStateMode.jump);
                //state.SetJumpTimer(charController.CharData.Move.CanStillJumpTimer);
                //state.SetJumpDirection(jumpDirection);

                //stateMachine.ChangeState(state);
            }
<<<<<<< HEAD
        }
=======
        } 
>>>>>>> github/grr

        public StateReturnContainer Update(float dt)
        {
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            //********************************
            //acceleration
            Vector3 wallrunVel = Vector3.ProjectOnPlane(movementInfo.velocity, collisionInfo.currentWallNormal);

            float restSpeed = movementInfo.velocity.magnitude - wallrunVel.magnitude;
            Vector3 transferredVel = restSpeed * ((Vector3.forward + Vector3.up) * 0.5f);

            Vector3 acceleration = wallrunVel * wallRunData.SlowdownFactor + transferredVel;

            //********************************
            //direction
            Vector3 direction = Vector3.Cross(collisionInfo.currentWallNormal, charController.MyTransform.up);

            //if the player is going in the other direction thna the cross result, direction is inversed
            if (Vector3.Dot(direction, charController.MyTransform.forward) < 0)
            {
                direction *= -1;
            }

            //********************************
            //wall hugging
<<<<<<< HEAD
            Vector3 wallHugging = Vector3.zero;// Vector3.right * 2f;
=======
            Vector3 wallHugging = Vector3.right * 2f;
>>>>>>> github/grr

            //if the wall is not on the right side of the player, wallHugging is inversed
            if (Vector3.Dot(-collisionInfo.currentWallNormal, charController.MyTransform.right) < 0)
            {
                wallHugging *= -1;
            }

            //
            var result = new StateReturnContainer()
            {
                CanTurnPlayer = false,
                //PlayerForward = direction.normalized,
                Acceleration = wallHugging + acceleration,
                TransitionSpeed = wallRunData.TransitionSpeed
            };

            return result;
        }

        //#############################################################################

        /// <summary>
        /// Checks whether a horizontal wall run can be started.
        /// </summary>
        /// <returns></returns>
        /// 
        public static bool CheckCanEnterWallRun(CharController charController)
        {
            //has the player activated the wall run ability?
            bool isAbilityActive = charController.PlayerModel.CheckAbilityActive(eAbilityType.WallRun);

            //is the player touching a wall?
            bool isTouchingWall = charController.CollisionInfo.side;

            //is the wall slope valid? If the wall is bend to the outside it is not
            bool isWallSlopeValid = Vector3.SignedAngle(charController.CollisionInfo.currentWallNormal, charController.MyTransform.up, charController.MyTransform.forward) >= 0f;

            //is the player jumping
            bool isJumping = charController.MovementInfo.velocity.y > 0;

            //is the player facing the wall
<<<<<<< HEAD
            bool directionOK = CheckPlayerForward(charController, 0f, 90f);
=======
            bool directionOK = CheckPlayerForward(charController, 0f, 60f);
>>>>>>> github/grr

            //is the player
            bool stickOK = CheckWallRunStick(charController);

            return (isAbilityActive && isTouchingWall && isWallSlopeValid && isJumping && directionOK && stickOK);
        }

        ///// <summary>
        ///// Checks if the player is turned towards the wall.
        ///// </summary>
        //public static bool CheckWallRunPlayerForward(CharController charController, float minDotProduct = -1)
        //{
        //    float dotProduct = Vector3.Dot(charController.MyTransform.forward, charController.CollisionInfo.currentWallNormal);
        //    return (minDotProduct <= dotProduct && dotProduct <= charController.PlayerModel.AbilityData.WallRun.General.DirectionTrigger);
        //}

        /// <summary>
        /// Checks the unsigned angle between the inversed normal vector of the wall and the forward vector of the player.
        /// An angle of 0 means that the player is facing the wall and an angle of 180 means that the player has his back to the wall.
        /// </summary>
        /// <returns>true if the angle is within the bounds, false otherwise</returns>
        public static bool CheckPlayerForward(CharController charController, float minAngle, float maxAngle)
        {
            float angle = Vector3.Angle(-charController.CollisionInfo.currentWallNormal, charController.MyTransform.forward);

            if (angle >= minAngle && angle < maxAngle)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether the player is holding the left stick in position to activate or stay in wall run mode.
        /// </summary>
        public static bool CheckWallRunStick(CharController charController)
        {
            var stick = charController.InputInfo.leftStickToCamera;
            float dotproduct = Vector3.Dot(stick, charController.MyTransform.forward);
            return (dotproduct >= charController.PlayerModel.AbilityData.WallRun.General.StickTrigger && !charController.InputInfo.leftStickAtZero);
        }

        //#############################################################################
    }
}