using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController.EnterArgs
{
    public class WindTunnelEnterArgs : IEnterArgs
    {
        public ePlayerState NewState { get { return ePlayerState.windTunnel; } }
        public ePlayerState PreviousState { get; private set; }

        public WindTunnelEnterArgs(ePlayerState previousState)
        {
            PreviousState = previousState;
        }
    }
} //end of namespace