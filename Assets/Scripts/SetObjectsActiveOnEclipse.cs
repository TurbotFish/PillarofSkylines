using UnityEngine;

public class SetObjectsActiveOnEclipse : MonoBehaviour {

    [SerializeField] bool setActive;
    [SerializeField] GameObject[] targets;

    private void OnEnable() {
        Game.Utilities.EventManager.EclipseEvent += OnEclipseEventHandler;
    }

    private void OnDisable() {
        Game.Utilities.EventManager.EclipseEvent -= OnEclipseEventHandler;
    }

    void OnEclipseEventHandler(object sender, Game.Utilities.EventManager.EclipseEventArgs args) {

        for (int i = 0; i < targets.Length; i++)
            targets[i].SetActive(setActive == args.EclipseOn);
    }
}
