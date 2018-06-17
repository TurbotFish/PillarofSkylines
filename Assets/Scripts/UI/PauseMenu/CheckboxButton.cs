using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace Game.UI.PauseMenu
{
    public class CheckboxButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {

        [SerializeField] RectTransform pointer;
        Image toFill;


        [SerializeField] float transitionDuration = 0.1f;

        [SerializeField] float selectedX = 200;
        [SerializeField] float deselectedX = 100;


        private void Awake()
        {
            toFill = pointer.GetComponent<Image>();
            CloseVisuals();
        }

        public void OnSelect(BaseEventData eventData)
        {
            StartCoroutine(_Transition(true));
        }

        public void OnDeselect(BaseEventData eventData)
        {
            StartCoroutine(_Transition(false));
        }

        void OnDisable()
        {
            CloseVisuals();
        }

        void CloseVisuals()
        {
            toFill.fillAmount = 0;

            Vector3 pos = pointer.position;
            pos.x = deselectedX;
            pointer.anchoredPosition = pos;
        }

        IEnumerator _Transition(bool selected)
        {
            Vector3 pos = pointer.position;

            pos.x = selected ? deselectedX : selectedX;
            pointer.anchoredPosition = pos;

            for (float elapsed = 0; elapsed < transitionDuration; elapsed += Time.unscaledDeltaTime)
            {
                float t = elapsed / transitionDuration;

                toFill.fillAmount = selected ? t : 1 - t;

                pos.x = Mathf.Lerp(selected ? deselectedX : selectedX, selected ? selectedX : deselectedX, elapsed / transitionDuration);
                pointer.anchoredPosition = pos;

                yield return null;
            }

            toFill.fillAmount = selected ? 1 : 0;
            pos.x = selected ? selectedX : deselectedX;
            pointer.anchoredPosition = pos;
        }


    }
}