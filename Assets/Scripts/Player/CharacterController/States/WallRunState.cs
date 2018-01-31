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

        bool firstFrame = true;

        float timerToUnstick;
        int noWallCounter = 0;
        Vector3 lastWallNormal;

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
            Debug.Log("Enter State: Wall Run");

            //if the player should not be able to walldrift he starts falling again
            if (!CheckCanEnterWallRun(charController))
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
            }
        }

        public void Exit()
        {
            Debug.Log("Exit State: Wall Run");
        }

        //#############################################################################

        public void HandleInput()
        {
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            PlayerInputInfo inputInfo = charController.InputInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            //land on ground
            if (collisionInfo.below)
            {
                stateMachine.ChangeState(new StandState(charController, stateMachine));
            }
            //no wall or stick released => fall
            else if (!collisionInfo.side && noWallCounter > 5)
            {
                Debug.Log("no col");
                stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
            }
            else if (timerToUnstick > wallRunData.TimeToUnstick)
            {
                Debug.Log("no stick");
                stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
            }
            //jump
            else if (inputInfo.jumpButtonDown)
            {
                Vector3 jumpDirection = Vector3.ProjectOnPlane(lastWallNormal + movementInfo.velocity.normalized, charController.MyTransform.up).normalized;
                charController.MyTransform.rotation = Quaternion.LookRotation(jumpDirection, charController.MyTransform.up);


                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.jump);
                stateMachine.SetRemainingAerialJumps(charController.CharData.Jump.MaxAerialJumps);
                state.SetJumpDirection(jumpDirection);
                state.SetTimerAirControl(wallRunData.TimerBeforeAirControl);

                stateMachine.ChangeState(state);
            }
        }


        public StateReturnContainer Update(float dt)
        {
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;
            PlayerInputInfo inputInfo = charController.InputInfo;

            if (collisionInfo.side)
            {
                noWallCounter = 0;
                if (collisionInfo.currentWallNormal != Vector3.zero)
                    lastWallNormal = collisionInfo.currentWallNormal;
            }
            else
            {
                noWallCounter++;
            }

            if (!CheckWallRunStick(inputInfo, lastWallNormal, wallRunData.MaxTriggerAngle) && !inputInfo.leftStickAtZero)
            {
                timerToUnstick += Time.deltaTime;
            } else
            {
                timerToUnstick = 0f;
            }
                //********************************
                //the direction along the wall
                Vector3 wallRunDir = Vector3.zero;
            if (firstFrame)
                wallRunDir = Vector3.ProjectOnPlane(movementInfo.velocity * wallRunData.SpeedMultiplier, lastWallNormal);
            

            //translate the direction to local space
            Vector3 localWallRunDir = charController.MyTransform.worldToLocalMatrix.MultiplyVector(wallRunDir);

            localWallRunDir += Vector3.ProjectOnPlane(inputInfo.leftStickToCamera, lastWallNormal) * wallRunData.Speed;
            Vector3 acceleration = localWallRunDir;

            //********************************
            
            //wall hugging
            //Vector3 wallHugging = charController.MyTransform.worldToLocalMatrix.MultiplyVector(-lastWallNormal) * 4f * (noWallCounter + 1);
            

            var result = new StateReturnContainer()
            {
                CanTurnPlayer = false,
                PlayerForward = Vector3.ProjectOnPlane(localWallRunDir, charController.MyTransform.up).normalized,
                Acceleration = acceleration,
                GravityMultiplier = wallRunData.GravityModifier,
                TransitionSpeed = wallRunData.TransitionSpeed
            };

            if (firstFrame) firstFrame = false;

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
            bool isWallSlopeValid = Vector3.Dot(charController.CollisionInfo.currentWallNormal, charController.MyTransform.up) >= -0.1f;
            
            //is the player facing the wall
            bool directionOK = CheckPlayerForward(charController, 0f, charController.CharData.WallRun.MaxTriggerAngle);

            //is the player
            bool stickOK = CheckWallRunStick(charController.InputInfo, charController.CollisionInfo.currentWallNormal, charController.CharData.WallRun.MaxTriggerAngle) || charController.InputInfo.leftStickAtZero;

            //Debug.Log("act : " + isAbilityActive + " touch : " + isTouchingWall + " slope : " + isWallSlopeValid + " dir : " + directionOK + " stick : " + stickOK);

            return (isAbilityActive && isTouchingWall && isWallSlopeValid && directionOK && stickOK);
        }

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
        public static bool CheckWallRunStick(PlayerInputInfo inputInfo, Vector3 wallNormal, float maxAngle)
        {
            var stick = inputInfo.leftStickToCamera;

            float angle = Vector3.Angle(-wallNormal, stick);

            //Debug.Log("stick : " + stick + " wall : " + -wallNormal + " angle : " + angle);

            if (angle < maxAngle)
            {
                return true;
            }

            return false;
        }

        //#############################################################################
    }
}