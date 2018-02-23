using UnityEngine;
using System;

namespace Game.Model
{
    [Serializable]
    public class Ability
    {
        //###########################################################

        //type
        public eAbilityType Type { get; private set; }

        //group
        [SerializeField]
        eAbilityGroup group;
        public eAbilityGroup Group { get { return group; } }

        //activation price
        [SerializeField]
        int activationPrice;
        public int ActivationPrice { get { return activationPrice; } }

        [Header("UI")]

        //icon
        [SerializeField]
        Sprite icon;
        public Sprite Icon { get { return icon; } }

        //name
        [SerializeField]
        string name;
        public string Name { get { return name; } }

        //description
        [SerializeField]
        string description;
        public string Description { get { return description; } }

        //###########################################################

        public Ability(eAbilityType abilityType)
        {
            Type = abilityType;
        }

        //###########################################################

        public virtual void OnValidate()
        {
            if (activationPrice < 1)
            {
                activationPrice = 1;
            }
        }

        //###########################################################
    }
} //end of namespace