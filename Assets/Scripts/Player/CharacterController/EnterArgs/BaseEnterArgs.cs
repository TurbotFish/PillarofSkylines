
namespace Game.Player.CharacterController.EnterArgs
{
    public abstract class BaseEnterArgs
    {
        public ePlayerState PreviousState { get; private set; }
        public abstract ePlayerState NewState { get; }

        protected BaseEnterArgs(ePlayerState previousState)
        {
            PreviousState = previousState;
        }
    }
} //end of namespace