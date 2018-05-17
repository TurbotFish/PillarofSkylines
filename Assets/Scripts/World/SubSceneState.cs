using System;

namespace Game.World
{
    [Flags]
    public enum SubSceneState
    {
        Unloaded = 1,
        Loading = 2,
        Loaded = 4,
        Unloading = 8
    }
} //end of namespace