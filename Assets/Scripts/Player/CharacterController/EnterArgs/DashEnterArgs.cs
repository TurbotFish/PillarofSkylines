using UnityEngine;

namespace Game.Player.CharacterController.EnterArgs
{
    public class DashEnterArgs : BaseEnterArgs
    {
        public override ePlayerState NewState { get { return ePlayerState.dash; } }

        public Vector3 Forward { get; private set; }

        public DashEnterArgs(ePlayerState previousState, Vector3 forward) : base(previousState)
        {
            Forward = forward;
        }
    }
} //end of namespace