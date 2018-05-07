using UnityEngine;

public class AppearNearPlayer : MonoBehaviour {

    Transform target;
    new Renderer renderer;

    private void OnEnable() {
        target = FindObjectOfType<Game.Player.CharacterController.CharController>().transform; // TODO: fix that
        renderer = GetComponent<Renderer>();
    }

    private void Update() {
        renderer.sharedMaterial.SetVector("_PlayerPosition", target.position);
    }

#if UNITY_EDITOR
    private void OnDisable()
    {
        renderer.sharedMaterial.SetVector("_PlayerPosition", Vector3.zero);
    }
#endif
}
