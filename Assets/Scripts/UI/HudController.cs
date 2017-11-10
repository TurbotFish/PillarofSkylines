using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game.UI
{
    public class HudController : MonoBehaviour, IUiState
    {
        [SerializeField]
        TMPro.TextMeshProUGUI messageView;

        Player.PlayerModel playerModel;

        public bool IsActive { get; private set; }


        //###########################################################

        #region monobehaviour methods

        // Use this for initialization
        void Start()
        {
            this.messageView.gameObject.SetActive(false);
            Utilities.EventManager.OnShowHudMessageEvent += OnShowHudMessageEventHandler;
        }

        // Update is called once per frame
        void Update()
        {
            if (!this.IsActive)
            {
                return;
            }

            if (Input.GetButtonDown("MenuButton"))
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(eUiState.AbilityMenu));
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        void IUiState.Initialize(Player.PlayerModel playerModel)
        {
            this.playerModel = playerModel;

            this.messageView.text = "";
        }

        void IUiState.Activate(Utilities.EventManager.OnShowMenuEventArgs args)
        {
            if (this.IsActive)
            {
                return;
            }

            this.IsActive = true;
            this.gameObject.SetActive(true);          
        }

        void IUiState.Deactivate()
        {
            this.IsActive = false;
            this.gameObject.SetActive(false);
        }

        //###########################################################

        void OnShowHudMessageEventHandler(object sender, Utilities.EventManager.OnShowHudMessageEventArgs args)
        {
            Debug.LogFormat("showHudMessageEvent: show={0}, message={1}", args.Show.ToString(), args.Message);

            if (args.Show)
            {
                this.messageView.gameObject.SetActive(true);
                this.messageView.text = args.Message;
            }
            else
            {
                this.messageView.text = "";
                this.messageView.gameObject.SetActive(false);
            }
        }

        //###########################################################
    }
} //end of namespace