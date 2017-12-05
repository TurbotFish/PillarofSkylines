using Game.Player.CharacterController.EnterArgs;

namespace Game.Player.CharacterController.States
{
    public class EmptyState : IState
    {
        public ePlayerState StateId { get { return ePlayerState.empty; } }

        public bool CheckCanEnterState() { return true; }

        public void Enter(BaseEnterArgs enterArgs) { }

        public void Exit() { }

        public void HandleInput() { }

        public void Update(float dt) { }
    }
} //end of namespace