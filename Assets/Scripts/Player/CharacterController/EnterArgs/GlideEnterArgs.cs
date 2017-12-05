
namespace Game.Player.CharacterController.EnterArgs
{
    public class GlideEnterArgs : BaseEnterArgs
    {
        public override ePlayerState NewState { get { return ePlayerState.glide; } }

        public GlideEnterArgs(ePlayerState previousState) : base(previousState)
        {

        }
    }
} //end of namespace