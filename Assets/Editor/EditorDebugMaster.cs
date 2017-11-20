using UnityEngine;
using UnityEditor;

public class EditorDebugMaster : EditorWindow {
    
    [MenuItem("PoS Debug/Debug Replacement Shader Mode _F1", false, -10)]
    public static void ChangeDebugShaderMode() {
        if (!Application.isPlaying) {
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

    [MenuItem("PoS Debug/Select Camera #C", false, -10)]
    public static void SelectCamera() {
        Transform cam = FindObjectOfType<PoS_Camera>().transform;
        Transform viewPos = SceneView.lastActiveSceneView.camera.transform;

        cam.position = viewPos.position;
        cam.rotation = viewPos.rotation;
    }
}
