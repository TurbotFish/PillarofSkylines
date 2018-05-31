//using Game.GameControl;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Game.UI
//{
//    public class IntroMenuController : MonoBehaviour, IUiMenu
//    {
//        //Player.PlayerModel playerModel;

//        public bool IsActive { get; private set; }

//        //###########################################################

//        #region monobehaviour methods

//        // Use this for initialization
//        void Start()
//        {

//        }

//        // Update is called once per frame
//        void Update()
//        {
//            if (!IsActive)
//            {
//                return;
//            }

//            if (Input.GetButtonDown("MenuButton"))
//            {
//                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(MenuType.HUD));
//            }
//        }

//        #endregion monobehaviour methods

//        //###########################################################

//        void IUiMenu.Initialize(IGameController gameController)
//        {
//            //this.playerModel = playerModel;
//        }

//        void IUiMenu.Activate(Utilities.EventManager.OnShowMenuEventArgs args)
//        {
//            if (this.IsActive)
//            {
//                return;
//            }

//            this.IsActive = true;
//            this.gameObject.SetActive(true);
//        }

//        void IUiMenu.Deactivate()
//        {
//            bool wasActive = this.IsActive;

//            this.IsActive = false;
//            this.gameObject.SetActive(false);
//        }

//        //###########################################################
//    }
//} //end of namespace