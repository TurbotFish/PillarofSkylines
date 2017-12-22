namespace Game.Player.CharacterController.EnterArgs
{
    public class SlideEnterArgs : IEnterArgs
    {
        public ePlayerState NewState { get { return ePlayerState.slide; } }
        public ePlayerState PreviousState { get; private set; }

        public SlideEnterArgs(ePlayerState previousState)
        {
            PreviousState = previousState;
        }
    }
} //end of namespace