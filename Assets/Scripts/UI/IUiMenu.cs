using Game.GameControl;

namespace Game.UI
{
    public interface IUiMenu
    {
        bool IsActive { get; }

        void Initialize(GameController gameController, UiController ui_controller);

        void Activate();

        void HandleInput();

        void Deactivate();
    }
} // end of namespace