using Game.LevelElements;
using Game.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// This class stores data about the player: favours, abilities, stats, ...
    /// </summary>
    public class PlayerModel : MonoBehaviour
    {
        //ability data
        [SerializeField]
        AbilitySystem.AbilityData abilityData;
        public AbilitySystem.AbilityData AbilityData { get { return abilityData; } }

        //pillar data
        PillarData pillarData;
        public PillarData PillarData { get { return pillarData; } }

        //
        public bool hasNeedle;

        //
        public int Favours { get; private set; }

        //ability variables
        List<eAbilityGroup> unlockedAbilityGroups = new List<eAbilityGroup>();
        List<eAbilityType> activatedAbilities = new List<eAbilityType>();
        List<eAbilityType> flaggedAbilities = new List<eAbilityType>();

        //
        List<ePillarId> destoyedPillars = new List<ePillarId>();
        List<ePillarId> unlockedPillars = new List<ePillarId>();

        List<string> pickedUpFavours = new List<string>();

        //level element data
        List<PersistentTrigger> persistentTriggers = new List<PersistentTrigger>();
        List<PersistentTriggerable> persistentTriggerables = new List<PersistentTriggerable>();

        //###########################################################

        public void InitializePlayerModel()
        {
            pillarData = Resources.Load<World.PillarData>("ScriptableObjects/PillarData");

            UnlockAbilityGroup(eAbilityGroup.Default);
        }

        //###########################################################

        #region monobehaviour methods

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.F2))
            {
                ChangeFavourAmount(1);
            }
            else if (Input.GetKeyUp(KeyCode.F5))
            {
                UnlockAbilityGroup(eAbilityGroup.Pillar1);
                UnlockAbilityGroup(eAbilityGroup.Pillar2);
                UnlockAbilityGroup(eAbilityGroup.Pillar3);
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        #region ability group unlocking methods

        /// <summary>
        /// This method checks if an ability group is unlocked
        /// </summary>
        /// <returns>Returns true if the ability group is unlocked, false otherwise.</returns>
        public bool CheckAbilityGroupUnlocked(eAbilityGroup abilityGroup)
        {
            return unlockedAbilityGroups.Contains(abilityGroup);
        }

        /// <summary>
        /// This method unlocks an ability group.
        /// </summary>
        /// <returns>Returns true if the ability group is now unlocked, false otherwise.</returns>
        public bool UnlockAbilityGroup(eAbilityGroup abilityGroup)
        {
            if (!unlockedAbilityGroups.Contains(abilityGroup))
            {
                unlockedAbilityGroups.Add(abilityGroup);

                //send events
                var abilities = abilityData.GetAllAbilities();
                for (int i = 0; i < abilities.Count; i++)
                {
                    var ability = abilities[i];

                    if (ability.Group == abilityGroup)
                    {
                        Utilities.EventManager.SendAbilityStateChangedEvent(this, new Utilities.EventManager.AbilityStateChangedEventArgs(ability.Type, eAbilityState.available));
                    }
                }

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
            if (unlockedAbilityGroups.Contains(abilityGroup))
            {
                unlockedAbilityGroups.Remove(abilityGroup);

                var abilitiesToDeactivate = new List<eAbilityType>();
                foreach (var abilityType in activatedAbilities)
                {
                    var ability = abilityData.GetAbility(abilityType);

                    if (ability.Group == abilityGroup)
                    {
                        abilitiesToDeactivate.Add(abilityType);
                    }
                }

                foreach (var abilityType in abilitiesToDeactivate)
                {
                    activatedAbilities.Remove(abilityType);
                    flaggedAbilities.Remove(abilityType);
                }

                //send events
                var abilities = abilityData.GetAllAbilities();
                for (int i = 0; i < abilities.Count; i++)
                {
                    var ability = abilities[i];

                    if (ability.Group == abilityGroup)
                    {
                        Utilities.EventManager.SendAbilityStateChangedEvent(this, new Utilities.EventManager.AbilityStateChangedEventArgs(ability.Type, eAbilityState.locked));
                    }
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

        #region ability activation methods

        /// <summary>
        /// This method checks if an ability is currently activated.
        /// </summary>
        /// <returns>Returns true if the ability is activated, false otherwise.</returns>
        public bool CheckAbilityActive(eAbilityType abilityType)
        {
            return activatedAbilities.Contains(abilityType);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CheckAbilityUnlocked(eAbilityType abilityType)
        {
            var ability = abilityData.GetAbility(abilityType);

            if (CheckAbilityGroupUnlocked(ability.Group))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<eAbilityType> GetAllActiveAbilities()
        {
            return new List<eAbilityType>(activatedAbilities);
        }

        /// <summary>
        /// This method activates an ability and removes favours equal to its activation price.
        /// Returns true if the ability is now activated, false otherwise.
        /// </summary>
        public bool ActivateAbility(eAbilityType abilityType)
        {
            var ability = abilityData.GetAbility(abilityType);

            if (activatedAbilities.Contains(abilityType))
            {
                return true;
            }
            else if (CheckAbilityGroupUnlocked(ability.Group) && Favours >= ability.ActivationPrice)
            {
                ChangeFavourAmount(-ability.ActivationPrice);
                activatedAbilities.Add(abilityType);

                Utilities.EventManager.SendAbilityStateChangedEvent(this, new Utilities.EventManager.AbilityStateChangedEventArgs(abilityType, eAbilityState.active));

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
            var ability = abilityData.GetAbility(abilityType);

            if (!activatedAbilities.Contains(abilityType))
            {
                return true;
            }
            else if (!flaggedAbilities.Contains(abilityType))
            {
                ChangeFavourAmount(ability.ActivationPrice);
                activatedAbilities.Remove(abilityType);

                Utilities.EventManager.SendAbilityStateChangedEvent(this, new Utilities.EventManager.AbilityStateChangedEventArgs(abilityType, eAbilityState.available));

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion ability activation methods

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

        #region pillar methods

        /// <summary>
        /// Destroys a pillar.
        /// </summary>
        /// <param name="pillarId">The Id of the pillar to destroy.</param>
        public void DestroyPillar(ePillarId pillarId)
        {
            if (!destoyedPillars.Contains(pillarId))
            {
                destoyedPillars.Add(pillarId);
                UnlockAbilityGroup(pillarData.GetPillarAbilityGroup(pillarId));

                Utilities.EventManager.SendPillarDestroyedEvent(this, new Utilities.EventManager.PillarDestroyedEventArgs(pillarId));
            }
        }

        /// <summary>
        /// Checks if a pillar has been destroyed.
        /// </summary>
        /// <param name="pillarId">The Id of the pillar to check.</param>
        /// <returns></returns>
        public bool CheckIsPillarDestroyed(ePillarId pillarId)
        {
            return destoyedPillars.Contains(pillarId);
        }

        /// <summary>
        /// Unlocks a pillar. The entry price will be removed from the player's favours.
        /// </summary>
        /// <param name="pillarId">The Id of the pillar to unlock.</param>
        public void UnlockPillar(ePillarId pillarId)
        {
            if (unlockedPillars.Contains(pillarId))
            {
                return;
            }
            else
            {
                unlockedPillars.Add(pillarId);
                ChangeFavourAmount(-PillarData.GetPillarEntryPrice(pillarId));
            }
        }

        /// <summary>
        /// Returns the amount of favours that need to be sacrificed in order to unlock a pillar.
        /// </summary>
        /// <param name="pillarId">The Id of the pillar to be unlocked.</param>
        /// <returns></returns>
        public int GetPillarEntryPrice(ePillarId pillarId)
        {
            if (unlockedPillars.Contains(pillarId))
            {
                return 0;
            }
            else
            {
                return pillarData.GetPillarEntryPrice(pillarId);
            }
        }

        /// <summary>
        /// Checks if a pillar has been destroyed.
        /// </summary>
        /// <param name="pillarId">The Id of the pillar to check.</param>
        /// <returns></returns>
        public bool CheckIsPillarUnlocked(ePillarId pillarId)
        {
            return unlockedPillars.Contains(pillarId);
        }

        #endregion pillar methods

        //###########################################################

        #region favour methods

        /// <summary>
        /// Informs the model that a favour has been picked up.
        /// </summary>
        /// <param name="favourId">The Id of the picked-up favour.</param>
        public void PickupFavour(string favourId)
        {
            if (pickedUpFavours.Contains(favourId))
            {
                Debug.LogWarning("Favour already picked up!");
            }
            else
            {
                //pick up favour
                if (favourId == "ECHO")
                    ChangeFavourAmount(0); // THIS IS TEMPORARY
                else
                    ChangeFavourAmount(1);
                pickedUpFavours.Add(favourId);

                //send event
                Utilities.EventManager.SendFavourPickedUpEvent(this, new Utilities.EventManager.FavourPickedUpEventArgs(favourId));
            }
        }

        /// <summary>
        /// Checks whether the favour has already been picked up.
        /// </summary>
        /// <param name="favourId">The Id of the favour to check.</param>
        /// <returns></returns>
        public bool CheckIsFavourPickedUp(string favourId)
        {
            return pickedUpFavours.Contains(favourId);
        }

        /// <summary>
        /// Changes the amount of favours of the player. Checks whether abilities have become available and sends appropriate events.
        /// </summary>
        /// <param name="favourDelta"></param>
        private void ChangeFavourAmount(int favourDelta)
        {
            Favours += favourDelta;

            Utilities.EventManager.SendFavourAmountChangedEvent(this, new Utilities.EventManager.FavourAmountChangedEventArgs(Favours));

            //send events
            var abilities = abilityData.GetAllAbilities();
            for (int i = 0; i < abilities.Count; i++)
            {
                var ability = abilities[i];

                if (!CheckAbilityActive(ability.Type) && CheckAbilityGroupUnlocked(ability.Group))
                {
                    var newState = eAbilityState.locked;

                    if (Favours >= ability.ActivationPrice)
                    {
                        newState = eAbilityState.available;
                    }

                    Utilities.EventManager.SendAbilityStateChangedEvent(this, new Utilities.EventManager.AbilityStateChangedEventArgs(ability.Type, newState));
                }
            }
        }

        #endregion favour methods

        //###########################################################

        #region level element methods

        /// <summary>
        /// Adds a PersistentTrigger to the model if it does not yet contain one with same Id.
        /// </summary>
        /// <param name="trigger"></param>
        public void AddPersistentTrigger(PersistentTrigger trigger)
        {
            if (GetPersistentTrigger(trigger.Id) == null)
            {
                persistentTriggers.Add(trigger);
            }
        }

        /// <summary>
        /// Returns the PersistentTrigger with the given Id if it exists, nul otherwise.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PersistentTrigger GetPersistentTrigger(string id)
        {
            return persistentTriggers.FirstOrDefault(item => item.Id == id);
        }

        /// <summary>
        /// Adds a PersistentTriggerable to the model if it does not yet contain one with same Id.
        /// </summary>
        /// <param name="triggerable"></param>
        public void AddPersistentTriggerable(PersistentTriggerable triggerable)
        {
            if (GetPersistentTriggerable(triggerable.Id) == null)
            {
                persistentTriggerables.Add(triggerable);
            }
        }

        /// <summary>
        /// Returns the PersistentTriggerable with the given Id if it exists, nul otherwise.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PersistentTriggerable GetPersistentTriggerable(string id)
        {
            return persistentTriggerables.FirstOrDefault(item => item.Id == id);
        }

        #endregion level element methods

        //###########################################################

        #region stuff

        public eAbilityState GetAbilityState(eAbilityType abilityType)
        {
            var ability = abilityData.GetAbility(abilityType);

            if (activatedAbilities.Contains(abilityType))
            {
                return eAbilityState.active;
            }
            else if (unlockedAbilityGroups.Contains(ability.Group) && Favours >= ability.ActivationPrice)
            {
                return eAbilityState.available;
            } else if (!CheckAbilityUnlocked(abilityType))
            {
                return eAbilityState.pillarLocked;
            }
            else
            {
                return eAbilityState.locked;
            }
        }



        #endregion stuff

        //###########################################################
    }
} //end of namespace