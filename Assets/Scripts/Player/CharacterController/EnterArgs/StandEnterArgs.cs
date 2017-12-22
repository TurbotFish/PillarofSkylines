using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.EnterArgs
{
    public class StandEnterArgs : IEnterArgs
    {
        public ePlayerState NewState { get { return ePlayerState.stand; } }
        public ePlayerState PreviousState { get; private set; }

        public StandEnterArgs(ePlayerState previousState)
        {
            PreviousState = previousState;
        }
    }
} //end of namespace