//using Game.GameControl;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Game.UI
//{
//    public class EndMenuController : MonoBehaviour, IUiMenu
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
//                if (Application.isEditor)
//                {
//#if UNITY_EDITOR
//                    UnityEditor.EditorApplication.isPlaying = false;
//#endif
//                }
//                else
//                {
//                    Application.Quit();
//                }
//            }
//        }

//        #endregion monobehaviour methods

//        //###########################################################

//        void IUiMenu.Initialize(GameController gameController, UiController ui_controller)
//        {
//            //this.playerModel = playerModel;
//        }

//        void IUiMenu.Activate()
//        {
//            if (IsActive)
//            {
//                return;
//            }

//            IsActive = true;
//            this.gameObject.SetActive(true);
//        }

//        void IUiMenu.Deactivate()
//        {
//            bool wasActive = this.IsActive;

//            this.IsActive = false;
//            this.gameObject.SetActive(false);
//        }

//        //###########################################################

//        public void HandleInput()
//        {
//        }
//    }
//}