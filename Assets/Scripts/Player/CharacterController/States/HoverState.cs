﻿using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class HoverState : IState
    {

        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.hover; } }

        CharController charController;
        StateMachine stateMachine;
        CharData.MoveData moveData;
        CharData.HoverData hoverData;

        float timer;

        //#############################################################################

        public HoverState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            moveData = charController.CharData.Move;
            hoverData = charController.CharData.Hover;
            timer = hoverData.Duration;
        }

        //#############################################################################

        public void Enter()
        {
            Debug.Log("Enter State: Hover");
        }

        public void Exit()
        {
            Debug.Log("Exit State: Hover");
        }

        //#############################################################################

        public void HandleInput()
        {
            PlayerInputInfo inputInfo = charController.InputInfo;
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            if (inputInfo.jumpButtonDown)
            {
                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.jump);
                stateMachine.SetRemainingAerialJumps(charController.CharData.Jump.MaxAerialJumps);

                stateMachine.ChangeState(state);
            }
            else if (inputInfo.dashButtonDown && !stateMachine.CheckStateLocked(ePlayerState.dash))
            {
                stateMachine.ChangeState(new DashState(charController, stateMachine, movementInfo.forward));
            }
            else if (collisionInfo.below)
            {
                stateMachine.ChangeState(new StandState(charController, stateMachine));
            }
            else if (inputInfo.leftStickAtZero || inputInfo.sprintButtonUp)
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
            }
        }

        public StateReturnContainer Update(float dt)
        {
            PlayerInputInfo inputInfo = charController.InputInfo;

            timer -= dt;

            if (timer <= 0)
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
            }

            var result = new StateReturnContainer
            {
                CanTurnPlayer = true,

                Acceleration = inputInfo.leftStickToSlope * moveData.Speed * (inputInfo.sprintButton ? moveData.SprintCoefficient : 1) * stateMachine.speedMultiplier,

                IgnoreGravity = true,
                MaxSpeed = moveData.MaxSpeed,
                TransitionSpeed = moveData.TransitionSpeed
            };

            if (charController.CollisionInfo.SlippySlope)
                result.TransitionSpeed = moveData.SlipperyGroundTransitionSpeed;

            return result;
        }

        //#############################################################################
    }
}