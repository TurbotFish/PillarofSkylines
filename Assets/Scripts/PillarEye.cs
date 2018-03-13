using UnityEngine;

public class PillarEye : MonoBehaviour {

    [SerializeField] GameObject defaultEye;
    [SerializeField] GameObject eclipseEye;
    Transform target;
    [SerializeField] float lookAtDamp = 0.5f;

    private void OnEnable() {
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
        
        if (args.EclipseOn) {
            //eclipse on
            defaultEye.SetActive(false);
            eclipseEye.SetActive(true);

        } else {
            //eclipse off
            defaultEye.SetActive(true);
            eclipseEye.SetActive(false);

        }
    }
}
