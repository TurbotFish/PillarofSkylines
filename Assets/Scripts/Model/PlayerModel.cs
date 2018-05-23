using Game.LevelElements;
using Game.Utilities;
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
    public class PlayerModel
    {
        //###########################################################

        // -- ATTRIBUTES

        public AbilityData AbilityData { get; private set; }
        public LevelData LevelData { get; private set; }

        public bool PlayerHasNeedle { get; set; }

        private List<PillarId> DestoyedPillarList = new List<PillarId>();
        private List<PillarId> UnlockedPillarList = new List<PillarId>();

        private Dictionary<AbilityType, AbilityState> AbilityStateDictionary;
        private Dictionary<PillarMarkId, PillarMarkState> PillarMarkStateDictionary;
        private Dictionary<PillarId, PillarState> PillarStateDictionary;

        private Dictionary<string, PersistentData> PersistentDataDictionary = new Dictionary<string, PersistentData>();

        //###########################################################

        // -- INITIALIZATION

        public PlayerModel()
        {
            AbilityData = Resources.Load<AbilityData>("ScriptableObjects/AbilityData");
            LevelData = Resources.Load<LevelData>("ScriptableObjects/LevelData");

            AbilityStateDictionary = new Dictionary<AbilityType, AbilityState>();
            foreach(var ability in Enum.GetValues(typeof(AbilityType)).Cast<AbilityType>())
            {
                AbilityStateDictionary.Add(ability, AbilityState.inactive);
            }

            PillarMarkStateDictionary = new Dictionary<PillarMarkId, PillarMarkState>();
            foreach(var pillar_mark in Enum.GetValues(typeof(PillarMarkId)).Cast<PillarMarkId>())
            {
                PillarMarkStateDictionary.Add(pillar_mark, PillarMarkState.inactive);
            }

            PillarStateDictionary = new Dictionary<PillarId, PillarState>();
            foreach(var pillar_id in Enum.GetValues(typeof(PillarId)).Cast<PillarId>())
            {
                PillarStateDictionary.Add(pillar_id, PillarState.Locked);
            }

            if (Application.isEditor)
            {
                SetAbilityState(AbilityType.Echo, AbilityState.active);
            }
        }

        //###########################################################

        // -- INQUIRIES

        /// <summary>
        /// Returns the state of the ability.
        /// </summary>
        /// <param name="ability_type"></param>
        /// <returns></returns>
        public AbilityState GetAbilityState(AbilityType ability_type)
        {
            return AbilityStateDictionary[ability_type];
        }

        /// <summary>
        /// This method checks if an ability is currently activated.
        /// Returns true if the ability is activated, false otherwise.
        /// </summary>
        /// <param name="ability_type"></param>
        /// <returns></returns>
        public bool CheckAbilityActive(AbilityType ability_type)
        {
            return AbilityStateDictionary[ability_type] == AbilityState.active;
        }

        /// <summary>
        /// Returns the state of the pillar mark.
        /// </summary>
        /// <param name="pillar_mark_id"></param>
        /// <returns></returns>
        public PillarMarkState GetPillarMarkState(PillarMarkId pillar_mark_id)
        {
            return PillarMarkStateDictionary[pillar_mark_id];
        }

        /// <summary>
        /// Returns the amount of active pillar marks.
        /// </summary>
        /// <returns></returns>
        public int GetActivePillarMarkCount()
        {
            int result = 0;

            foreach (var pillar_mark in Enum.GetValues(typeof(PillarMarkId)).Cast<PillarMarkId>())
            {
                if(PillarMarkStateDictionary[pillar_mark] == PillarMarkState.active)
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the current state of the pillar.
        /// </summary>
        /// <param name="pillar_id"></param>
        /// <returns></returns>
        public PillarState GetPillarState(PillarId pillar_id)
        {
            return PillarStateDictionary[pillar_id];
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
        /// Sets the state of an ability.
        /// </summary>
        public void SetAbilityState(AbilityType ability_type, AbilityState ability_state)
        {
            AbilityStateDictionary[ability_type] = ability_state;

            EventManager.SendAbilityStateChangedEvent(this, new EventManager.AbilityStateChangedEventArgs(ability_type, ability_state));
        }

        /// <summary>
        /// Sets the state of a pillar mark.
        /// </summary>
        /// <param name="pillar_mark_id"></param>
        /// <param name="pillar_mark_state"></param>
        public void SetPillarMarkState(PillarMarkId pillar_mark_id, PillarMarkState pillar_mark_state)
        {
            PillarMarkStateDictionary[pillar_mark_id] = pillar_mark_state;

            EventManager.SendPillarMarkStateChangedEvent(this, new EventManager.PillarMarkStateChangedEventArgs(pillar_mark_id, pillar_mark_state));
        }

        public void SetPillarState(PillarId pillar_id, PillarState pillar_state)
        {
            PillarStateDictionary[pillar_id] = pillar_state;


        }

        /// <summary>
        /// Destroys a pillar.
        /// </summary>
        /// <param name="pillarId">The Id of the pillar to destroy.</param>
        public void SetPillarDestroyed(PillarId pillarId)
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
                if (GetActivePillarMarkCount() >= GetPillarEntryPrice(pillarId))
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
    }
} //end of namespace