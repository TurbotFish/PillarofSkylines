using UnityEngine;

[ExecuteInEditMode]
public class MasterDebug : MonoBehaviour {

    [SerializeField] Game.Player.CharacterController.CharController player;
    
	void Start () {
        player = FindObjectOfType<Game.Player.CharacterController.CharController>(); // TODO: fix that
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
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            player.CharData.Jump.MaxAerialJumps = player.CharData.Jump.MaxAerialJumps == 1000 ? 1 : 1000;
        }
        else if (Input.GetKeyDown(KeyCode.F7))
        {
            player.graviswapAvailable = !player.graviswapAvailable;
            print("Graviswap is " + player.graviswapAvailable);
        }
    }

    private void OnDisable()
    {
        if (player && Application.isPlaying)
            player.CharData.Jump.MaxAerialJumps = 1;
    }
}
