using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.EnterArgs
{
    public class JumpEnterArgs : BaseEnterArgs
    {
        public override ePlayerState NewState { get { return ePlayerState.jump; } }

        public JumpEnterArgs(ePlayerState previousState) : base(previousState)
        {

        }
    }
}