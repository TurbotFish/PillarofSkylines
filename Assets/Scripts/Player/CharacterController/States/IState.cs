
using Game.Player.CharacterController.Containers;

namespace Game.Player.CharacterController.States
{
    public interface IState
    {
        ePlayerState StateId { get; }

        StateReturnContainer Update(float dt, PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo);
        void HandleInput(PlayerInputInfo inputInfo);

        void Enter(EnterArgs.BaseEnterArgs enterArgs);
        void Exit();
    }
} //end of namespace