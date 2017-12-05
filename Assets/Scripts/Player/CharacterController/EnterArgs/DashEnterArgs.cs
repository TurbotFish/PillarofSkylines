
namespace Game.Player.CharacterController.EnterArgs
{
    public class DashEnterArgs : BaseEnterArgs
    {
        public override ePlayerState NewState { get { return ePlayerState.dash; } }

        public DashEnterArgs(ePlayerState previousState) : base(previousState)
        {

        }
    }
} //end of namespace