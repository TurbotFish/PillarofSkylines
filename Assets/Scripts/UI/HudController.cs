using UnityEngine;
using Game.GameControl;

namespace Game.UI
{

    public enum eMessageType
    {
        Help = 0,
        Important
    }

    public class HudController : MonoBehaviour, IUiState
    {
        [SerializeField]
        TMPro.TextMeshProUGUI helpMessage;
        GameObject helpPanel;
        
        [SerializeField]
        TMPro.TextMeshProUGUI importantTitle;
        [SerializeField]
        TMPro.TextMeshProUGUI importantDescription;
        GameObject importantPanel;

        public bool IsActive { get; private set; }


        //###########################################################

        #region monobehaviour methods
            
        void Start()
        {
            helpPanel = helpMessage.transform.parent.gameObject;
            helpPanel.SetActive(false);

            importantPanel = importantTitle.transform.parent.gameObject;
            importantPanel.SetActive(false);

            Utilities.EventManager.OnShowHudMessageEvent += OnShowHudMessageEventHandler;
        }
        
        void Update()
        {
            if (!IsActive)
                return;

            if (Input.GetButtonDown("MenuButton"))
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(eUiState.AbilityMenu));
                return;
            }
            else if (Input.GetButtonDown("Back"))
            {
                Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(eUiState.HelpMenu));
                return;
            }
            else if (Input.GetButtonDown("Interact") && importantPanel.activeSelf)
            {
                Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false, "", eMessageType.Important));
                return;
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        void IUiState.Initialize(IGameControllerBase gameController)
        {
            helpMessage.text = "";
        }

        void IUiState.Activate(Utilities.EventManager.OnShowMenuEventArgs args)
        {
            if (IsActive)
                return;
            IsActive = true;
            gameObject.SetActive(true);          
        }

        void IUiState.Deactivate()
        {
            IsActive = false;
            gameObject.SetActive(false);
        }

        //###########################################################

        void OnShowHudMessageEventHandler(object sender, Utilities.EventManager.OnShowHudMessageEventArgs args)
        {
            Debug.LogFormat("showHudMessageEvent: show={0}, message={1}", args.Show.ToString(), args.Message);

            switch (args.MessageType)
            {
                default:
                case eMessageType.Help:
                    helpPanel.SetActive(args.Show);
                    if (args.Show)
                        helpMessage.text = args.Message;
                    else
                        helpMessage.text = "";
                    break;

                case eMessageType.Important:
                    importantPanel.SetActive(args.Show);
                    if (args.Show)
                    {
                        importantTitle.text = args.Message;
                        importantDescription.text = args.Description;
                    }
                    else
                    {
                        importantTitle.text = "";
                        importantDescription.text = "";
                    }
                    break;
            }
        }

        //###########################################################
    }
} //end of namespace