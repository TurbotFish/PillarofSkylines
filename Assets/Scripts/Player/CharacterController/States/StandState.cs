using System.Collections;
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
        }

        public void Exit()
        {

        }

        //#############################################################################

        public void HandleInput()
        {
            PlayerInputInfo inputInfo = charController.InputInfo;
            PlayerMovementInfo movementInfo = charController.MovementInfo;
            CharacControllerRecu.CollisionInfo collisionInfo = charController.CollisionInfo;


            if (inputInfo.jumpButtonDown && !charController.isInsideNoRunZone)
            {
                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.jump);
                stateMachine.SetRemainingAerialJumps(charController.CharData.Jump.MaxAerialJumps);

                stateMachine.ChangeState(state);
            }
            else if (inputInfo.dashButtonDown && !stateMachine.CheckStateLocked(ePlayerState.dash) && !charController.isInsideNoRunZone)
            {
                stateMachine.ChangeState(new DashState(charController, stateMachine, movementInfo.forward));
            }
            else if (!collisionInfo.below)
			{
				var state = new AirState(charController, stateMachine, AirState.eAirStateMode.fall);
                stateMachine.SetRemainingAerialJumps(charController.CharData.Jump.MaxAerialJumps);
                state.SetJumpTimer(charController.CharData.Move.CanStillJumpTimer);
                stateMachine.ChangeState(state);
            }
            //jetpack
            else if (inputInfo.jetpackButtonDown && !stateMachine.CheckStateLocked(ePlayerState.jetpack) && !charController.isInsideNoRunZone)
            {
                stateMachine.ChangeState(new JetpackState(charController, stateMachine));
            }
            else if (Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) < charController.CharData.General.MinWallAngle 
                && (Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) > charController.CharData.General.MaxSlopeAngle && !collisionInfo.NotSlippySlope) 
                || collisionInfo.SlippySlope && Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) > 2f)
			{
				stateMachine.ChangeState(new SlideState(charController, stateMachine));
            }
            else if (Vector3.Angle(collisionInfo.currentGroundNormal, movementInfo.up) > charController.CharData.General.MinWallAngle)
            {
                var state = new AirState(charController, stateMachine, AirState.eAirStateMode.fall);
                stateMachine.SetRemainingAerialJumps(charController.CharData.Jump.MaxAerialJumps);

                stateMachine.ChangeState(state);
            }
            else if (!inputInfo.leftStickAtZero)
            {
                stateMachine.ChangeState(new MoveState(charController, stateMachine));
            }
            else if (inputInfo.rightStickButtonDown && !stateMachine.CheckStateLocked(ePlayerState.graviswap) && !charController.isInsideNoRunZone)
            {
                stateMachine.ChangeState(new GraviSwapState(charController, stateMachine), true);
            }
            else if (inputInfo.echoButtonTimePressed > 1f && !stateMachine.CheckStateLocked(ePlayerState.phantom) && charController.PlayerController.InteractionController.currentEcho != null)
            {
                stateMachine.ChangeState(new PhantomState(charController, stateMachine), true);
            }
        }

        public StateReturnContainer Update(float dt)
        {
            var result = new StateReturnContainer
            {
                CanTurnPlayer = true,
                IgnoreGravity = true,
                keepVerticalMovement = true,

                Acceleration = Vector3.zero,
                TransitionSpeed = standData.TransitionSpeed
            };
            
            if (charController.CollisionInfo.SlippySlope)
                result.TransitionSpeed = charController.CharData.Move.SlipperyGroundTransitionSpeed;

            return result;
        }

        //#############################################################################
    }
} //end of namespace