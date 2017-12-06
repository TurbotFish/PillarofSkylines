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

        public StateReturnValues Update(float dt) { return new StateReturnValues(Vector3.zero); }
    }
} //end of namespace