using UnityEngine;
using System.Collections;

namespace Game.UI.AbilityMenu
{
    public class DescriptionPanel : MonoBehaviour
    {
        [SerializeField] float timeToAppear = 0.5f;
        [SerializeField] AnimationCurve curve;

        Vector3 visiblePosition, hiddenPosition;
        RectTransform rec;

        public void Awake()
        {
            rec = GetComponent<RectTransform>();

            visiblePosition = rec.localPosition;
            hiddenPosition = new Vector3(rec.localPosition.x + 700, rec.localPosition.y, rec.localPosition.z);
        }

        public void Appear()
        {
            StopCoroutine(_Appear());

            if (gameObject.activeInHierarchy)
                StartCoroutine(_Appear());
        }

        private void OnEnable()
        {
            StartCoroutine(_Appear());
        }

        private void OnDisable()
        {
            StopCoroutine(_Appear());
        }

        IEnumerator _Appear()
        {
            if (!rec) yield break;

            for(float elapsed = 0; elapsed < timeToAppear; elapsed += Time.unscaledDeltaTime)
            {
                float t = elapsed / timeToAppear;
                rec.localPosition = Vector3.Lerp(hiddenPosition, visiblePosition, curve.Evaluate(t));
                yield return null;
            }
        }

    }
}