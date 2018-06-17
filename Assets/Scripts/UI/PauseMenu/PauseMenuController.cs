using System.Collections;
using System.Collections.Generic;
using Game.GameControl;
using Game.Utilities;
using UnityEngine;

namespace Game.UI.PauseMenu
{
    public class PauseMenuController : MonoBehaviour, IUiMenu
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private OverviewMenuController OverviewPauseMenuController;
        [SerializeField] private SkillsMenuController SkillsPauseMenuController;
        [SerializeField] private OptionsPauseMenuController OptionsPauseMenuController;

        //###########################################################

        // -- ATTRIBUTES

        public bool IsActive { get; private set; }

        public GameController GameController;
        private UiController UiController;

        private PauseMenuType CurrentState;
        private Dictionary<PauseMenuType, IPauseMenu> SubMenuDictionary = new Dictionary<PauseMenuType, IPauseMenu>();

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(GameController game_controller, UiController ui_controller)
        {
            GameController = game_controller;
            UiController = ui_controller;

            SubMenuDictionary.Clear();
            SubMenuDictionary.Add(PauseMenuType.Overview, OverviewPauseMenuController);
            SubMenuDictionary.Add(PauseMenuType.Skills, SkillsPauseMenuController);
            SubMenuDictionary.Add(PauseMenuType.Options, OptionsPauseMenuController);

            foreach (var menu in SubMenuDictionary.Values)
            {
                menu.Initialize(GameController.PlayerModel, this);
                menu.Deactivate();
            }
        }

        public void Activate()
        {
            this.gameObject.SetActive(true);

            SwitchPauseMenu(PauseMenuType.Overview);

            IsActive = true;
        }

        public void Deactivate()
        {
            this.gameObject.SetActive(false);
            IsActive = false;
        }

        //###########################################################

        // -- OPERATIONS

        public void HandleInput()
        {
            if (Input.GetButtonDown("MenuButton") || (CurrentState == PauseMenuType.Overview && Input.GetButtonDown("Cancel")))
                ClosePauseMenu();

            SubMenuDictionary[CurrentState].HandleInput();
        }

        public void SwitchPauseMenu(PauseMenuType new_menu_type)
        {
            if (SubMenuDictionary[CurrentState].IsActive)
            {
                SubMenuDictionary[CurrentState].Deactivate();
            }

            CurrentState = new_menu_type;

            SubMenuDictionary[CurrentState].Activate();
        }

        public void ClosePauseMenu()
        {
            GameController.ClosePauseMenu();
        }

        public void ExitGame()
        {
            UiController.ExitGame();
        }
    }
} // end of namespace