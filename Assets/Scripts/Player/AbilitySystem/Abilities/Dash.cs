using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Player.AbilitySystem
{
    [Serializable]
    public class Dash : Ability
    {
        public Dash() : base(eAbilityType.Dash)
        {

        }

        //###########################################################

        public override void OnValidate()
        {
            base.OnValidate();
        }
    }
} //end of namespace