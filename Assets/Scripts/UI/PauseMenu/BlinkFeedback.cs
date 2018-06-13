using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlinkFeedback : MonoBehaviour {

    [SerializeField] Image img;
    [SerializeField] float growth = 2;
    [SerializeField] float duration = 0.2f;
    [SerializeField] AnimationCurve curve;

    RectTransform myTransform;

	void Start () {
        myTransform = transform as RectTransform;

        StartCoroutine(_Feedback());
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }

    IEnumerator _Feedback()
    {
        Color color = img.color;
        float startAlpha = color.a;
        Vector2 startScale = myTransform.sizeDelta;

        for (float elapsed =0; elapsed < duration; elapsed += Time.unscaledDeltaTime)
        {
            float t = curve.Evaluate(elapsed / duration);

            color.a = Mathf.Lerp(startAlpha, 0, t);
            img.color = color;

            myTransform.sizeDelta = Vector2.Lerp(startScale, startScale * growth, t);

            yield return null;
        }

        Destroy(gameObject);
    }

}
