using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Player.AbilitySystem
{
    [Serializable]
    public abstract class Ability
    {
        //group
        [SerializeField]
        eAbilityGroup group;

        public eAbilityGroup Group { get { return this.group; } }

        //activation price
        [SerializeField]
        int activationPrice;

        public int ActivationPrice { get { return this.activationPrice; } }

        //###########################################################

        public virtual void OnValidate()
        {
            if(this.activationPrice < 1)
            {
                this.activationPrice = 1;
            }
        }
    }
} //end of namespace