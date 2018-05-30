using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class DashState : IState
    {
        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.dash; } }

        CharController charController;
        StateMachine stateMachine;
        CharData.DashData dashData;

        float timer;
        Vector3 forward;

        //#############################################################################

        public DashState(CharController charController, StateMachine stateMachine, Vector3 forward)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            dashData = charController.CharData.Dash;

            this.forward = forward;
        }

        //#############################################################################

        public void Enter()
        {
			//Debug.Log("Enter State: Dash");
			charController.animator.SetTrigger("DashTrigger");
            charController.fxManager.DashPlay();
            timer = dashData.Time;
        }

        public void Exit()
        {
            //Debug.Log("Exit State: Dash");
            stateMachine.SetStateCooldown(new StateCooldown(StateId, dashData.Cooldown));
        }

        //#############################################################################

        public void HandleInput()
        {
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;

            if (collisionInfo.side && WallRunState.CheckCanEnterWallRun(charController))
            {
                stateMachine.ChangeState(new WallRunState(charController, stateMachine));
            }
        }

        public StateReturnContainer Update(float dt)
        {
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;
            PlayerInputInfo inputInfo = charController.InputInfo;
            timer -= dt;

            var result = new StateReturnContainer
            {
                CanTurnPlayer = false,
                IgnoreGravity = true,
                PlayerForward = forward,
                Acceleration = charController.TurnSpaceToLocal(forward * dashData.Speed * stateMachine.speedMultiplier)
            };
            result.MaxSpeed = result.Acceleration.magnitude + 1; //+1 is just for security
            result.TransitionSpeed = dashData.TransitionSpeed;
            //result.keepVerticalMovement = true;
            result.resetVerticalVelocity = true;
            if (timer <= 0)
            {
                if (collisionInfo.below)
                {
                    stateMachine.ChangeState(new MoveState(charController, stateMachine));
                } else
                {
                    if (inputInfo.glideButton && !stateMachine.CheckStateLocked(ePlayerState.glide))
                    {
                        stateMachine.ChangeState(new GlideState(charController, stateMachine));
                    } else
                    {
                        stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
                    }
                }
            }

            return result;
        }

        //#############################################################################
    }
} //end of namespace