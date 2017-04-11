using UnityEngine;
using System.Collections;

public class EchoCameraEffect : MonoBehaviour {

    new Camera camera;
    float previousFov;

    [SerializeField]
    AnimationCurve FOVChange;

    void Start() {
        camera = GetComponent<Camera>();
        previousFov = camera.fieldOfView;
    }

    public void SetFov(float newFov, float time, bool goBack = false) {
        StartCoroutine(_SetFov(newFov, time, goBack));
    }

    IEnumerator _SetFov(float newFov, float time, bool goBack = false) {

        previousFov = camera.fieldOfView;

        for (float elapsed = 0; elapsed < time; elapsed += Time.deltaTime) {

            camera.fieldOfView = Mathf.LerpUnclamped(previousFov, newFov, FOVChange.Evaluate(elapsed / time));
            yield return null;
        }
        camera.fieldOfView = newFov;
        
        if (goBack) {
            SetFov(previousFov, time);
        }
    }

}
