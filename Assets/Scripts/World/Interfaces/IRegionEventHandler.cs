using UnityEngine.EventSystems;

namespace Game.World
{
    public interface IRegionEventHandler : IEventSystemHandler
    {
#if UNITY_EDITOR
        void CreateSubScene(eSubSceneMode subSceneMode, eSubSceneType subSceneType);

        void AdjustBounds();
#endif
    }
} //end of namespace