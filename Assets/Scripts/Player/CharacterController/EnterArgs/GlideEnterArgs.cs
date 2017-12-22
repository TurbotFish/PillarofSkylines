
namespace Game.Player.CharacterController.EnterArgs
{
    public class GlideEnterArgs : IEnterArgs
    {
        public ePlayerState NewState { get { return ePlayerState.glide; } }
        public ePlayerState PreviousState { get; private set; }

        public GlideEnterArgs(ePlayerState previousState)
        {
            PreviousState = previousState;
        }
    }
} //end of namespace