using UnityEngine;
using UnityEditor;

public class EditorDebugMaster : EditorWindow
{

    [MenuItem("PoS Debug/Debug Replacement Shader Mode _F1", false)]
    public static void ChangeDebugShaderMode()
    {
        var shaderScript = Camera.main.GetComponent<DebugReplacementShading>();

        if (!shaderScript.enabled)
        {
            shaderScript.enabled = false;
            shaderScript.debugMode = DebugReplacementShading.DebugMode.Checker;
            shaderScript.enabled = true;

        }
        else if (shaderScript.debugMode == DebugReplacementShading.DebugMode.Checker)
        {
            shaderScript.enabled = false;
            shaderScript.debugMode = DebugReplacementShading.DebugMode.SlopeCheck;
            shaderScript.enabled = true;
        }
        else
            shaderScript.enabled = false;
    }

    [MenuItem("PoS Debug/Select Camera %#C", false)]
    public static void SelectCamera()
    {
        Transform cam = FindObjectOfType<PoS_Camera>().transform;
        Transform viewPos = SceneView.lastActiveSceneView.camera.transform;

        cam.position = viewPos.position;
        cam.rotation = viewPos.rotation;
    }

    [MenuItem("PoS Debug/Bring Player &F", false)]
    public static void BringPlayer()
    {

        Transform player = FindObjectOfType<Game.Player.CharacterController.CharController>().transform;
        Transform viewPos = SceneView.lastActiveSceneView.camera.transform;
        float nearClipPlane = SceneView.lastActiveSceneView.camera.nearClipPlane;

        player.position = viewPos.position + viewPos.forward * nearClipPlane * 100f;
        RaycastHit hit;
        if (Physics.Raycast(player.position - viewPos.forward, viewPos.forward, out hit, nearClipPlane * 10f, FindObjectOfType < Game.Player.CharacterController.CharacControllerRecu>().collisionMask))
        {
            player.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(viewPos.forward, hit.normal), hit.normal);
            player.position = player.position + hit.normal * 1f;
        } else
        {
            player.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(viewPos.forward, Vector3.up), Vector3.up);
            player.position = player.position + Vector3.up * 1f;
        }
    }

    [MenuItem("PoS Debug/Project Data/Scenes", false)]
    public static void EditSceneData()
    {
        Selection.activeObject = Resources.Load("ScriptableObjects/SceneNamesData");
    }

    [MenuItem("PoS Debug/Project Data/Pillars", false)]
    public static void EditPillarData()
    {
        Selection.activeObject = Resources.Load("ScriptableObjects/PillarData");
    }

    [MenuItem("PoS Debug/Project Data/Abilities", false)]
    public static void EditAbilityData()
    {
        Selection.activeObject = Resources.Load("ScriptableObjects/AbilityData");
    }

    [MenuItem("PoS Debug/Project Data/Chunk System", false)]
    public static void EditChunkSystemData()
    {
        Selection.activeObject = Resources.Load("ScriptableObjects/ChunkSystemData");
    }

    [MenuItem("PoS Debug/Project Data/Character", false)]
    public static void EditCharData()
    {
        Selection.activeObject = Resources.Load("ScriptableObjects/CharData");
    }

}
