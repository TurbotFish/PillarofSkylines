﻿using Game.Player.CharacterController.Containers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class AirState : IState
    {
        public enum eAirStateMode { fall, jump, aerialJump };

        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.air; } }

        CharController charController;
        StateMachine stateMachine;

        CharData.JumpData jumpData;
        CharData.FallData fallData;

        readonly eAirStateMode mode;
        int remainingAerialJumps = 0;
        float jumpTimer = 0;
        Vector3 jumpDirection = Vector3.zero;

        bool initializing;
        bool firstUpdate;

        //#############################################################################

        /// <summary>
        /// Constructor for AirState. Default mode is "fall".
        /// </summary>
        public AirState(CharController charController, StateMachine stateMachine, eAirStateMode mode)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            jumpData = charController.CharData.Jump;
            fallData = charController.CharData.Fall;

            this.mode = mode;

            initializing = true;
        }

        //#############################################################################

        public void SetRemainingAerialJumps(int jumps)
        {
            if (!initializing)
            {
                return;
            }

            remainingAerialJumps = jumps;
        }

        public void SetJumpTimer(float jumpTimer)
        {
            if (!initializing)
            {
                return;
            }

            this.jumpTimer = jumpTimer;
        }

        public void SetJumpDirection(Vector3 direction)
        {
            if (!initializing)
            {
                return;
            }

            jumpDirection = direction.normalized;
        }

        //#############################################################################

        public void Enter()
        {
            Debug.LogFormat("Enter State: Air - {0}", mode.ToString());

            initializing = false;
            firstUpdate = true;

            Utilities.EventManager.WindTunnelPartEnteredEvent += OnWindTunnelPartEnteredEventHandler;
        }

        public void Exit()
        {
            Debug.LogFormat("Exit State: Air - {0}", mode.ToString());

            Utilities.EventManager.WindTunnelPartEnteredEvent -= OnWindTunnelPartEnteredEventHandler;
        }

        //#############################################################################

        public void HandleInput()
        {
            PlayerInputInfo inputInfo = charController.InputInfo;
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            //jump
            if (inputInfo.jumpButtonDown)
            {
                if (mode == eAirStateMode.fall && jumpTimer > 0)
                {
                    var state = new AirState(charController, stateMachine, eAirStateMode.jump);
                    state.SetRemainingAerialJumps(jumpData.MaxAerialJumps);

                    stateMachine.ChangeState(state);
                }
                else if (remainingAerialJumps > 0 && charController.PlayerModel.CheckAbilityActive(eAbilityType.DoubleJump))
                {
                    var state = new AirState(charController, stateMachine, eAirStateMode.aerialJump);
                    state.SetRemainingAerialJumps(remainingAerialJumps - 1);

                    stateMachine.ChangeState(state);
                }
            }
            //dash
            else if (inputInfo.dashButtonDown && !stateMachine.CheckStateLocked(ePlayerState.dash))
            {
                stateMachine.ChangeState(new DashState(charController, stateMachine, movementInfo.forward));
            }
            //glide
            else if (inputInfo.sprintButtonDown && !stateMachine.CheckStateLocked(ePlayerState.glide))
            {
                stateMachine.ChangeState(new GlideState(charController, stateMachine));
            }
            //landing on slope
            else if (collisionInfo.below && (Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) > charController.CharData.General.MaxSlopeAngle || collisionInfo.SlippySlope))
            {
                stateMachine.ChangeState(new SlideState(charController, stateMachine));
            }
            //landing
            else if (collisionInfo.below)
            {
                stateMachine.ChangeState(new StandState(charController, stateMachine));
            }
            //wall- run/drift
            else if (collisionInfo.side)
            {
                if (movementInfo.velocity.y <= 0 && WallDriftState.CanEnterWallDrift(charController, true))
                {
                    stateMachine.ChangeState(new WallDriftState(charController, stateMachine));
                }
                else if (movementInfo.velocity.y > 0 && WallRunState.CheckCanEnterWallRun(charController))
                {
                    stateMachine.ChangeState(new WallRunState(charController, stateMachine));
                }
            }
        }

        public StateReturnContainer Update(float dt)
        {
            PlayerInputInfo inputInfo = charController.InputInfo;
            PlayerMovementInfo movementInfo = charController.MovementInfo;

            if (jumpTimer > 0)
            {
                jumpTimer -= dt;
            }

            var result = new StateReturnContainer()
            {
                CanTurnPlayer = true,
                keepVerticalMovement = true
            };

            //first update for jump (initial force)
            if (firstUpdate && (mode == eAirStateMode.jump || mode == eAirStateMode.aerialJump))
            {
                float jumpStrength = jumpData.Strength;

                if (mode == eAirStateMode.aerialJump)
                {
                    jumpStrength *= jumpData.AerialJumpCoeff;
					charController.aerialJumpFX.Play();
                }

                Vector3 direction = jumpDirection == Vector3.zero ? Vector3.up : jumpDirection;

                charController.AddExternalVelocity((direction) * jumpStrength, false, false);
                result.resetVerticalVelocity = true;
                //result.Acceleration = (movementInfo.velocity * 0.05f + Vector3.up) * jumpStrength;
                //result.TransitionSpeed = 1 / dt;

                firstUpdate = false;
            }
            //falling
            else if (mode == eAirStateMode.fall || movementInfo.velocity.y < 0)
            {
                result.MaxSpeed = fallData.MaxSpeed;
                result.TransitionSpeed = fallData.TransitionSpeed;
                result.Acceleration = inputInfo.leftStickToCamera * fallData.Speed;
            }
            //jumping
            else if (mode == eAirStateMode.jump || mode == eAirStateMode.aerialJump)
            {
                float minJumpStrength = jumpData.MinStrength;

                if (mode == eAirStateMode.aerialJump)
                {
                    minJumpStrength *= jumpData.AerialJumpCoeff;
                }

                result.Acceleration = inputInfo.leftStickToCamera * jumpData.Speed;

                if (!inputInfo.jumpButton && movementInfo.velocity.y > minJumpStrength)
                {
                    charController.SetVelocity(new Vector3(movementInfo.velocity.x, minJumpStrength, movementInfo.velocity.z), false);
                    result.Acceleration += Vector3.down * (movementInfo.velocity.y - minJumpStrength) * (0.1f / dt);
                }

                result.TransitionSpeed = jumpData.TransitionSpeed;
            }
            //error
            else
            {
                Debug.LogError("error!");
            }

            return result;
        }

        //#############################################################################

        void OnWindTunnelPartEnteredEventHandler(object sender, Utilities.EventManager.WindTunnelPartEnteredEventArgs args)
        {
            stateMachine.ChangeState(new WindTunnelState(charController, stateMachine));
        }

        //#############################################################################
    }
}