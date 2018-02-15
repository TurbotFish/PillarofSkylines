using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class TriggerReset : Trigger {

    private void Start() {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    [SerializeField]
    string tagToActivate = "Player";
    [SerializeField]
    bool definitiveActivation;
    [SerializeField]
    float delayBeforeDeactivation;

    [SerializeField]
    bool changeMaterial;

    [ConditionalHide("changeMaterial"), SerializeField]
    int materialID = 0;

    [ConditionalHide("changeMaterial"), SerializeField]
    Material on, off;

    [ConditionalHide("changeMaterial"), SerializeField]
    new Renderer renderer;
    
    private void OnTriggerEnter(Collider other) {
        if (other.tag == tagToActivate) {
            /*if (Toggle)
                TriggerState ^= true;
            else*/
                TriggerState = false;

            if (changeMaterial) {
                Material[] sharedMaterialsCopy = renderer.sharedMaterials;
                sharedMaterialsCopy[materialID] = TriggerState ? on : off;
                renderer.sharedMaterials = sharedMaterialsCopy;
            }
        }
    }

    /*private IEnumerator OnTriggerExit(Collider other) {
        if (definitiveActivation || Toggle) yield break;
        if (other.tag == tagToActivate) {
            yield return new WaitForSeconds(delayBeforeDeactivation);
            TriggerState = false;
        }
    }*/

    /*protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;
        
        if (Targets == null || Targets.Count == 0)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        Gizmos.DrawWireCube(box.center, box.size);
    }*/

}
