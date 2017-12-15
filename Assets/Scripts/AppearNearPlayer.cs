using UnityEngine;

public class AppearNearPlayer : MonoBehaviour {

    Transform target;
    new Renderer renderer;

    private void OnEnable() {
        target = FindObjectOfType<Game.Player.CharacterController.Character>().transform; // TODO: fix that
        renderer = GetComponent<Renderer>();
    }

    private void Update() {
        renderer.sharedMaterial.SetVector("_PlayerPosition", target.position);
    }
    
}
