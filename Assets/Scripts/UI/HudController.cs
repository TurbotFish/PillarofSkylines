using UnityEngine;
using Game.GameControl;
using System.Collections;

namespace Game.UI
{

    public enum eMessageType
    {
        Help = 0,
        Important,
        Announcement
    }

    public class HudController : MonoBehaviour, IUiMenu
    {
        //###########################################################

        // -- CONSTANTS

        [Header("Help")]
        [SerializeField] private TMPro.TextMeshProUGUI helpMessage;
        [SerializeField] private float helpFadeTime = 0.1f;

        [Header("Important")]
        [SerializeField] private TMPro.TextMeshProUGUI importantTitle;
        [SerializeField] private TMPro.TextMeshProUGUI importantDescription;
        [SerializeField] private float importantFadeTime = 0.5f;

        [Header("Announce")]
        [SerializeField] private TMPro.TextMeshProUGUI announcement;
        [SerializeField] private TMPro.TextMeshProUGUI announcementDescription;
        [SerializeField] private float announceFadeTime = 0.5f;

        public CanvasGroup myRenderer;

        //###########################################################

        // -- ATTRIBUTES

        public bool IsActive { get; private set; }

        GameObject helpPanel;
        CanvasGroup helpRenderer;

        GameObject importantPanel;
        CanvasGroup importantRenderer;

        GameObject announcePanel;
        CanvasGroup announceRenderer;
        float announceTime;
        bool announcePanelActive;

        private GameController GameController;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(GameController gameController, UiController ui_controller)
        {
            GameController = gameController;
            helpMessage.text = "";
        }

        public void Activate(Utilities.EventManager.OnShowMenuEventArgs args)
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;
            myRenderer.alpha = 1;
            StopAllCoroutines();
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            if (!IsActive)
            {
                return;
            }

            IsActive = false;

            if (myRenderer)
            {
                Display(myRenderer, false, 0.5f);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        //###########################################################

        // -- OPERATIONS

        public void HandleInput()
        {
            if (Input.GetButtonDown("MenuButton"))
            {
                GameController.OpenPauseMenu();
                return;
            }
            else if (Input.GetButtonDown("Interact") && importantPanel.activeSelf)
            {
                Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false, "", eMessageType.Important));
                return;
            }
        }

        private void Start()
        {
            //myRenderer = GetComponent<CanvasGroup>();

            helpPanel = helpMessage.transform.parent.gameObject;
            helpPanel.SetActive(false);
            helpRenderer = helpPanel.GetComponent<CanvasGroup>();

            importantPanel = importantTitle.transform.parent.gameObject;
            importantPanel.SetActive(false);
            importantRenderer = importantPanel.GetComponent<CanvasGroup>();

            announcePanel = announcement.transform.parent.gameObject;
            announcePanel.SetActive(false);
            announceRenderer = announcePanel.GetComponent<CanvasGroup>();

            Utilities.EventManager.OnShowHudMessageEvent += OnShowHudMessageEventHandler;
        }

        private void Update()
        {
            if (!IsActive)
                return;

            if (announcePanelActive && announceTime > 0)
            {
                announceTime -= Time.unscaledDeltaTime;
                if (announceTime <= 0)
                {
                    announcePanelActive = false;
                    Display(announceRenderer, false, announceFadeTime);
                }
            }
        }

        private void Display(CanvasGroup canvas, bool active, float fadeTime)
        {
            StartCoroutine(_Display(canvas, active, fadeTime));
        }

        private IEnumerator _Display(CanvasGroup canvas, bool active, float fadeTime)
        {
            if (active)
                canvas.gameObject.SetActive(true);

            for (float elapsed = 0; elapsed < fadeTime; elapsed += Time.unscaledDeltaTime)
            {
                float t = elapsed / fadeTime;
                if (!active) t = 1 - t;
                canvas.alpha = t;
                yield return null;
            }

            if (!active)
                canvas.gameObject.SetActive(false);
            else
                canvas.alpha = 1;
        }

        private void OnShowHudMessageEventHandler(object sender, Utilities.EventManager.OnShowHudMessageEventArgs args)
        {
            //Debug.LogFormat("showHudMessageEvent: show={0}, message={1}", args.Show.ToString(), args.Message);

            if (!IsActive)
                return;

            switch (args.MessageType)
            {
                default:
                case eMessageType.Help:
                    Display(helpRenderer, args.Show, helpFadeTime);
                    if (args.Show)
                        helpMessage.text = args.Message;
                    break;

                case eMessageType.Important:
                    Display(importantRenderer, args.Show, importantFadeTime);
                    if (args.Show)
                    {
                        importantTitle.text = args.Message;
                        importantDescription.text = args.Description;
                    }
                    break;

                case eMessageType.Announcement:
                    announcePanelActive = args.Show;
                    Display(announceRenderer, args.Show, announceFadeTime);
                    if (args.Show)
                    {
                        announcement.text = args.Message;
                        announcementDescription.text = args.Description;
                        announceTime = args.Time;
                    }
                    else
                        announceTime = 0;
                    break;
            }
        }
    }
} //end of namespace