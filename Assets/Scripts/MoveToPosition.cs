using UnityEngine;
using System.Collections;

public class MoveToPosition : MonoBehaviour {

    [SerializeField] Vector3 targetPos;
    [SerializeField] AnimationCurve curve;
    [SerializeField] float duration = 30;

    Transform my;
    Vector3 startPos;

    void Start () {
        my = transform;
        startPos = my.localPosition;
        StartCoroutine(Move());
	}

    IEnumerator Move() {

        for (float elapsed = 0; elapsed < duration; elapsed+=Time.deltaTime) {
            float t = curve.Evaluate(elapsed / duration);
            my.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

    }
}
