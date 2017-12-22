
namespace Game.Player.CharacterController.EnterArgs
{
    public interface IEnterArgs
    {
        ePlayerState PreviousState { get; }
        ePlayerState NewState { get; }
    }
} //end of namespace