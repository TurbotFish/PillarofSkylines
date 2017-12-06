using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController
{
    public class StateReturnValues
    {
        public Vector3 DeltaAcceleration { get; private set; }
        public bool CanTurnPlayer { get; private set; }

        public StateReturnValues(Vector3 deltaAcceleration, bool canTurnPlayer = true)
        {
            DeltaAcceleration = deltaAcceleration;
            CanTurnPlayer = canTurnPlayer;
        }
    }
} //end of namespace