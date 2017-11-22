using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.Interaction
{
    public interface IWorldObject
    {
        void InitializeWorldObject(World.ChunkSystem.WorldController worldController);
    }
} //end of namespace