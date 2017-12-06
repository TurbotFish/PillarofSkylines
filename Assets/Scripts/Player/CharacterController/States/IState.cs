
namespace Game.Player.CharacterController.States
{
    public interface IState
    {
        ePlayerState StateId { get; }

        StateReturnValues Update(float dt);
        void HandleInput(PlayerInputInfo inputInfo);

        void Enter(EnterArgs.BaseEnterArgs enterArgs);
        void Exit();
    }
} //end of namespace