using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using Game.Player.CharacterController.EnterArgs;
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

        public DashState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            dashData = charController.CharData.Dash;
        }

        //#############################################################################

        public void Enter(BaseEnterArgs enterArgs)
        {
            if(enterArgs.NewState != ePlayerState.dash)
            {
                Debug.LogError("Dash state entered with wrong arguments!");
                stateMachine.ChangeState(new StandEnterArgs(StateId));
                return;
            }

            var args = enterArgs as DashEnterArgs;
            forward = args.Forward.normalized;

            charController.PlayerModel.FlagAbility(eAbilityType.Dash);
            timer = dashData.Time;
        }

        public void Exit()
        {
            charController.PlayerModel.UnflagAbility(eAbilityType.Dash);

            stateMachine.SetStateCooldown(new StateCooldown(StateId, dashData.Cooldown));
        }

        //#############################################################################

        public void HandleInput(PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
        }

        public StateReturnContainer Update(float dt, PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            timer -= dt;

            var result = new StateReturnContainer();

            result.CanTurnPlayer = false;
            result.IgnoreGravity = true;
            result.PlayerForward = forward;
            result.Acceleration = forward * dashData.Speed;
            result.MaxSpeed = result.Acceleration.magnitude + 1; //+1 is just for security
            result.TransitionSpeed = dashData.TransitionSpeed;

            if(timer <= 0)
            {
                stateMachine.ChangeState(new StandEnterArgs(StateId));
            }

            return result;
        }

        //#############################################################################
    }
} //end of namespace