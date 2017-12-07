
using Game.Player.CharacterController.Containers;

namespace Game.Player.CharacterController.States
{
    public interface IState
    {
        ePlayerState StateId { get; }

        StateReturnContainer Update(float dt, PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo);
        void HandleInput(PlayerInputInfo inputInfo, PlayerMovementInfo movementInfo, CharacControllerRecu.CollisionInfo collisionInfo);

        void Enter(EnterArgs.BaseEnterArgs enterArgs);
        void Exit();
    }
} //end of namespace