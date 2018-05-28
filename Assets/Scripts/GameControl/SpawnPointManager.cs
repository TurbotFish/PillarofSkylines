using Game.Model;
using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameControl
{
    public class SpawnPointManager : MonoBehaviour
    {
        //##################################################################

        private SpawnPoint initialSpawnPoint;
        private Dictionary<PillarId, SpawnPoint> pillarIntactExitDictionary = new Dictionary<PillarId, SpawnPoint>();
        private Dictionary<PillarId, SpawnPoint> pillarDestroyedExitDictionary = new Dictionary<PillarId, SpawnPoint>();

        bool isInitialized = false;

        //##################################################################

        void Initialize()
        {
            var children = GetComponentsInChildren<SpawnPoint>();

            foreach (var child in children)
            {
                if (child.Type == SpawnPointType.Initial)
                {
                    if (initialSpawnPoint == null)
                    {
                        initialSpawnPoint = child;
                    }
                }
                else if (child.Type == SpawnPointType.PillarExitIntact)
                {
                    if (!pillarIntactExitDictionary.ContainsKey(child.Pillar))
                    {
                        pillarIntactExitDictionary.Add(child.Pillar, child);
                    }
                }
                else if (child.Type == SpawnPointType.PillarExitDestroyed)
                {
                    if (!pillarDestroyedExitDictionary.ContainsKey(child.Pillar))
                    {
                        pillarDestroyedExitDictionary.Add(child.Pillar, child);
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
        /// Returns the position in the open world where the player spawns after exiting the pillar.
        /// </summary>
        public Vector3 GetPillarExitPoint(PillarId pillarId, PillarVariant pillar_variant)
        {
            if (!isInitialized)
            {
                Initialize();
            }

            if (pillar_variant == PillarVariant.Intact && pillarIntactExitDictionary.ContainsKey(pillarId))
            {
                return pillarIntactExitDictionary[pillarId].transform.position;
            }
            else if (pillar_variant == PillarVariant.Destroyed && pillarDestroyedExitDictionary.ContainsKey(pillarId))
            {
                return pillarDestroyedExitDictionary[pillarId].transform.position;
            }
            else
            {
                Debug.LogErrorFormat("SpawnPointManager: no {0} exit point found for pillar {1}!", pillar_variant.ToString(), pillarId.ToString());
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Returns the rotation the player should have when leaving the Pillar.
        /// </summary>
        public Quaternion GetPillarExitOrientation(PillarId pillarId, PillarVariant pillar_variant)
        {
            if (!isInitialized)
            {
                Initialize();
            }

            if (pillar_variant == PillarVariant.Intact && pillarIntactExitDictionary.ContainsKey(pillarId))
            {
                return pillarIntactExitDictionary[pillarId].transform.rotation;
            }
            else if(pillar_variant== PillarVariant.Destroyed && pillarDestroyedExitDictionary.ContainsKey(pillarId))
            {
                return pillarDestroyedExitDictionary[pillarId].transform.rotation;
            }
            else
            {
                Debug.LogErrorFormat("SpawnPointManager: no {0} exit point found for pillar {1}!", pillar_variant.ToString(), pillarId.ToString());
                return Quaternion.identity;
            }
        }

        //##################################################################
    }
} //end of namespace