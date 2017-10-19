using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game.Player.UI
{
    public class HudController : MonoBehaviour, IUiState
    {
        [SerializeField]
        TMPro.TextMeshProUGUI messageView;

        PlayerModel playerModel;

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

        }

        #endregion monobehaviour methods

        //###########################################################

        void IUiState.Initialize(PlayerModel playerModel)
        {
            this.playerModel = playerModel;
        }

        void IUiState.Activate()
        {
            if (this.IsActive)
            {
                return;
            }

            this.IsActive = true;
            this.gameObject.SetActive(true);

            this.messageView.text = "";
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