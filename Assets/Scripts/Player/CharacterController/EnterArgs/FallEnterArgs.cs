
namespace Game.Player.CharacterController.EnterArgs
{
    public class FallEnterArgs : BaseEnterArgs
    {
        public override ePlayerState NewState { get { return ePlayerState.fall; } }

        public bool CanJump { get; private set; }
        public float JumpTimer { get; private set; }

        public FallEnterArgs(ePlayerState previousState) : base(previousState)
        {
            CanJump = false;
        }

        public FallEnterArgs(ePlayerState previousState, float jumpTimer) : base(previousState)
        {
            CanJump = true;
            JumpTimer = jumpTimer;
        }
    }
} //end of namespace