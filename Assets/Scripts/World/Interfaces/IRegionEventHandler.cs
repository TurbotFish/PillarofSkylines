using UnityEngine.EventSystems;

namespace Game.World
{
    public interface IRegionEventHandler : IEventSystemHandler
    {
#if UNITY_EDITOR
        void CreateSubScene(eSubSceneVariant subSceneVariant, eSubSceneLayer subSceneLayer);

        void AdjustBounds();

        void SetDrawBounds(bool drawBounds);
#endif
    }
} //end of namespace