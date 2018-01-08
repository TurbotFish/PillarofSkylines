
using Game.Player.CharacterController.Containers;

namespace Game.Player.CharacterController.States
{
    public interface IState
    {
        ePlayerState StateId { get; }

        StateReturnContainer Update(float dt);
        void HandleInput();

        void Enter();
        void Exit();
    }
} //end of namespace