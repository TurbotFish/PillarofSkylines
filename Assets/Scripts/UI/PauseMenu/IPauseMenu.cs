using Game.Model;

namespace Game.UI.PauseMenu
{
    public interface IPauseMenu
    {
        bool IsActive { get; }

        void Initialize(PlayerModel model, PauseMenuController pause_menu_controller);

        void Activate();

        void HandleInput();

        void Deactivate();
    }
} // end of namespace