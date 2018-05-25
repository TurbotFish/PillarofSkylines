using Game.GameControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public interface IEntryView
    {
        void Initialize(IGameController game_controller, SkillsMenuController skills_menu_controller);
    }
} // end of namespace