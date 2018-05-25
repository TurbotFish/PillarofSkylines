using Game.Model;
using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameControl
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        SpawnPointType type;
        public SpawnPointType Type
        {
            get { return type; }
#if UNITY_EDITOR
            set { type = value; }
#endif
        }

        [SerializeField]
        [HideInInspector]
        PillarId pillar;
        public PillarId Pillar
        {
            get { return pillar; }
#if UNITY_EDITOR
            set { pillar = value; }
#endif
        }
    }
} //end of namespace