using System.Collections;
using System.Collections.Generic;
using Game.Model;
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
        CharData.GeneralData generalData;

        bool firstFrame = true;
        bool ledgeGrab = false;
        bool climbUp = false;
        bool corner = false;

        float timerToUnstick;
        int noWallCounter = 0;
        Vector3 lastWallNormal;

        //#############################################################################

        public WallRunState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            wallRunData = charController.CharData.WallRun;
            generalData = charController.CharData.General;
        }

        //#############################################################################

        public void Enter()
        {

            //if the player should not be able to walldrift he starts falling again
            if (!CheckCanEnterWallRun(charController))
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
            }
            charController.animator.SetBool("WallRunning", true);
        }

        public void Exit()
        {
            charController.animator.SetBool("WallRunning", false);
        }

        //#############################################################################

        public void HandleInput()
        {
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            PlayerInputInfo inputInfo = charController.InputInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;
            
            //landing on slope
            if (collisionInfo.below && Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) < generalData.MinWallAngle && ((Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) > generalData.MaxSlopeAngle && !collisionInfo.NotSlippySlope)
                || collisionInfo.SlippySlope && Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) > 2f))
            {
                stateMachine.ChangeState(new SlideState(charController, stateMachine));
                charController.fxManager.FootDustPlay();

            }
            //landing
            else if (collisionInfo.below && Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) < generalData.MinWallAngle)
            {
                stateMachine.ChangeState(new MoveState(charController, stateMachine));
                if (!collisionInfo.SlippySlope)
                    charController.SetVelocity(Vector3.Project(movementInfo.velocity, inputInfo.leftStickToSlope), false);
                charController.fxManager.FootDustPlay();

            }
            else if (climbUp)
            {
                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.jump);
                stateMachine.SetRemainingAerialJumps(charController.CharData.Jump.MaxAerialJumps);

                state.shouldPlayJumpSound = false;
                Vector3 jumpDirection = Vector3.zero;
                state.SetJumpStrengthModifierFromState(wallRunData.JumpStrengthModifierLedgeGrab);

                stateMachine.ChangeState(state);
            }
            //no wall or stick released => fall
            else if ((!collisionInfo.side && noWallCounter > 5) || corner || timerToUnstick > wallRunData.TimeToUnstick)
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
            }
            //dash
            else if (inputInfo.dashButtonDown && !stateMachine.CheckStateLocked(ePlayerState.dash))
            {
                stateMachine.ChangeState(new DashState(charController, stateMachine, movementInfo.forward));
            }
            //jump
            else if (inputInfo.jumpButtonDown)
            {
                
                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.jump);
                stateMachine.SetRemainingAerialJumps(charController.CharData.Jump.MaxAerialJumps);
                Vector3 parallelDir = Vector3.ProjectOnPlane(movementInfo.velocity / 10, charController.MyTransform.up);
                Vector3 jumpDirection = Vector3.zero;
                if (!ledgeGrab)
                {
                    jumpDirection = Vector3.ProjectOnPlane(lastWallNormal + (parallelDir.sqrMagnitude > .25f ? parallelDir : Vector3.zero), charController.MyTransform.up).normalized;
                    charController.MyTransform.rotation = Quaternion.LookRotation(jumpDirection, charController.MyTransform.up);
                    state.SetJumpDirection(charController.TurnSpaceToLocal(jumpDirection));
                } else
                {
                    state.SetJumpStrengthModifierFromState(wallRunData.JumpStrengthModifierLedgeGrab);
                }
                state.SetTimerAirControl(wallRunData.TimerBeforeAirControl);

                stateMachine.ChangeState(state);
            }
            else if (inputInfo.echoButtonTimePressed > 1f && !stateMachine.CheckStateLocked(ePlayerState.phantom) && charController.PlayerController.InteractionController.currentEcho != null)
            {
                stateMachine.ChangeState(new PhantomState(charController, stateMachine), true);
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
                {
                    if (Vector3.Angle(lastWallNormal, collisionInfo.currentWallNormal) > 2f && lastWallNormal != Vector3.zero)
                        corner = true;
                    else 
                        lastWallNormal = collisionInfo.currentWallNormal;
                }

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

            ledgeGrab = !Physics.Raycast(charController.MyTransform.position + charController.tempPhysicsHandler.playerAngle * (charController.tempPhysicsHandler.center + charController.tempPhysicsHandler.capsuleHeightModifier / 2)
                                                    , -lastWallNormal, charController.tempPhysicsHandler.radius * 2f, charController.tempPhysicsHandler.collisionMask);
           

            //********************************
            //the direction along the wall
            Vector3 wallRunDir = Vector3.zero;
            if (firstFrame)
            {
                wallRunDir = Vector3.ProjectOnPlane(movementInfo.velocity * wallRunData.SpeedMultiplier, lastWallNormal);

                /*
                charController.SetVelocity(Quaternion.AngleAxis(Vector3.SignedAngle(-lastWallNormal, Vector3.ProjectOnPlane(movementInfo.velocity, charController.MyTransform.up), charController.MyTransform.up)*.8f, lastWallNormal) 
                    * charController.MyTransform.up * movementInfo.velocity.magnitude, false);

                Debug.Log("wallrun angle : " + Vector3.Angle(-lastWallNormal, Vector3.ProjectOnPlane(movementInfo.velocity, charController.MyTransform.up)));
                
                if (Vector3.Angle(lastWallNormal, Vector3.ProjectOnPlane(movementInfo.velocity, charController.MyTransform.up)) < 100f && movementInfo.velocity.sqrMagnitude > wallRunData.SpeedRequiredForHorizontalWallRun){
                    Debug.Log("hori wall run");
                }*/
            }
            
            
            //translate the direction to local space
            Vector3 localWallRunDir = charController.TurnSpaceToLocal(wallRunDir);

            localWallRunDir += Vector3.ProjectOnPlane(inputInfo.leftStickToCamera, lastWallNormal) * wallRunData.Speed;
            Vector3 acceleration = localWallRunDir;

            charController.animator.SetFloat("WallRunSpeed", Vector3.ProjectOnPlane(localWallRunDir, charController.MyTransform.up).sqrMagnitude * (Vector3.Dot(lastWallNormal, charController.MyTransform.right) > 0 ? 1 : -1)/80f);
            charController.animator.SetFloat("VerticalSpeed", Vector3.Project(localWallRunDir, charController.MyTransform.up).sqrMagnitude);

            //********************************

            //wall hugging
            //Vector3 wallHugging = charController.MyTransform.worldToLocalMatrix.MultiplyVector(-lastWallNormal) * 4f * (noWallCounter + 1);


            var result = new StateReturnContainer()
            {
                CanTurnPlayer = false,
                Acceleration = acceleration,
                GravityMultiplier = wallRunData.GravityModifier,
                TransitionSpeed = wallRunData.TransitionSpeed
            };


            if (ledgeGrab)
            {
                //result.PlayerForward = -lastWallNormal;

                Debug.DrawRay(charController.MyTransform.position + charController.tempPhysicsHandler.playerAngle * (charController.tempPhysicsHandler.center - charController.tempPhysicsHandler.capsuleHeightModifier / 2)
                                                    - localWallRunDir.normalized * charController.tempPhysicsHandler.radius, -lastWallNormal, Color.black);

                climbUp = !Physics.Raycast(charController.MyTransform.position + charController.tempPhysicsHandler.playerAngle * (charController.tempPhysicsHandler.center - charController.tempPhysicsHandler.capsuleHeightModifier / 2)
                                                    - localWallRunDir.normalized * charController.tempPhysicsHandler.radius
                                                    , -lastWallNormal, charController.tempPhysicsHandler.radius * 1.1f, charController.tempPhysicsHandler.collisionMask);

                climbUp = climbUp && movementInfo.velocity.y > 0;

                //Debug.Log("LEFFDGE GRAOBING!!! new forward is  : " + result.PlayerForward);
            }
            else
            {
                climbUp = false;
                if (localWallRunDir != Vector3.zero)
                {
                    Vector3 projectedVelocity = Vector3.Project(charController.TurnLocalToSpace(localWallRunDir), Vector3.Cross(lastWallNormal, charController.MyTransform.up));
                    result.PlayerForward = Vector3.Lerp(-lastWallNormal, projectedVelocity, projectedVelocity.magnitude/10f);
                    //Debug.Log("new player forward is : " + result.PlayerForward);
                }
            }

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
            bool isAbilityActive = charController.PlayerModel.CheckAbilityActive(AbilityType.WallRun);

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