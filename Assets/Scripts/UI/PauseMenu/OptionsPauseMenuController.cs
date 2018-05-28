using Game.Model;
using UnityEngine;

namespace Game.UI.PauseMenu
{
    public class OptionsPauseMenuController : MonoBehaviour, IPauseMenu
    {
        //###########################################################

        // -- ATTRIBUTES

        public bool IsActive { get; private set; }

        private PlayerModel Model;
        private PauseMenuController PauseMenuController;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(PlayerModel model, PauseMenuController pause_menu_controller)
        {
            Model = model;
            PauseMenuController = pause_menu_controller;
        }

        public void Activate()
        {
            this.gameObject.SetActive(true);
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

        }
    }
}