using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.Interaction
{
    public class PillarEntrance : MonoBehaviour
    {
        [SerializeField]
        ePillarId pillarId;
        public ePillarId PillarId { get { return pillarId; } }
    }
}