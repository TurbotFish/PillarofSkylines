using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class TriggerBox : Trigger {

    private void Start() {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    [SerializeField]
    string tagToActivate = "Player";
    [SerializeField]
    bool definitiveActivation;
    [SerializeField]
    bool toggle;
    [SerializeField]
    float delayBeforeDeactivation;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == tagToActivate) {
            if (toggle)
                TriggerState ^= true;
            else
                TriggerState = true;
        }
    }

    private IEnumerator OnTriggerExit(Collider other) {
        if (definitiveActivation || toggle) yield break;
        if (other.tag == tagToActivate) {
            yield return new WaitForSeconds(delayBeforeDeactivation);
            TriggerState = false;
        }
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;
        
        if (targets == null || targets.Count == 0)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        Gizmos.DrawWireCube(box.center, box.size);
    }

}
