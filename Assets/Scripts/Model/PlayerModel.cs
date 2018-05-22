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

        // -- ATTRIBUTES

        public AbilityData AbilityData { get; private set; }
        public LevelData LevelData { get; private set; }

        public bool HasNeedle { get; set; }
        public int PillarKeys { get; private set; }

        private List<AbilityType> ActivatedAbilityList = new List<AbilityType>();

        private List<PillarId> DestoyedPillarList = new List<PillarId>();
        private List<PillarId> UnlockedPillarList = new List<PillarId>();

        private Dictionary<string, PersistentData> PersistentDataDictionary = new Dictionary<string, PersistentData>();

        //###########################################################

        // -- INITIALIZATION

        public void Initialize()
        {
            AbilityData = Resources.Load<AbilityData>("ScriptableObjects/AbilityData");
            LevelData = Resources.Load<LevelData>("ScriptableObjects/LevelData");

            if (Application.isEditor)
            {
                ActivateAbility(AbilityType.Echo);
            }
        }

        //###########################################################

        // -- INQUIRIES

        /// <summary>
        /// Returns the state of the ability.
        /// </summary>
        /// <param name="abilityType"></param>
        /// <returns></returns>
        public AbilityState GetAbilityState(AbilityType abilityType)
        {
            if (ActivatedAbilityList.Contains(abilityType))
            {
                return AbilityState.active;
            }
            else
            {
                return AbilityState.locked;
            }
        }

        /// <summary>
        /// This method checks if an ability is currently activated.
        /// </summary>
        /// <returns>Returns true if the ability is activated, false otherwise.</returns>
        public bool CheckAbilityActive(AbilityType abilityType)
        {
            return ActivatedAbilityList.Contains(abilityType);
        }

        /// <summary>
        /// Returns a list with all active abilities.
        /// </summary>
        public List<AbilityType> GetAllActiveAbilities()
        {
            return new List<AbilityType>(ActivatedAbilityList);
        }

        /// <summary>
        /// Checks if a pillar has been destroyed.
        /// </summary>
        /// <param name="pillarId">The Id of the pillar to check.</param>
        /// <returns></returns>
        public bool CheckIsPillarDestroyed(PillarId pillarId)
        {
            return DestoyedPillarList.Contains(pillarId);
        }

        /// <summary>
        /// Returns the amount of pillarkeys that need to be sacrificed in order to unlock a pillar.
        /// </summary>
        /// <param name="pillarId">The Id of the pillar to be unlocked.</param>
        /// <returns></returns>
        public int GetPillarEntryPrice(PillarId pillarId)
        {
            if (UnlockedPillarList.Contains(pillarId))
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
        public bool CheckIsPillarUnlocked(PillarId pillarId)
        {
            return UnlockedPillarList.Contains(pillarId);
        }

        /// <summary>
        /// Gets the persistent data object with the id if it exists, null otherwise.
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        public PersistentData GetPersistentDataObject(string uniqueId)
        {
            if (PersistentDataDictionary.ContainsKey(uniqueId))
            {
                return PersistentDataDictionary[uniqueId];
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
            if (PersistentDataDictionary.ContainsKey(uniqueId))
            {
                var dataObject = GetPersistentDataObject(uniqueId);

                return dataObject as T;
            }
            else
            {
                return null;
            }
        }

        //###########################################################

        // -- OPERATIONS

        /// <summary>
        /// This method activates an ability if its Group is unlocked.
        /// Returns true if the ability is now activated, false otherwise.
        /// </summary>
        public bool ActivateAbility(AbilityType abilityType)
        {
            var ability = AbilityData.GetAbility(abilityType);

            if (!ActivatedAbilityList.Contains(abilityType))
            {
                ActivatedAbilityList.Add(abilityType);
                
            }

            return true;
        }

        /// <summary>
        /// This method deactivates an ability and gives the player back the amount of favours it cost to activate it.
        /// Returns true if the ability is now deactivated, false otherwise.
        /// </summary>
        public bool DeactivateAbility(AbilityType abilityType)
        {
            ActivatedAbilityList.Remove(abilityType);

            return true;
        }

        /// <summary>
        /// Destroys a pillar.
        /// </summary>
        /// <param name="pillarId">The Id of the pillar to destroy.</param>
        public void DestroyPillar(PillarId pillarId)
        {
            if (!DestoyedPillarList.Contains(pillarId))
            {
                DestoyedPillarList.Add(pillarId);

                Utilities.EventManager.SendPillarDestroyedEvent(this, new Utilities.EventManager.PillarDestroyedEventArgs(pillarId));
            }
        }

        /// <summary>
        /// Unlocks a pillar. The entry price will be removed from the player's favours.
        /// </summary>
        /// <param name="pillarId">The Id of the pillar to unlock</param>
        /// <returns>true if the Pillar is unlocked, false otherwise</returns>
        public bool UnlockPillar(PillarId pillarId)
        {
            if (UnlockedPillarList.Contains(pillarId))
            {
                return true;
            }
            else
            {
                if (PillarKeys >= GetPillarEntryPrice(pillarId))
                {
                    UnlockedPillarList.Add(pillarId);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Registers a new persistent data object. Returns true if the registration was successfull, false otherwise.
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public bool AddPersistentDataObject(PersistentData dataObject)
        {
            if (PersistentDataDictionary.ContainsKey(dataObject.UniqueId))
            {
                if (PersistentDataDictionary[dataObject.UniqueId] == dataObject)
                {
                    return true;
                }
                else
                {
                    Debug.LogErrorFormat("Model: AddPersistentDataObject: An object with the id \"{0}\" already exists! currentObjectType={1}; newObjectType={2}",
                        dataObject.UniqueId,
                        PersistentDataDictionary[dataObject.UniqueId].GetType(),
                        dataObject.GetType()
                    );

                    return false;
                }
            }
            else
            {
                PersistentDataDictionary.Add(dataObject.UniqueId, dataObject);
                return true;
            }
        }

        /// <summary>
        /// Changes the amount of pillar keys the player has.
        /// </summary>
        /// <param name="pillarKeysDelta"></param>
        public void ChangePillarKeysCount(int pillarKeysDelta)
        {
            PillarKeys += pillarKeysDelta;
            if (PillarKeys < 0)
            {
                PillarKeys = 0;
            }
        }

        /// <summary>
        /// Unity's Update methpod.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.F2))
            {
                Debug.Log("CHEATING: One PillarKey appeared out of nowhere!");
                PillarKeys++;
            }
            else if (Input.GetKeyUp(KeyCode.F5))
            {
                Debug.Log("CHEATING: You were supposed to find the Tombs, not make them useless!");

                foreach (var ability in AbilityData.GetAllAbilities())
                {
                    ActivateAbility(ability.Type);
                }
            }
        }
    }
} //end of namespace