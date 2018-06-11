using Game.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Game.UI.PauseMenu
{
    public class DescriptionPanelView : MonoBehaviour
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private TextMeshProUGUI NameTextComponent;
        [SerializeField] private Text DescriptionTextComponent;
        [SerializeField] private Image NameTextUnderline;
        [SerializeField] private Image IconImageComponent;
        [SerializeField] RectTransform FXicon;
        [SerializeField] float iconFXDuration = 0.2f;
        [SerializeField] float nameMoveDuration = 100f;

        //###########################################################

        // -- ATTRIBUTES

        private PlayerModel Model;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(PlayerModel model)
        {
            Model = model;
        }

        //###########################################################

        // -- OPERATIONS

        public void ShowAbility(AbilityType ability)
        {
            var ability_data = Model.AbilityData.GetAbility(ability);
            StopAllCoroutines();
            StartCoroutine(_PlayIconFX());
            StartCoroutine(_MoveTitle());

            NameTextComponent.text = ability_data.Name;
            DescriptionTextComponent.text = ability_data.Description;
            IconImageComponent.sprite = ability_data.Icon;
        }

        IEnumerator _PlayIconFX()
        {
            Vector3 pos = new Vector3(100, 100, 0);

            for(float elapsed = 0; elapsed < iconFXDuration; elapsed += Time.unscaledDeltaTime)
            {
                float t = elapsed / iconFXDuration;
                FXicon.anchoredPosition = Vector3.Lerp(-pos, pos, t);
                yield return null;
            }
            FXicon.localPosition = pos;
        }

        IEnumerator _MoveTitle()
        {
            Vector3 namePos = NameTextComponent.rectTransform.anchoredPosition;
            Vector3 underlinePos = NameTextUnderline.rectTransform.anchoredPosition;

            for (float elapsed = 0; elapsed < nameMoveDuration; elapsed += Time.unscaledDeltaTime)
            {
                float t = elapsed / nameMoveDuration;

                t = Mathf.Sqrt(1 - (--t) * t);

                namePos.x = Mathf.Lerp(70, 200, t);
                underlinePos.x = Mathf.Lerp(0, -140, t);

                NameTextComponent.rectTransform.anchoredPosition = namePos;
                NameTextUnderline.rectTransform.anchoredPosition = underlinePos;
                yield return null;
            }

        }

    }
} // end of namespace