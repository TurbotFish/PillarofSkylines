using Game.GameControl;

namespace Game.UI
{
    public interface IUiMenu
    {
        bool IsActive { get; }

        void Initialize(IGameController gameController, UiController ui_controller);

        void Activate(Utilities.EventManager.OnShowMenuEventArgs args);

        bool HandleInput();

        void Deactivate();
    }
} // end of namespace