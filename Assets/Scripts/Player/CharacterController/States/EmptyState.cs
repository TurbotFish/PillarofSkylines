using Game.Player.CharacterController.Containers;
using Game.Player.CharacterController.EnterArgs;
using UnityEngine;

namespace Game.Player.CharacterController.States
{
    public class EmptyState : IState
    {
        public ePlayerState StateId { get { return ePlayerState.empty; } }

        public bool CheckCanEnterState() { return true; }

        public void Enter(BaseEnterArgs enterArgs) { }

        public void Exit() { }

        public void HandleInput(PlayerInputInfo inputInfo) { }

        public StateReturnContainer Update(float dt, PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo) { return new StateReturnContainer(); }
    }
} //end of namespace