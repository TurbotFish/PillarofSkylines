using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.SpawnPointSystem
{
    public class SpawnPointManager : MonoBehaviour
    {
        //##################################################################

        private SpawnPoint initialSpawnPoint;
        private SpawnPoint home;
        private Dictionary<ePillarId, SpawnPoint> pillarIntactExitDictionary = new Dictionary<ePillarId, SpawnPoint>();
        private Dictionary<ePillarId, SpawnPoint> pillarDestroyedExitDictionary = new Dictionary<ePillarId, SpawnPoint>();

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
                else if (child.Type == eSpawnPointType.PillarExitIntact)
                {
                    if (!pillarIntactExitDictionary.ContainsKey(child.Pillar))
                    {
                        pillarIntactExitDictionary.Add(child.Pillar, child);
                    }
                }
                else if (child.Type == eSpawnPointType.PillarExitDestroyed)
                {
                    if (!pillarDestroyedExitDictionary.ContainsKey(child.Pillar))
                    {
                        pillarDestroyedExitDictionary.Add(child.Pillar, child);
                    }
                }
                else if (child.Type == eSpawnPointType.Home)
                {
                    if (home == null)
                    {
                        home = child;
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
        public Quaternion GetInitialSpawnOrientation()
        {
            if (!isInitialized)
            {
                Initialize();
            }

            if (initialSpawnPoint != null)
            {
                return initialSpawnPoint.transform.rotation;
            }
            else
            {
                Debug.LogError("SpawnPointManager: no initial spawn point found!");
                return Quaternion.identity;
            }
        }


        /// <summary>
        /// Returns the home spawn point.
        /// Only works in the Open World.
        /// </summary>
        public Transform GetHomeSpawnTransform()
        {
            if (!isInitialized)
            {
                Initialize();
            }

            if (home != null)
            {
                return home.transform;
            }
            else
            {
                Debug.LogError("SpawnPointManager: no home spawn point found!");
                return null;
            }
        }

        /// <summary>
        /// Returns the home spawn point.
        /// Only works in the Open World.
        /// </summary>
        public Vector3 GetHomeSpawnPoint()
        {
            if (!isInitialized)
            {
                Initialize();
            }

            if (home != null)
            {
                return home.transform.position;
            }
            else
            {
                Debug.LogError("SpawnPointManager: no home spawn point found!");
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Returns the home spawn rotation.
        /// Only works in the Open World.
        /// </summary>
        public Quaternion GetHomeSpawnOrientation()
        {
            if (!isInitialized)
            {
                Initialize();
            }

            if (home != null)
            {
                return home.transform.rotation;
            }
            else
            {
                Debug.LogError("SpawnPointManager: no home spawn point found!");
                return Quaternion.identity;
            }
        }

        /// <summary>
        /// Returns the position in the open world where the player spawns after exiting the pillar.
        /// </summary>
        public Vector3 GetPillarExitPoint(ePillarId pillarId, ePillarState pillarState)
        {
            if (!isInitialized)
            {
                Initialize();
            }

            if (pillarState == ePillarState.Intact && pillarIntactExitDictionary.ContainsKey(pillarId))
            {
                return pillarIntactExitDictionary[pillarId].transform.position;
            }
            else if (pillarState == ePillarState.Destroyed && pillarDestroyedExitDictionary.ContainsKey(pillarId))
            {
                return pillarDestroyedExitDictionary[pillarId].transform.position;
            }
            else
            {
                Debug.LogErrorFormat("SpawnPointManager: no {0} exit point found for pillar {1}!", pillarState.ToString(), pillarId.ToString());
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Returns the rotation the player should have when leaving the Pillar.
        /// </summary>
        public Quaternion GetPillarExitOrientation(ePillarId pillarId, ePillarState pillarState)
        {
            if (!isInitialized)
            {
                Initialize();
            }

            if (pillarState == ePillarState.Intact && pillarIntactExitDictionary.ContainsKey(pillarId))
            {
                return pillarIntactExitDictionary[pillarId].transform.rotation;
            }
            else if(pillarState== ePillarState.Destroyed && pillarDestroyedExitDictionary.ContainsKey(pillarId))
            {
                return pillarDestroyedExitDictionary[pillarId].transform.rotation;
            }
            else
            {
                Debug.LogErrorFormat("SpawnPointManager: no {0} exit point found for pillar {1}!", pillarState.ToString(), pillarId.ToString());
                return Quaternion.identity;
            }
        }

        //##################################################################
    }
} //end of namespace