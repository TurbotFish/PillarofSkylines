
namespace Game.Player.CharacterController.EnterArgs
{
    public class FallEnterArgs : BaseEnterArgs
    {
        public override ePlayerState NewState { get { return ePlayerState.fall; } }

        public FallEnterArgs(ePlayerState previousState) : base(previousState)
        {

        }
    }
} //end of namespace