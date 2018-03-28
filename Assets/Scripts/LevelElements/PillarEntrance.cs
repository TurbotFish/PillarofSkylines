using Game.World;
using UnityEngine;

namespace Game.LevelElements
{
    /// <summary>
    /// Used to tag the interaction collider that allows the player to enter a pillar from the open world.
    /// </summary>
    public class PillarEntrance : MonoBehaviour
    {
        [SerializeField] private ePillarId pillarId;

        public ePillarId PillarId { get { return pillarId; } }
    }
} //end of namespace