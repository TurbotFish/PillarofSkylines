using UnityEngine;

[ExecuteInEditMode]
public class MasterDebug : MonoBehaviour {

    [SerializeField] Player player;
    
	void Start () {
        if (player)
            player.numberOfAerialJumps = 1000;
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
        }
    }
}
