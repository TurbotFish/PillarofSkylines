using Game.World;
using UnityEngine;

namespace Game.LevelElements
{
    public class PillarEntrance : MonoBehaviour
    {
        [SerializeField] private ePillarId pillarId;

        public ePillarId PillarId { get { return pillarId; } }
    }
} //end of namespace