
namespace Game.Player.CharacterController.EnterArgs
{
    public class FallEnterArgs : IEnterArgs
    {
        public ePlayerState NewState { get { return ePlayerState.fall; } }
        public ePlayerState PreviousState { get; private set; }

        public bool CanJump { get; private set; }
        public float JumpTimer { get; private set; }

        public FallEnterArgs(ePlayerState previousState)
        {
            PreviousState = previousState;

            CanJump = false;
        }

        public FallEnterArgs(ePlayerState previousState, float jumpTimer)
        {
            PreviousState = previousState;

            CanJump = true;
            JumpTimer = jumpTimer;
        }
    }
} //end of namespace