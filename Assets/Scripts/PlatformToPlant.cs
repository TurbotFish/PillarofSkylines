using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformToPlant : MonoBehaviour {

    public AnimationCurve curve;
    public float animationDuration;

    Vector3 origin;

    public void PlantInto(Vector3 position) {
        origin = transform.position;
        StartCoroutine(_PlantInto(position));
    }

    private IEnumerator _PlantInto(Vector3 position) {

        for(float elapsed = 0; elapsed < animationDuration; elapsed += Time.deltaTime) {
            float t = elapsed / animationDuration;

            transform.position = Vector3.LerpUnclamped(origin, position, curve.Evaluate(t));

            yield return null;
        }
        transform.position = position;
    }
}
