﻿using System.Collections;
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
        Dash,
        TombFinder,
        WallRun
    }

    public enum eAbilityGroup
    {
        Default,
        GroupA
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

        //pillar data
        World.PillarData pillarData;
        public World.PillarData PillarData { get { return this.pillarData; } }

        //
        public bool hasNeedle;

        //
        int favours = 0;
        public int Favours
        {
            get { return this.favours; }
            set
            {
                this.favours = value;
                Utilities.EventManager.SendOnFavourAmountChangedEvent(this, new Utilities.EventManager.OnFavourAmountChangedEventArgs(this.favours));
            }
        }

        //ability variables
        List<eAbilityGroup> unlockedAbilityGroups = new List<eAbilityGroup>();
        List<eAbilityType> activatedAbilities = new List<eAbilityType>();
        List<eAbilityType> flaggedAbilities = new List<eAbilityType>();

        //
        List<World.ePillarId> destoyedPillars = new List<World.ePillarId>();

        //###########################################################

        public void InitializePlayerModel()
        {
            this.pillarData = Resources.Load<World.PillarData>("ScriptableObjects/PillarData");

            UnlockAbilityGroup(eAbilityGroup.Default);
        }

        //###########################################################
        //###########################################################

        #region monobehaviour methods

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.F2))
            {
                this.Favours++;
            }
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
                    this.flaggedAbilities.Remove(abilityType);
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

        public List<eAbilityType> GetAllActiveAbilities()
        {
            return new List<eAbilityType>(this.activatedAbilities);
        }

        /// <summary>
        /// This method activates an ability and removes favours equal to its activation price.
        /// Returns true if the ability is now activated, false otherwise.
        /// </summary>
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
        /// Returns true if the ability is now deactivated, false otherwise.
        /// </summary>
        public bool DeactivateAbility(eAbilityType abilityType)
        {
            var ability = this.abilityData.GetAbility(abilityType);

            if (!this.activatedAbilities.Contains(abilityType))
            {
                return true;
            }
            else if (!this.flaggedAbilities.Contains(abilityType))
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

        #region ability flagging methods

        /// <summary>
        /// This method checks if an ability is currently in use by the player.
        /// </summary>
        /// <returns>Returns true if the ability is flagged, false otherwise.</returns>
        public bool CheckAbilityFlagged(eAbilityType abilityType)
        {
            if (this.flaggedAbilities.Contains(abilityType))
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
        /// <returns>Returns true if the ability is now flagged, false otherwise.</returns>
        public bool FlagAbility(eAbilityType abilityType)
        {
            if (this.flaggedAbilities.Contains(abilityType))
            {
                return true;
            }
            else if (this.activatedAbilities.Contains(abilityType))
            {
                this.flaggedAbilities.Add(abilityType);
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
        /// <returns>Returns true if the ability is now unflagged, false otherwise.</returns>
        public bool UnflagAbility(eAbilityType abilityType)
        {
            if (this.flaggedAbilities.Contains(abilityType))
            {
                this.flaggedAbilities.Remove(abilityType);
                return true;
            }
            else
            {
                return true;
            }
        }

        #endregion ability flagging methods

        //###########################################################
        //###########################################################

        #region pillar state methods

        public void SetPillarDestroyed(World.ePillarId pillarId)
        {
            if (!this.destoyedPillars.Contains(pillarId))
            {
                this.destoyedPillars.Add(pillarId);
                UnlockAbilityGroup(pillarData.GetPillarAbilityGroup(pillarId));

                Utilities.EventManager.SendPillarDestroyedEvent(this, new Utilities.EventManager.PillarDestroyedEventArgs(pillarId));
            }
        }

        public bool IsPillarDestroyed(World.ePillarId pillarId)
        {
            if (this.destoyedPillars.Contains(pillarId))
            {
                return true;
            }

            return false;
        }

        #endregion pillar state methods

        //###########################################################
    }
} //end of namespace