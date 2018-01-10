﻿using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class StandState : IState
    {
        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.stand; } }

        CharController charController;
        StateMachine stateMachine;
        CharData.StandData standData;

        //#############################################################################

        public StandState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            standData = charController.CharData.Stand;
        }

        //#############################################################################

        public void Enter()
        {
            Debug.Log("Enter State: Stand");
        }

        public void Exit()
        {
            Debug.Log("Exit State: Stand");
        }

        //#############################################################################

        public void HandleInput()
        {
            PlayerInputInfo inputInfo = charController.InputInfo;
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            if (inputInfo.jumpButtonDown)
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, true));
            }
            else if (!inputInfo.leftStickAtZero)
            {
                stateMachine.ChangeState(new MoveState(charController, stateMachine));
            }

            else if (Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) > charController.CharData.General.MaxSlopeAngle)
            {
                stateMachine.ChangeState(new SlideState(charController, stateMachine));
            }
            else if (!collisionInfo.below)
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, false));
            }
        }

        public StateReturnContainer Update(float dt)
        {
            var result = new StateReturnContainer
            {
                CanTurnPlayer = true,
                IgnoreGravity = true,

                Acceleration = Vector3.zero,
                TransitionSpeed = standData.TransitionSpeed
            };

            return result;
        }

        //#############################################################################
    }
} //end of namespace