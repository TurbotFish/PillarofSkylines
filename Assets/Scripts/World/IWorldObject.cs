using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    public interface IWorldObjectInitialization
    {
        void Initialize(ChunkSystem.WorldController worldController, bool isCopy);
    }

    public interface IWorldObjectActivation
    {
        void OnSubChunkActivated();
        void OnSubChunkDeactivated();
    }
} //end of namespace