using UnityEngine;

[ExecuteInEditMode]
public class MasterDebug : MonoBehaviour {

    [SerializeField] Game.Player.CharacterController.Character player;
    
	void Start () {
        player = FindObjectOfType<Game.Player.CharacterController.Character>(); // TODO: fix that
	}
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            var shaderScript = Camera.main.GetComponent<DebugReplacementShading>();

            if (!shaderScript.enabled) {
                shaderScript.enabled = false;
                shaderScript.debugMode = DebugReplacementShading.DebugMode.Checker;
                shaderScript.enabled = true;

            } else if (shaderScript.debugMode == DebugReplacementShading.DebugMode.Checker) {
                shaderScript.enabled = false;
                shaderScript.debugMode = DebugReplacementShading.DebugMode.SlopeCheck;
                shaderScript.enabled = true;
            } else
                shaderScript.enabled = false;

        } else if (Input.GetKeyDown(KeyCode.F3)) {
            player.numberOfAerialJumps = player.numberOfAerialJumps == 1000 ? 1 : 1000;
        }
    }
}
