using System.Collections;
using UnityEngine;

public class RetractingDoor : TriggerableObject {

    [Header("Door")]
    public Bool3 retractingAxis;
    public float timeToRetract = 1;
    public bool startOpen;
    Transform my;
    Vector3 startScale;

    public void Awake() {
        my = transform;
        startScale = my.localScale;
        if (startOpen) {
            Vector3 targetScale = new Vector3(retractingAxis.x ? 0 : 1,
                                              retractingAxis.y ? 0 : 1,
                                              retractingAxis.z ? 0 : 1);
            my.localScale = targetScale;
        }

    }

    void Toggle(bool yo) {
        if (yo) {
            Retract(startScale);
        } else {
            Vector3 targetScale = new Vector3(retractingAxis.x ? 0 : 1,
                                              retractingAxis.y ? 0 : 1,
                                              retractingAxis.z ? 0 : 1);
            Retract(targetScale);
        }
    }

    protected override void Activate() {
        Toggle(startOpen);
    }

    protected override void Deactivate() {
        Toggle(!startOpen);
    }

    void Retract(Vector3 targetScale) {
        StopAllCoroutines();
        StartCoroutine(_Retract(targetScale));
    }
    
    IEnumerator _Retract(Vector3 targetScale) {

        Vector3 startScale = my.localScale;
        for (float elapsed = 0; elapsed < timeToRetract; elapsed+=Time.deltaTime) {

            my.localScale = Vector3.Lerp(startScale, targetScale, elapsed / timeToRetract);
            yield return null;
        }
        my.localScale = targetScale;
    }

}
