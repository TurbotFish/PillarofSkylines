using UnityEngine;

public class PillarEye : MonoBehaviour {

    [SerializeField] Material defaultMaterial;
    [SerializeField] Material eclipseMaterial;
    Renderer rend;
    Transform target;
    [SerializeField] float lookAtDamp = 0.5f;

    private void OnEnable() {
        rend = GetComponent<Renderer>();
        target = FindObjectOfType<Game.Player.CharacterController.CharController>().transform; // TODO: fix that
        Game.Utilities.EventManager.EclipseEvent += OnEclipseEventHandler;
    }

    private void OnDisable() {
        Game.Utilities.EventManager.EclipseEvent -= OnEclipseEventHandler;
    }

    private void Update() {
        //smooth look at
        if (!target)
            return;
        var rotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * lookAtDamp);
    }

    void OnEclipseEventHandler(object sender, Game.Utilities.EventManager.EclipseEventArgs args) {

        if (!rend)
            return;

        if (args.EclipseOn) {
            //eclipse on
            rend.sharedMaterial = eclipseMaterial;

        } else {
            //eclipse off
            rend.sharedMaterial = defaultMaterial;

        }
    }
}
