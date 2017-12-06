using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.Interaction
{
    public interface IWorldObject
    {
        void InitializeWorldObject(ChunkSystem.WorldController worldController, bool isCopy);
    }
} //end of namespace