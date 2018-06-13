using System.Collections;
using UnityEngine;


public class TopPanel : MonoBehaviour {

    RectTransform myTransform;

    [SerializeField] float duration = 0.1f;
    [SerializeField] AnimationCurve curve;

    private void Awake()
    {
        myTransform = transform as RectTransform;
    }

    private void OnEnable()
    {
        StartCoroutine(_Entrance());
    }

    IEnumerator _Entrance()
    {
        Vector3 pos = myTransform.anchoredPosition;

        for (float elapsed = 0; elapsed < duration; elapsed += Time.unscaledDeltaTime)
        {
            pos.y = Mathf.Lerp(250, 0, curve.Evaluate(elapsed / duration));
            myTransform.anchoredPosition = pos;
            yield return null;
        }

    }

}
