using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Game.UI.PauseMenu
{
    public class PauseMenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {

        RectTransform myTransform;
        [SerializeField] float transitionDuration = 0.1f;

        [SerializeField] float selectedWidth = 540;
        [SerializeField] float deselectedWidth = 320;


        private void Awake()
        {
            myTransform = transform as RectTransform;
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
            Vector2 size = myTransform.sizeDelta;

            size.x = deselectedWidth;
            myTransform.sizeDelta = size;
        }

        IEnumerator _Transition(bool selected)
        {
            Vector2 size = myTransform.sizeDelta;

            size.x = selected ? deselectedWidth : selectedWidth;
            myTransform.sizeDelta = size;

            for (float elapsed = 0; elapsed < transitionDuration; elapsed += Time.unscaledDeltaTime)
            {
                float t = elapsed / transitionDuration;

                size.x = Mathf.Lerp(selected ? deselectedWidth : selectedWidth, selected ? selectedWidth : deselectedWidth, elapsed / transitionDuration);
                myTransform.sizeDelta = size;

                yield return null;
            }

            size.x = selected ? selectedWidth : deselectedWidth;
            myTransform.sizeDelta = size;
        }

    }
}