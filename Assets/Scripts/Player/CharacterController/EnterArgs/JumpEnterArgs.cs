using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.EnterArgs
{
    public class JumpEnterArgs : IEnterArgs
    {
        public ePlayerState NewState { get { return ePlayerState.jump; } }
        public ePlayerState PreviousState { get; private set; }

        public int RemainingAerialJumps { get; private set; }
        public bool RemainingAerialJumpsSet { get; private set; }

        public JumpEnterArgs(ePlayerState previousState)
        {
            PreviousState = previousState;
        }

        public JumpEnterArgs(ePlayerState previousState, int remainingAerialJumps)
        {
            PreviousState = previousState;

            RemainingAerialJumps = remainingAerialJumps;
            RemainingAerialJumpsSet = true;
        }
    }
}