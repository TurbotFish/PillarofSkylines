using UnityEngine;

public class PillarEye : MonoBehaviour {

    [SerializeField] Material defaultMaterial;
    [SerializeField] Material eclipseMaterial;
    Renderer rend;

    private void OnEnable() {
        rend = GetComponent<Renderer>();
        Game.Utilities.EventManager.EclipseEvent += OnEclipseEventHandler;
    }

    private void OnDisable() {
        Game.Utilities.EventManager.EclipseEvent -= OnEclipseEventHandler;
    }

    void OnEclipseEventHandler(object sender, Game.Utilities.EventManager.EclipseEventArgs args) {

        if (args.EclipseOn) {
            //eclipse on
            rend.sharedMaterial = eclipseMaterial;

        } else {
            //eclipse off
            rend.sharedMaterial = defaultMaterial;

        }
    }
}
