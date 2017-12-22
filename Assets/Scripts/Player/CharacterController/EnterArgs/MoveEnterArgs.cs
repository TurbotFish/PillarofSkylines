﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.EnterArgs
{
    public class MoveEnterArgs : IEnterArgs
    {
        public ePlayerState NewState { get { return ePlayerState.move; } }
        public ePlayerState PreviousState { get; private set; }

        public MoveEnterArgs(ePlayerState previousState)
        {
            PreviousState = previousState;
        }
    }
} //end of namespace