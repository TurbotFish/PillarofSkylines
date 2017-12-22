using Game.Player.CharacterController.Containers;
using Game.Player.CharacterController.EnterArgs;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class EmptyState : IState
    {
        public ePlayerState StateId
        {
            get { return ePlayerState.empty; }
        }

        public bool CheckCanEnterState()
        {
            return true;
        }

        public void Enter(IEnterArgs enterArgs)
        {
            Debug.Log("Enter State: Empty");
        }

        public void Exit()
        {
            Debug.Log("Exit State: Empty");
        }

        public void HandleInput(PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            //Debug.Log("EmptyState: HandleInput");
        }

        public StateReturnContainer Update(float dt, PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo)
        {
            return new StateReturnContainer();
        }
    }
} //end of namespace