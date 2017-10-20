using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Player.AbilitySystem
{
    [Serializable]
    public class DoubleJump : Ability
    {
        public override eAbilityType Type { get { return eAbilityType.DoubleJump; } }

        //###########################################################

        public override void OnValidate()
        {
            base.OnValidate();
        }
    }
} //end of namespace