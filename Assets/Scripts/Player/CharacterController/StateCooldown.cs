using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.CharacterController
{
    public class StateCooldown
    {
        public ePlayerState StateId { get; private set; }
        public float CooldownTime { get; private set; }

        public StateCooldown(ePlayerState stateId, float cooldownTime)
        {
            StateId = stateId;
            CooldownTime = cooldownTime;
        }

        /// <summary>
        /// Update the cooldown. Returns true if the cooldown is over, false otherwise.
        /// </summary>
        public bool Update(float dt)
        {
            CooldownTime -= dt;

            if (CooldownTime < 0)
            {
                return true;
            }

            return false;
        }
    }
} //end of namespace