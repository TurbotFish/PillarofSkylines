using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Player.AbilitySystem
{
    [Serializable]
    public class Glide : Ability
    {
        public Glide() : base(eAbilityType.Glide)
        {

        }

        //###########################################################

        public override void OnValidate()
        {
            base.OnValidate();
        }
    }
} //end of namespace