using UnityEngine;
using UnityEngine.UI;
using Game.GameControl;
using System.Collections;
using TMPro;

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

        [SerializeField] private CanvasGroup MyCanvasGroup;

        [Header("Help")]
        [SerializeField] private GameObject HelpPanel;
        [SerializeField] private CanvasGroup HelpCanvasGroup;
        [SerializeField] private TextMeshProUGUI HelpText;
        [SerializeField] private float HelpFadeTime = 0.2f;

        [Header("Announce")]
        [SerializeField] private GameObject AnnouncmentPanel;
        [SerializeField] private CanvasGroup AnnouncmentCanvasGroup;
        [SerializeField] private TextMeshProUGUI AnnouncmentTitleText;
        [SerializeField] private TextMeshProUGUI AnnouncmentDescriptionText;
        [SerializeField] private RectTransform Brackets;
        [SerializeField] private RectTransform Icon;
        [SerializeField] private float AnnouncmentFadeTime = 0.5f;
        [SerializeField] float BracketStartWidth = 320, BracketEndWidth = 450;
        [SerializeField] float IconStartSize = 40, IconEndSize = 90;

        //###########################################################

        // -- ATTRIBUTES

        public bool IsActive { get; private set; }

        private GameController GameController;

        private string HelpMessageId;
        private bool IsHelpPanelVisible;
        private bool IsHelpPanelFadingIn;
        private bool IsHelpPanelFadingOut;

        private bool IsAnnouncmentPanelVisible;
        private bool IsAnnouncmentPanelFadingIn;
        private bool IsAnnouncmentPanelFadingOut;
        private float AnnouncmentTime;

        //###########################################################

        // -- INITIALIZATION

        /// <summary>
        /// Initializes the Hud Menu.
        /// </summary>
        /// <param name="gameController"></param>
        /// <param name="ui_controller"></param>
        public void Initialize(GameController gameController, UiController ui_controller)
        {
            if (!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
            }

            GameController = gameController;

            if (!HelpPanel.activeSelf)
            {
                HelpPanel.SetActive(true);
            }

            HelpCanvasGroup.alpha = 0;
            HelpText.text = "";

            if (!AnnouncmentPanel.activeSelf)
            {
                AnnouncmentPanel.SetActive(true);
            }

            AnnouncmentCanvasGroup.alpha = 0;
            AnnouncmentTitleText.text = "";
            AnnouncmentDescriptionText.text = "";
        }

        /// <summary>
        /// Shows the Hud Menu.
        /// </summary>
        public void Activate()
        {
            if (IsActive)
            {
                return;
            }

            if (!this.gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            MyCanvasGroup.alpha = 1;
            IsActive = true;
        }

        /// <summary>
        /// Hides the Hud Menu.
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive)
            {
                return;
            }

            MyCanvasGroup.alpha = 0;
            IsActive = false;
        }

        //###########################################################

        // -- OPERATIONS

        /// <summary>
        /// Shows the message in the Help Panel.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="id"></param>
        public void ShowHelpMessage(string message, string id)
        {
            HelpText.text = message;
            HelpMessageId = id;

            if (IsHelpPanelFadingOut)
            {
                StopCoroutine(FadeOutHelpPanelCoroutine());
                IsHelpPanelFadingOut = false;
            }

            if (!IsHelpPanelVisible && !IsHelpPanelFadingIn)
            {
                StartCoroutine(FadeInHelpPanelCoroutine());
            }
        }

        /// <summary>
        /// Hides the Help Panel if the id is empty or corresponds to the id of the current message.
        /// </summary>
        /// <param name="id"></param>
        public void HideHelpMessage(string id = "")
        {
            if (id != "" && id != HelpMessageId)
            {
                return;
            }

            HelpMessageId = "";

            if (IsHelpPanelFadingIn)
            {
                StopCoroutine(FadeInHelpPanelCoroutine());
                IsHelpPanelFadingIn = false;
            }

            if (IsHelpPanelVisible && !IsHelpPanelFadingOut)
            {
                StartCoroutine(FadeOutHelpPanelCoroutine());
            }
        }

        /// <summary>
        /// Shows an announcment.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="time"></param>
        public void ShowAnnouncmentMessage(string title, string description, float time = 4f, Sprite image = null)
        {
            if (IsAnnouncmentPanelFadingIn)
            {
                Debug.LogError("HudController: ShowAnnouncmentMessage: an announcment is currently fading in!");

                return;
            }

            if (image)
                Icon.GetComponent<Image>().sprite = image;

            StartCoroutine(ReplaceAnnouncmentMessageCoroutine(title, description, time));
        }

        /// <summary>
        /// Handles Input.
        /// </summary>
        public void HandleInput()
        {
            if (Input.GetButtonDown("MenuButton"))
            {
                GameController.OpenPauseMenu();
            }
            //else if (Input.GetButtonDown("Interact") && importantPanel.activeSelf)
            //{
            //    Utilities.EventManager.SendShowHudMessageEvent(this, new Utilities.EventManager.OnShowHudMessageEventArgs(false, "", eMessageType.Important));
            //}
        }

        /// <summary>
        /// Coroutine to fade in the Help Panel.
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeInHelpPanelCoroutine()
        {
            IsHelpPanelFadingIn = true;

            for (float time_elapsed = (1 - HelpCanvasGroup.alpha) * HelpFadeTime; time_elapsed <= HelpFadeTime; time_elapsed += Time.unscaledDeltaTime)
            {
                HelpCanvasGroup.alpha = time_elapsed / HelpFadeTime;

                yield return null;
            }

            HelpCanvasGroup.alpha = 1;
            IsHelpPanelFadingIn = false;
            IsHelpPanelVisible = true;
        }

        /// <summary>
        /// Coroutine to fade out the Help Panel.
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeOutHelpPanelCoroutine()
        {
            IsHelpPanelFadingOut = true;

            for (float time_elapsed = HelpCanvasGroup.alpha * HelpFadeTime; time_elapsed <= HelpFadeTime; time_elapsed += Time.unscaledDeltaTime)
            {
                HelpCanvasGroup.alpha = 1 - (time_elapsed / HelpFadeTime);

                yield return null;
            }

            HelpCanvasGroup.alpha = 0;
            IsHelpPanelFadingOut = false;
            IsHelpPanelVisible = false;
        }

        /// <summary>
        /// Coroutine to fade in the Announcment Panel.
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeInAnnouncmentPanelCoroutine(float time)
        {
            IsAnnouncmentPanelFadingIn = true;

            StartCoroutine(OpenIconFeedback(time));

            for (float time_elapsed = 0; time_elapsed <= AnnouncmentFadeTime; time_elapsed += Time.unscaledDeltaTime)
            {
                float t = time_elapsed / AnnouncmentFadeTime;

                AnnouncmentCanvasGroup.alpha = t;
                
                yield return null;
            }

            AnnouncmentCanvasGroup.alpha = 1;
            IsAnnouncmentPanelFadingIn = false;
            IsAnnouncmentPanelVisible = true;
            AnnouncmentTime = time;
        }

        IEnumerator OpenIconFeedback(float time) {

            Vector3 bracketSize = Brackets.sizeDelta;
            Vector3 iconSize = Icon.sizeDelta;

            for (float time_elapsed = 0; time_elapsed <= time; time_elapsed += Time.unscaledDeltaTime)
            {
                float t = time_elapsed / time;

                t = Mathf.Sqrt(1 - (--t) * t);

                bracketSize.x = Mathf.Lerp(BracketStartWidth, BracketEndWidth, t);
                Brackets.sizeDelta = bracketSize;

                iconSize = Vector3.one * Mathf.Lerp(IconStartSize, IconEndSize, t);
                Icon.sizeDelta = iconSize;
                
                yield return null;
            }

        }

        /// <summary>
        /// Coroutine to fade out the Announcment Panel.
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeOutAnnouncmentPanelCoroutine()
        {
            IsAnnouncmentPanelFadingOut = true;

            for (float time_elapsed = 0; time_elapsed <= AnnouncmentFadeTime; time_elapsed += Time.unscaledDeltaTime)
            {
                AnnouncmentCanvasGroup.alpha = 1 - (time_elapsed / AnnouncmentFadeTime);

                yield return null;
            }

            AnnouncmentCanvasGroup.alpha = 0;
            IsAnnouncmentPanelFadingOut = false;
            IsAnnouncmentPanelVisible = false;
        }

        /// <summary>
        /// Coroutine to fade out the Announcment Panel and then fade it in again with new content.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private IEnumerator ReplaceAnnouncmentMessageCoroutine(string title, string description, float time)
        {
            if (IsAnnouncmentPanelVisible && !IsAnnouncmentPanelFadingOut)
            {
                StartCoroutine(FadeOutAnnouncmentPanelCoroutine());
            }

            while (IsAnnouncmentPanelVisible)
            {
                yield return null;
            }

            AnnouncmentTitleText.text = title;
            AnnouncmentDescriptionText.text = description;

            StartCoroutine(FadeInAnnouncmentPanelCoroutine(time));
        }

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            if (IsAnnouncmentPanelVisible && !IsAnnouncmentPanelFadingOut)
            {
                AnnouncmentTime -= Time.unscaledDeltaTime;

                if (AnnouncmentTime <= 0)
                {
                    StartCoroutine(FadeOutAnnouncmentPanelCoroutine());
                }
            }

            //if (!IsActive)
            //    return;

            //if (announcePanelActive && announceTime > 0)
            //{
            //    announceTime -= Time.unscaledDeltaTime;
            //    if (announceTime <= 0)
            //    {
            //        announcePanelActive = false;
            //        Display(announceRenderer, false, AnnouncmentFadeTime);
            //    }
            //}
        }

        //private void Display(CanvasGroup canvas, bool active, float fadeTime)
        //{
        //    StartCoroutine(_Display(canvas, active, fadeTime));
        //}

        //private IEnumerator _Display(CanvasGroup canvas, bool active, float fadeTime)
        //{
        //    if (active)
        //        canvas.gameObject.SetActive(true);

        //    for (float elapsed = 0; elapsed < fadeTime; elapsed += Time.unscaledDeltaTime)
        //    {
        //        float t = elapsed / fadeTime;
        //        if (!active) t = 1 - t;
        //        canvas.alpha = t;
        //        yield return null;
        //    }

        //    if (!active)
        //        canvas.gameObject.SetActive(false);
        //    else
        //        canvas.alpha = 1;
        //}

        //private void OnShowHudMessageEventHandler(object sender, Utilities.EventManager.OnShowHudMessageEventArgs args)
        //{
        //    //Debug.LogFormat("showHudMessageEvent: show={0}, message={1}", args.Show.ToString(), args.Message);

        //    if (!IsActive)
        //        return;

        //    switch (args.MessageType)
        //    {
        //        default:
        //        case eMessageType.Help:
        //            Display(helpRenderer, args.Show, HelpFadeTime);
        //            if (args.Show)
        //                HelpText.text = args.Message;
        //            break;

        //        case eMessageType.Important:
        //            Display(importantRenderer, args.Show, importantFadeTime);
        //            if (args.Show)
        //            {
        //                importantTitle.text = args.Message;
        //                importantDescription.text = args.Description;
        //            }
        //            break;

        //        case eMessageType.Announcement:
        //            announcePanelActive = args.Show;
        //            Display(announceRenderer, args.Show, AnnouncmentFadeTime);
        //            if (args.Show)
        //            {
        //                AnnouncmentTitleText.text = args.Message;
        //                AnnouncmentDescriptionText.text = args.Description;
        //                announceTime = args.Time;
        //            }
        //            else
        //                announceTime = 0;
        //            break;
        //    }
        //}
    }
} //end of namespace