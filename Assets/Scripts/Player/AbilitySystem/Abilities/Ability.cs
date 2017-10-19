using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Player.AbilitySystem
{
    [Serializable]
    public abstract class Ability
    {
        //type
        public abstract eAbilityType Type { get; }

        //group
        [SerializeField]
        eAbilityGroup group;

        public eAbilityGroup Group { get { return this.group; } }

        //activation price
        [SerializeField]
        int activationPrice;

        public int ActivationPrice { get { return this.activationPrice; } }

        [Header("UI")]

        //icon
        [SerializeField]
        Sprite icon;

        public Sprite Icon { get { return this.icon; } }

        //name
        [SerializeField]
        string name;

        public string Name { get { return this.name; } }

        //description
        [SerializeField]
        string description;

        public string Description { get { return this.description; } }

        

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