using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.EnterArgs
{
    public class StandEnterArgs : BaseEnterArgs
    {
        public override ePlayerState NewState { get { return ePlayerState.stand; } }

        public StandEnterArgs(ePlayerState previousState) : base(previousState)
        {

        }
    }
} //end of namespace