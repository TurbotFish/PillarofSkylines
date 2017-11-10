using UnityEngine;
using UnityEditor;

public class DebugReplacementShaderEditor : EditorWindow {
    
    [MenuItem("Tools/Debug Replacement Shader Mode _F1", false, -10)]
    public static void ChangeDebugShaderMode() {
        Debug.Log("je debug le shader ????");
        var shaderScript = Camera.main.GetComponent<DebugReplacementShading>();

        if (!shaderScript.enabled) {
            shaderScript.enabled = false;
            shaderScript.debugMode = DebugReplacementShading.DebugMode.Checker;
            shaderScript.enabled = true;

        } else if (shaderScript.debugMode == DebugReplacementShading.DebugMode.Checker) {
            shaderScript.enabled = false;
            shaderScript.debugMode = DebugReplacementShading.DebugMode.SlopeCheck;
            shaderScript.enabled = true;
        }
        else
            shaderScript.enabled = false;
        
    }

}
