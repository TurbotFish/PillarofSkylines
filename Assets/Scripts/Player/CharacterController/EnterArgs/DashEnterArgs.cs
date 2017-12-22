using UnityEngine;

namespace Game.Player.CharacterController.EnterArgs
{
    public class DashEnterArgs : IEnterArgs
    {
        public ePlayerState NewState { get { return ePlayerState.dash; } }
        public ePlayerState PreviousState { get; private set; }

        public Vector3 Forward { get; private set; }

        public DashEnterArgs(ePlayerState previousState, Vector3 forward)
        {
            PreviousState = previousState;
            Forward = forward;
        }
    }
} //end of namespace