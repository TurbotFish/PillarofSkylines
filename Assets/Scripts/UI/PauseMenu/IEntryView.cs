using Game.Model;

namespace Game.UI.PauseMenu
{
    public interface IEntryView
    {
        void Initialize(PlayerModel model, SkillsMenuController skills_menu_controller);
    }
} // end of namespace