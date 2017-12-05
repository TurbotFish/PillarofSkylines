
namespace Game.Player.CharacterController.States
{
    public interface IState
    {
        ePlayerState StateId { get; }

        bool CheckCanEnterState();

        void Update(float dt);
        void HandleInput();

        void Enter(EnterArgs.BaseEnterArgs enterArgs);
        void Exit();
    }
} //end of namespace