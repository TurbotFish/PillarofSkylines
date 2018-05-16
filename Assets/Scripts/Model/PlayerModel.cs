using Game.LevelElements;
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

        public LevelData LevelData { get; private set; }

        //
        public bool hasNeedle;
        private int pillarKeys;

        //currencies
        private Dictionary<eCurrencyType, int> currencies;

        //ability variables
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
            LevelData = Resources.Load<LevelData>("ScriptableObjects/LevelData");

            //initializing currency dictionary
            currencies = new Dictionary<eCurrencyType, int>();
            foreach (var value in Enum.GetValues(typeof(eCurrencyType)).Cast<eCurrencyType>())
            {
                currencies.Add(value, 0);
            }
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

                foreach(var ability in abilityData.GetAllAbilities())
                {
                    ActivateAbility(ability.Type);
                }
            }
        }

        #endregion monobehaviour methods

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
            else
            {
                activatedAbilities.Add(abilityType);

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
                return LevelData.GetPillarSceneActivationCost(pillarId);
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
            else
            {
                return eAbilityState.locked;
            }
        }



        #endregion stuff

        //###########################################################
    }
} //end of namespace