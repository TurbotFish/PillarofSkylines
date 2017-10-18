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
        public int Favours { get; set; }

        //
        List<eAbilityGroup> unlockedAbilityGroups = new List<eAbilityGroup>();
        List<eAbilityType> activatedAbilities = new List<eAbilityType>();
        List<eAbilityType> blockedAbilities = new List<eAbilityType>();


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

        #region ability group unlocking methods

        /// <summary>
        /// This method checks if an ability group is unlocked
        /// </summary>
        /// <returns>Returns true if the ability group is unlocked, false otherwise.</returns>
        public bool CheckAbilityGroupUnlocked(eAbilityGroup abilityGroup)
        {
            if (this.unlockedAbilityGroups.Contains(abilityGroup))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method unlocks an ability group.
        /// </summary>
        /// <returns>Returns true if the ability group is now unlocked, false otherwise.</returns>
        public bool UnlockAbilityGroup(eAbilityGroup abilityGroup)
        {
            if (!this.unlockedAbilityGroups.Contains(abilityGroup))
            {
                this.unlockedAbilityGroups.Add(abilityGroup);
                return true;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// This method locks an ability group and deactivates all abilities that are part of it.
        /// </summary>
        /// <returns>Returns true if the ability group is now locked, false otherwise.</returns>
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
                    this.blockedAbilities.Remove(abilityType);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion ability group unlocking methods

        //###########################################################
        //###########################################################

        #region ability activation methods

        /// <summary>
        /// This method checks if an ability is currently activated.
        /// </summary>
        /// <returns>Returns true if the ability is activated, false otherwise.</returns>
        public bool CheckAbilityActive(eAbilityType abilityType)
        {
            if (this.activatedAbilities.Contains(abilityType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method activates an ability and removes favours equal to its activation price
        /// </summary>
        /// <returns>Returns true if the ability is now activated, false otherwise.</returns>
        public bool ActivateAbility(eAbilityType abilityType)
        {
            var ability = this.abilityData.GetAbility(abilityType);

            if (this.activatedAbilities.Contains(abilityType))
            {
                return true;
            }
            else if (CheckAbilityGroupUnlocked(ability.Group) && this.Favours >= ability.ActivationPrice)
            {
                this.Favours -= ability.ActivationPrice;
                this.activatedAbilities.Add(abilityType);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method deactivates an ability and gives the player back the amount of favours it cost to activate it.
        /// </summary>
        /// <returns>Returns true if the ability is now deactivated, false otherwise.</returns>
        public bool DeactivateAbility(eAbilityType abilityType)
        {
            var ability = this.abilityData.GetAbility(abilityType);

            if (!this.activatedAbilities.Contains(abilityType))
            {
                return true;
            }
            else if (!this.blockedAbilities.Contains(abilityType))
            {
                this.Favours += ability.ActivationPrice;
                this.activatedAbilities.Remove(abilityType);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion ability activation methods

        //###########################################################
        //###########################################################

        #region ability blocking methods

        /// <summary>
        /// This method checks if an ability is currently in use by the player.
        /// </summary>
        /// <returns>Returns true if the ability is blocked, false otherwise.</returns>
        public bool CheckAbilityBlocked(eAbilityType abilityType)
        {
            if (this.blockedAbilities.Contains(abilityType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method flags an ability as in use by the player. This means that it cannot be deactivated in the ability menu.
        /// </summary>
        /// <returns>Returns true if the ability is now blocked, false otherwise.</returns>
        public bool BlockAbility(eAbilityType abilityType)
        {
            if (this.blockedAbilities.Contains(abilityType))
            {
                return true;
            }
            else if (this.activatedAbilities.Contains(abilityType))
            {
                this.blockedAbilities.Add(abilityType);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method flags an ability as NOT in use by the player. This means that it CAN be deactivated in the ability menu.
        /// </summary>
        /// <returns>Returns true if the ability is now unblocked, false otherwise.</returns>
        public bool UnblockAbility(eAbilityType abilityType)
        {
            if (this.blockedAbilities.Contains(abilityType))
            {
                this.blockedAbilities.Remove(abilityType);
                return true;
            }
            else
            {
                return true;
            }
        }

        #endregion ability usage methods

        //###########################################################
    }
} //end of namespace