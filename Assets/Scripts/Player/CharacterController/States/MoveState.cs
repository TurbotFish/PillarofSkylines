using System.Collections;
using System.Collections.Generic;
using Game.Player.CharacterController.Containers;
using Game.Player.CharacterController.EnterArgs;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class MoveState : IState
    {
        //#############################################################################

        public ePlayerState StateId { get { return ePlayerState.move; } }

        CharController charController;
        StateMachine stateMachine;
        CharData.MoveData moveData;

        //#############################################################################

        public MoveState(CharController charController, StateMachine stateMachine)
        {
            this.charController = charController;
            this.stateMachine = stateMachine;
            moveData = charController.CharData.Move;
        }

        //#############################################################################

        public void Enter(BaseEnterArgs enterArgs)
        {
            Debug.Log("Enter State: Move");
        }

        public void Exit()
        {
            Debug.Log("Exit State: Move");
        }

        //#############################################################################

        public void HandleInput(PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            if (inputInfo.leftStickAtZero)
            {
                stateMachine.ChangeState(new StandEnterArgs(StateId));
            }
        }

        public StateReturnContainer Update(float dt, PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            var result = new StateReturnContainer();

            //if (!collisionInfo.below)
            //{
            //    stateMachine.ChangeState(new FallEnterArgs(StateId));
            //}
            //else
            //{
                if (!charController.CanTurnPlayer)
                {
                    result.CanTurnPlayer = true;
                }

                result.DesiredVelocity = inputInfo.leftStickToSlope * moveData.Speed * (inputInfo.sprintButton ? moveData.SprintCoefficient : 1);
            //}

            return result;
        }

        //#############################################################################
    }
} //end of namespace