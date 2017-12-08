using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using Game.Player.CharacterController.EnterArgs;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class StandState : IState
    {
        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.stand; } }

        CharController charController;
        StateMachine stateMachine;

        //#############################################################################

        public StandState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
        }

        //#############################################################################

        public void Enter(BaseEnterArgs enterArgs)
        {
            Debug.Log("Enter State: Stand");
        }

        public void Exit()
        {
            Debug.Log("Exit State: Stand");
        }

        //#############################################################################

        public void HandleInput(PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            if (!inputInfo.leftStickAtZero)
            {
                stateMachine.ChangeState(new MoveEnterArgs(StateId));
            }
        }

        public StateReturnContainer Update(float dt, PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            var result = new StateReturnContainer();

            if (!charController.CanTurnPlayer)
            {
                result.CanTurnPlayer = true;
            }

            result.DesiredVelocity = Vector3.zero;
            result.TransitionSpeed = 8;

            return result;
        }

        //#############################################################################
    }
} //end of namespace