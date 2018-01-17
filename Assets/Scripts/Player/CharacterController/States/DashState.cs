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
            Debug.Log("Enter State: Dash");
            charController.animator.SetTrigger("DashTrigger");
            charController.dashParticles.Play();
            timer = dashData.Time;
        }

        public void Exit()
        {
            Debug.Log("Exit State: Dash");

            stateMachine.SetStateCooldown(new StateCooldown(StateId, dashData.Cooldown));
        }

        //#############################################################################

        public void HandleInput()
        {
        }

        public StateReturnContainer Update(float dt)
        {
            timer -= dt;

            var result = new StateReturnContainer();

            result.CanTurnPlayer = false;
            result.IgnoreGravity = true;
            result.PlayerForward = forward;
            result.Acceleration = forward * dashData.Speed;
            result.MaxSpeed = result.Acceleration.magnitude + 1; //+1 is just for security
            result.TransitionSpeed = dashData.TransitionSpeed;

            if (timer <= 0)
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
            }

            return result;
        }

        //#############################################################################
    }
} //end of namespace