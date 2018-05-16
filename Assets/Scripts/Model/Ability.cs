using UnityEngine;
using System;

namespace Game.Model
{
    [Serializable]
    public class Ability
    {
        //###########################################################

        [SerializeField, HideInInspector] public string title; //this is used by Unity to name the object in an inspector array!!!!

        //type
        [SerializeField, HideInInspector] private AbilityType type; 
        public AbilityType Type { get { return type; } }

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

        public Ability(AbilityType abilityType)
        {
            type = abilityType;
        }

        //###########################################################

        public virtual void OnValidate()
        {
            if (activationPrice < 1)
            {
                activationPrice = 1;
            }

            title = Type.ToString();
        }

        //###########################################################
    }
} //end of namespace