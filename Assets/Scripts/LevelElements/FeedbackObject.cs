using UnityEngine;

public class FeedbackObject : TriggerableObject {

    [SerializeField] Material on;
    [SerializeField] Material off;

    Renderer rend;

    private void Start() {
        rend = GetComponent<Renderer>();
        rend.sharedMaterial = off;
    }

    protected override void Activate() {
        rend.sharedMaterial = on;
    }

    protected override void Deactivate() {
        rend.sharedMaterial = off;
    }
}
