using Game.GameControl;

namespace Game.UI
{
    public interface IUiMenu
    {
        bool IsActive { get; }

        void Initialize(GameController gameController, UiController ui_controller);

        void Activate(Utilities.EventManager.OnShowMenuEventArgs args);

        void HandleInput();

        void Deactivate();
    }
} // end of namespace