using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// The list of the abilities of the player
    /// </summary>
    public enum eAbilityType
    {
        DoubleJump,
        Glide,
        Dash
    }

    public enum eAbilityGroup
    {
        test
    }

    /// <summary>
    /// This class stores data about the player: favours, abilities, stats, ...
    /// </summary>
    public class PlayerModel : MonoBehaviour
    {
        //ability data
        [SerializeField]
        AbilitySystem.AbilityData abilityData;

        public AbilitySystem.AbilityData AbilityData { get { return this.abilityData; } }



        //
        List<eAbilityGroup> unlockedAbilityGroups = new List<eAbilityGroup>();
        List<eAbilityType> activatedAbilities = new List<eAbilityType>();
        public int Favours { get; set; }

        //###########################################################

        #region monobehaviour methods

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion monobehaviour methods

        //###########################################################
        //###########################################################

        #region ability group methods

        public bool CheckAbilityGroupUnlocked(eAbilityGroup abilityGroup)
        {
            if (this.unlockedAbilityGroups.Contains(abilityGroup))
            {
                return true;
            }

            return false;
        }

        public bool UnlockAbilityGroup(eAbilityGroup abilityGroup)
        {
            if (this.unlockedAbilityGroups.Contains(abilityGroup))
            {
                return true;
            }
            else
            {
                this.unlockedAbilityGroups.Add(abilityGroup);
                return true;
            }
        }

        public bool LockAbilityGroup(eAbilityGroup abilityGroup)
        {
            if (this.unlockedAbilityGroups.Contains(abilityGroup))
            {
                this.unlockedAbilityGroups.Remove(abilityGroup);

                var abilitiesToDeactivate = new List<eAbilityType>();
                foreach (var abilityType in this.activatedAbilities)
                {
                    var ability = this.abilityData.GetAbility(abilityType);

                    if (ability.Group == abilityGroup)
                    {
                        abilitiesToDeactivate.Add(abilityType);
                    }
                }

                foreach (var abilityType in abilitiesToDeactivate)
                {
                    this.activatedAbilities.Remove(abilityType);
                }

                return true;
            }

            return false;
        }

        #endregion ability group methods

        //###########################################################
        //###########################################################

        #region ability methods

        public bool CheckAbilityActive(eAbilityType abilityType)
        {
            if (this.activatedAbilities.Contains(abilityType))
            {
                return true;
            }

            return false;
        }

        public bool ActivateAbility(eAbilityType abilityType)
        {
            var ability = this.abilityData.GetAbility(abilityType);

            if (CheckAbilityGroupUnlocked(ability.Group) && this.Favours >= ability.ActivationPrice)
            {
                this.Favours -= ability.ActivationPrice;
                this.activatedAbilities.Add(abilityType);
                return true;
            }

            return false;
        }

        public bool DeactivateAbility(eAbilityType abilityType)
        {
            var ability = this.abilityData.GetAbility(abilityType);

            if (this.activatedAbilities.Contains(abilityType))
            {
                this.Favours += ability.ActivationPrice;
                this.activatedAbilities.Remove(abilityType);
                return true;
            }

            return false;
        }

        #endregion ability methods

        //###########################################################
    }
} //end of namespace