using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.SpawnPointSystem
{
    public class SpawnPointManager : MonoBehaviour
    {
        //##################################################################

        SpawnPoint initialSpawnPoint;
        Dictionary<ePillarId, SpawnPoint> pillarExitPointDictionary = new Dictionary<ePillarId, SpawnPoint>();

        bool isInitialized = false;

        //##################################################################

        void Initialize()
        {
            var children = GetComponentsInChildren<SpawnPoint>();

            foreach (var child in children)
            {
                if (child.Type == eSpawnPointType.Initial)
                {
                    if (initialSpawnPoint == null)
                    {
                        initialSpawnPoint = child;
                    }
                }
                else if (child.Type == eSpawnPointType.PillarExit)
                {
                    if (!pillarExitPointDictionary.ContainsKey(child.Pillar))
                    {
                        pillarExitPointDictionary.Add(child.Pillar, child);
                    }
                }
            }

            isInitialized = true;
        }

        //##################################################################

        /// <summary>
        /// Returns the initial spawn point of the current level.
        /// In the open world this is where the player starts the game, in a pillar it is where the player enters the pillar.
        /// </summary>
        public Vector3 GetInitialSpawnPoint()
        {
            if (!isInitialized)
            {
                Initialize();
            }

            if (initialSpawnPoint != null)
            {
                return initialSpawnPoint.transform.position;
            }
            else
            {
                Debug.LogError("SpawnPointManager: no initial spawn point found!");
                return Vector3.zero;
            }
        }
        
        /// <summary>
        /// Returns the rotation of the initial spawn point of the current level.
        /// In the open world this is where the player starts the game, in a pillar it is where the player enters the pillar.
        /// </summary>
        public Quaternion GetInitialSpawnOrientation() {
            if (!isInitialized) {
                Initialize();
            }

            if (initialSpawnPoint != null) {
                return initialSpawnPoint.transform.rotation;
            } else {
                Debug.LogError("SpawnPointManager: no initial spawn point found!");
                return Quaternion.identity;
            }
        }

        /// <summary>
        /// Returns the position in the open world where the player spawns after exiting the pillar.
        /// </summary>
        public Vector3 GetPillarExitPoint(ePillarId pillarId)
        {
            if (!isInitialized)
            {
                Initialize();
            }

            if (pillarExitPointDictionary.ContainsKey(pillarId))
            {
                return pillarExitPointDictionary[pillarId].transform.position;
            }
            else
            {
                Debug.LogErrorFormat("SpawnPointManager: no spawn point found for pillar {0}!", pillarId.ToString());
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Returns the rotation the player should have when leaving the Pillar.
        /// </summary>
        public Quaternion GetPillarExitOrientation(ePillarId pillarId) {
            if (!isInitialized) {
                Initialize();
            }

            if (pillarExitPointDictionary.ContainsKey(pillarId)) {
                return pillarExitPointDictionary[pillarId].transform.rotation;
            } else {
                Debug.LogErrorFormat("SpawnPointManager: no spawn point found for pillar {0}!", pillarId.ToString());
                return Quaternion.identity;
            }
        }

        //##################################################################
    }
} //end of namespace