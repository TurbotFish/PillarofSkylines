﻿using Game.LevelElements;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Model
{
    /// <summary>
    /// This class stores data about the player: favours, abilities, stats, ...
    /// </summary>
    public class PlayerModel : MonoBehaviour
    {
        //###########################################################

        //ability data
        [SerializeField]
        AbilityData abilityData;
        public AbilityData AbilityData { get { return abilityData; } }

        //pillar data
        PillarData pillarData;
        public PillarData PillarData { get { return pillarData; } }

        //
        public bool hasNeedle;
        private int pillarKeys;

        //currencies
        private Dictionary<eCurrencyType, int> currencies;

        //ability variables
        List<eAbilityGroup> unlockedAbilityGroups = new List<eAbilityGroup>();
        List<eAbilityType> activatedAbilities = new List<eAbilityType>();
        List<eAbilityType> flaggedAbilities = new List<eAbilityType>();

        //pillars
        List<ePillarId> destoyedPillars = new List<ePillarId>();
        List<ePillarId> unlockedPillars = new List<ePillarId>();

        //persistent data
        Dictionary<string, PersistentData> persistentDataDict = new Dictionary<string, PersistentData>();

        //###########################################################

        public void InitializePlayerModel()
        {
            pillarData = Resources.Load<PillarData>("ScriptableObjects/PillarData");

            //initializing currency dictionary
            currencies = new Dictionary<eCurrencyType, int>();
            foreach (var value in Enum.GetValues(typeof(eCurrencyType)).Cast<eCurrencyType>())
            {
                currencies.Add(value, 0);
            }

            UnlockAbilityGroup(eAbilityGroup.Default);
        }

        //###########################################################

        #region monobehaviour methods

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.F2))
            {
                Debug.Log("CHEATING: One PillarKey appeared out of nowhere!");
                pillarKeys++;
            }
            else if (Input.GetKeyUp(KeyCode.F5))
            {
                Debug.Log("CHEATING: You were supposed to find the Tombs, not make them useless!");
                UnlockAbilityGroup(eAbilityGroup.Pillar1);
                UnlockAbilityGroup(eAbilityGroup.Pillar2);
                UnlockAbilityGroup(eAbilityGroup.Pillar3);

                foreach(var ability in abilityData.GetAllAbilities())
                {
                    ActivateAbility(ability.Type);
                }
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
        /// This method activates an ability if its Group is unlocked.
        /// Returns true if the ability is now activated, false otherwise.
        /// </summary>
        public bool ActivateAbility(eAbilityType abilityType)
        {
            var ability = abilityData.GetAbility(abilityType);

            if (activatedAbilities.Contains(abilityType))
            {
                return true;
            }
            else if (CheckAbilityGroupUnlocked(ability.Group))
            {
                activatedAbilities.Add(abilityType);

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
            if (!activatedAbilities.Contains(abilityType))
            {
                return true;
            }
            else if (!flaggedAbilities.Contains(abilityType))
            {
                activatedAbilities.Remove(abilityType);

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
            if (flaggedAbilities.Contains(abilityType))
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
            if (flaggedAbilities.Contains(abilityType))
            {
                return true;
            }
            else if (activatedAbilities.Contains(abilityType))
            {
                flaggedAbilities.Add(abilityType);
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
            if (flaggedAbilities.Contains(abilityType))
            {
                flaggedAbilities.Remove(abilityType);
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
        /// <param name="pillarId">The Id of the pillar to unlock</param>
        /// <returns>true if the Pillar is unlocked, false otherwise</returns>
        public bool UnlockPillar(ePillarId pillarId)
        {
            if (unlockedPillars.Contains(pillarId))
            {
                return true;
            }
            else
            {
                if (pillarKeys >= GetPillarEntryPrice(pillarId))
                {
                    unlockedPillars.Add(pillarId);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the amount of pillarkeys that need to be sacrificed in order to unlock a pillar.
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
        /// Checks if a pillar has been unlocked. This means that the player can enter the pillar.
        /// </summary>
        /// <param name="pillarId">The Id of the pillar to check.</param>
        /// <returns></returns>
        public bool CheckIsPillarUnlocked(ePillarId pillarId)
        {
            return unlockedPillars.Contains(pillarId);
        }

        #endregion pillar methods

        //###########################################################

        //OBSOLETE
        #region pick-up methods

        ///// <summary>
        ///// Informs the model that a Pick-up has been collected.
        ///// </summary>
        ///// <param name="pickUp">The collected Pick-up.</param>
        //[Obsolete]
        //public void CollectPickUp(CurrencyPickUp pickUp)
        //{
        //    if (collectedPickUps.Contains(pickUp.PickUpId))
        //    {
        //        Debug.LogWarningFormat("Pick-up \"{0}\" already collected!", pickUp.PickUpId);
        //    }
        //    else
        //    {
        //        //pick up favour
        //        if (pickUp.CurrencyType == eCurrencyType.Favour)
        //        {
        //            ChangeCurrencyAmount(eCurrencyType.Favour, 1);
        //        }
        //        else if (pickUp.CurrencyType == eCurrencyType.PillarKey)
        //        {
        //            ChangeCurrencyAmount(eCurrencyType.PillarKey, 1);
        //        }

        //        collectedPickUps.Add(pickUp.PickUpId);

        //        //send event
        //        Utilities.EventManager.SendFavourPickedUpEvent(this, new Utilities.EventManager.FavourPickedUpEventArgs(pickUp.PickUpId));
        //    }
        //}

        ///// <summary>
        ///// Checks whether a Pick-up has already been collected.
        ///// </summary>
        ///// <param name="pickUpId">The Id of the favour to check.</param>
        ///// <returns></returns>
        //[Obsolete]
        //public bool CheckIfPickUpCollected(string pickUpId)
        //{
        //    return collectedPickUps.Contains(pickUpId);
        //}

        #endregion pick-up methods

        //###########################################################

        //OBSOLETE
        #region currency methods

        [Obsolete]
        public int GetCurrencyAmount(eCurrencyType currencyType)
        {
            return currencies[currencyType];
        }

        ///// <summary>
        ///// Changes the amount of a currency. Checks whether abilities have become available and sends appropriate events.
        ///// </summary>
        ///// <param name="currencyDelta"></param>
        //[Obsolete]
        //private void ChangeCurrencyAmount(eCurrencyType currencyType, int currencyDelta)
        //{
        //    currencies[currencyType] += currencyDelta;

        //    Utilities.EventManager.SendCurrencyAmountChangedEvent(this, new Utilities.EventManager.CurrencyAmountChangedEventArgs(currencyType, currencies[currencyType]));

        //    //send events
        //    var abilities = abilityData.GetAllAbilities();
        //    for (int i = 0; i < abilities.Count; i++)
        //    {
        //        var ability = abilities[i];

        //        if (!CheckAbilityActive(ability.Type) && CheckAbilityGroupUnlocked(ability.Group))
        //        {
        //            var newState = eAbilityState.locked;

        //            if (GetCurrencyAmount(eCurrencyType.Favour) >= ability.ActivationPrice)
        //            {
        //                newState = eAbilityState.available;
        //            }

        //            Utilities.EventManager.SendAbilityStateChangedEvent(this, new Utilities.EventManager.AbilityStateChangedEventArgs(ability.Type, newState));
        //        }
        //    }
        //}

        #endregion currency methods

        //###########################################################

        #region persistent data methods

        /// <summary>
        /// Registers a new persistent data object. Returns true if the registration was successfull, false otherwise.
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public bool AddPersistentDataObject(PersistentData dataObject)
        {
            if (persistentDataDict.ContainsKey(dataObject.UniqueId))
            {
                if (persistentDataDict[dataObject.UniqueId] == dataObject)
                {
                    return true;
                }
                else
                {
                    Debug.LogErrorFormat("Model: AddPersistentDataObject: An object with the id \"{0}\" already exists! currentObjectType={1}; newObjectType={2}",
                        dataObject.UniqueId,
                        persistentDataDict[dataObject.UniqueId].GetType(),
                        dataObject.GetType()
                    );

                    return false;
                }
            }
            else
            {
                persistentDataDict.Add(dataObject.UniqueId, dataObject);
                return true;
            }
        }

        /// <summary>
        /// Gets the persistent data object with the id if it exists, null otherwise.
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        public PersistentData GetPersistentDataObject(string uniqueId)
        {
            if (persistentDataDict.ContainsKey(uniqueId))
            {
                return persistentDataDict[uniqueId];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the persistent data object with the id if it exists and can be cast to T, null otherwise.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        public T GetPersistentDataObject<T>(string uniqueId) where T : PersistentData
        {
            if (persistentDataDict.ContainsKey(uniqueId))
            {
                var dataObject = GetPersistentDataObject(uniqueId);

                return dataObject as T;
            }
            else
            {
                return null;
            }
        }

        #endregion persistent data methods

        //###########################################################

        #region pillar key methods

        public int PillarKeysCount { get { return pillarKeys; } }

        public void ChangePillarKeysCount(int pillarKeysDelta)
        {
            pillarKeys += pillarKeysDelta;
            if (pillarKeys < 0)
            {
                pillarKeys = 0;
            }
        }

        #endregion pillar key methods

        //###########################################################

        #region stuff

        public eAbilityState GetAbilityState(eAbilityType abilityType)
        {
            var ability = abilityData.GetAbility(abilityType);

            if (activatedAbilities.Contains(abilityType))
            {
                return eAbilityState.active;
            }
            else if (unlockedAbilityGroups.Contains(ability.Group) /*&& GetCurrencyAmount(eCurrencyType.Favour) >= ability.ActivationPrice*/)
            {
                return eAbilityState.available;
            }
            else if (!CheckAbilityUnlocked(abilityType))
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