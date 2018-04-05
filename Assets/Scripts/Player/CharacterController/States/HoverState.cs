using System.Collections;
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


        Vector3 gravityToResetTo;
        int nbrOfBlocksFeedback = 10;
        float timer;
        bool shouldResetGravity = true;

        //#############################################################################

        public HoverState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            moveData = charController.CharData.Move;
            hoverData = charController.CharData.Hover;
            timer = hoverData.Duration;
            gravityToResetTo = new Vector3(0f, -1f, 0f);
        }

        public HoverState(CharController charController, StateMachine stateMachine, Vector3 gravityReset)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            moveData = charController.CharData.Move;
            hoverData = charController.CharData.Hover;
            timer = hoverData.Duration;
            gravityToResetTo = gravityReset;
        }

        //#############################################################################

        public void Enter()
        {
            Debug.Log("Enter State: Hover");
            ParticleSystem.MainModule mainMod = charController.hoverFX.main;
            mainMod.duration = hoverData.Duration;
            charController.hoverFX.Play();
        }

        public void Exit()
        {
            Debug.Log("Exit State: Hover");
            /*PlayerMovementInfo movementInfo = charController.MovementInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;
            RaycastHit hit;
            if (Physics.Raycast(movementInfo.position, -movementInfo.up, out hit, hoverData.MaxHeight, collisionInfo.collisionLayer))
            {
                if (hit.collider.CompareTag("Gravifloor"))
                {
                    charController.ChangeGravityDirection(gravityToResetTo);
                }
            }*/
            charController.hoverFX.Stop();
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
            //jetpack
            else if (inputInfo.jetpackButtonDown && !stateMachine.CheckStateLocked(ePlayerState.jetpack))
            {
                stateMachine.ChangeState(new JetpackState(charController, stateMachine));
            }
            else if (collisionInfo.below)
            {
                stateMachine.ChangeState(new MoveState(charController, stateMachine));
            }
            else if (Physics.Raycast(movementInfo.position, -movementInfo.up, hoverData.MaxHeight, collisionInfo.collisionLayer) || inputInfo.leftStickAtZero || inputInfo.sprintButtonUp)
            {
                stateMachine.ChangeState(new AirState(charController, stateMachine, AirState.eAirStateMode.fall));
            }
            else if (inputInfo.rightStickButtonDown && charController.graviswapAvailable)
            {
                stateMachine.ChangeState(new GraviSwapState(charController, stateMachine), true);
            }
            else if (inputInfo.echoButtonTimePressed > 1f && !stateMachine.CheckStateLocked(ePlayerState.phantom))
            {
                stateMachine.ChangeState(new PhantomState(charController, stateMachine), true);
            }
        }

        public StateReturnContainer Update(float dt)
        {
            /*
            ParticleSystem parts = Object.Instantiate(charController.hoverFX, charController.MyTransform.position, charController.MyTransform.rotation);
            parts.transform.localScale *= (timer / hoverData.Duration) * 5;
            parts.Play();
            */

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
