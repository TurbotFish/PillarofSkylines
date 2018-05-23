using Game.GameControl;

namespace Game.UI
{
    public interface IUiMenu
    {
        bool IsActive { get; }

        void Initialize(IGameController gameController);

        void Activate(Utilities.EventManager.OnShowMenuEventArgs args);

        void Deactivate();
    }
}