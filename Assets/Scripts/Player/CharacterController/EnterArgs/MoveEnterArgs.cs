using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.EnterArgs
{
    public class MoveEnterArgs : BaseEnterArgs
    {
        public override ePlayerState NewState { get { return ePlayerState.move; } }

        public MoveEnterArgs(ePlayerState previousState) : base(previousState)
        {

        }
    }
} //end of namespace