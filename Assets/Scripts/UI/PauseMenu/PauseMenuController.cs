using System.Collections;
using System.Collections.Generic;
using Game.GameControl;
using Game.Utilities;
using UnityEngine;

namespace Game.UI
{
    public class PauseMenuController : MonoBehaviour, IUiMenu
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private OverviewPauseMenuController OverviewPauseMenuController;
        [SerializeField] private SkillsPauseMenuController SkillsPauseMenuController;
        [SerializeField] private OptionsPauseMenuController OptionsPauseMenuController;

        //###########################################################

        // -- ATTRIBUTES

        public bool IsActive { get; private set; }

        private IGameController GameController;
        private PauseMenuType CurrentState;
        private Dictionary<PauseMenuType, IUiMenu> SubMenuDictionary = new Dictionary<PauseMenuType, IUiMenu>();

        //###########################################################

        // -- INITIALIZATION

        void IUiMenu.Initialize(IGameController game_controller)
        {
            GameController = game_controller;

            SubMenuDictionary.Clear();
            SubMenuDictionary.Add(PauseMenuType.Overview, OverviewPauseMenuController);
            SubMenuDictionary.Add(PauseMenuType.Skills, SkillsPauseMenuController);
            SubMenuDictionary.Add(PauseMenuType.Options, OptionsPauseMenuController);

            foreach(var menu in SubMenuDictionary.Values)
            {
                menu.Initialize(game_controller);
                menu.Deactivate();
            }
        }

        //###########################################################

        // -- OPERATIONS

        void IUiMenu.Activate(EventManager.OnShowMenuEventArgs args)
        {
            SwitchMenu(PauseMenuType.Overview);

            this.gameObject.SetActive(true);
            IsActive = true;
        }

        void IUiMenu.Deactivate()
        {
            this.gameObject.SetActive(false);
            IsActive = false;
        }

        
        public void SwitchMenu(PauseMenuType new_menu_type)
        {
            if (SubMenuDictionary[CurrentState].IsActive)
            {
                SubMenuDictionary[CurrentState].Deactivate();
            }

            CurrentState = new_menu_type;

            SubMenuDictionary[CurrentState].Activate(null);
        }
    }
} // end of namespace