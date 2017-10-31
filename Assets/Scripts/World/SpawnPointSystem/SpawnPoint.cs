using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.SpawnPointSystem
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        eSpawnPointType type;
        public eSpawnPointType Type
        {
            get { return this.type; }
#if UNITY_EDITOR
            set { this.type = value; }
#endif
        }

        [SerializeField]
        [HideInInspector]
        ePillarId pillar;
        public ePillarId Pillar
        {
            get { return this.pillar; }
#if UNITY_EDITOR
            set { this.pillar = value; }
#endif
        }
    }
} //end of namespace