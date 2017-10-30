using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.SpawnPointSystem
{
    public class SpawnPointManager : MonoBehaviour
    {
        SpawnPoint initialSpawnPoint;

        Dictionary<ePillarId, SpawnPoint> pillarExitPointDictionary = new Dictionary<ePillarId, SpawnPoint>();

        void Start()
        {
            var children = GetComponentsInChildren<SpawnPoint>();

            foreach(var child in children)
            {
                if(child.Type == eSpawnPointType.Initial)
                {
                    if(this.initialSpawnPoint == null)
                    {
                        this.initialSpawnPoint = child;
                    }
                }
                else if(child.Type == eSpawnPointType.PillarExit)
                {
                    if (!this.pillarExitPointDictionary.ContainsKey(child.Pillar))
                    {
                        this.pillarExitPointDictionary.Add(child.Pillar, child);
                    }
                }
            }
        }

        public Vector3 GetInitialSpawnPoint()
        {
            return this.initialSpawnPoint.transform.position;
        }

        public Vector3 GetPillarExitPoint(ePillarId pillar)
        {
            return this.pillarExitPointDictionary[pillar].transform.position;
        }
    }
} //end of namespace