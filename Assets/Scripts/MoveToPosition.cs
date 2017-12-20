using UnityEngine;
using System.Collections;

public class MoveToPosition : MonoBehaviour {

    [SerializeField] Vector3 targetPos;
    [SerializeField] AnimationCurve curve;
    [SerializeField] float delay = 0;
    [SerializeField] float duration = 30;
    [SerializeField] bool inverse;

    Transform my;
    Vector3 startPos;

    void Start () {
        my = transform;
        startPos = my.localPosition;
        StartCoroutine(Move());
	}

    IEnumerator Move() {

        yield return new WaitForSeconds(delay);

        for (float elapsed = 0; elapsed < duration; elapsed+=Time.deltaTime) {
            float t = curve.Evaluate(elapsed / duration);
            if (inverse)
                my.localPosition = Vector3.Lerp(targetPos, startPos, t);
            else
                my.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

    }
}
