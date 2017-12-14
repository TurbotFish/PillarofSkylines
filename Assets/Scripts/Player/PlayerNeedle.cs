using UnityEngine;

public class PlayerNeedle : MonoBehaviour {

    [SerializeField] GameObject[] needles;

    private void OnEnable() {
        Game.Utilities.EventManager.EclipseEvent += OnEclipseEventHandler;
    }

    private void OnDisable() {
        Game.Utilities.EventManager.EclipseEvent -= OnEclipseEventHandler;
    }

    void OnEclipseEventHandler(object sender, Game.Utilities.EventManager.EclipseEventArgs args) {
        foreach(GameObject go in needles)
            go.SetActive(args.EclipseOn);
    }

}
