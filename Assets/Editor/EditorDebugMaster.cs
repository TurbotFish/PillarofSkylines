using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class EditorDebugMaster : EditorWindow {

    [MenuItem("PoS Debug/Debug Replacement Shader Mode _F1", false)]
    public static void ChangeDebugShaderMode()
    {
        var shaderScript = FindObjectOfType<DebugReplacementShading>();

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

    [MenuItem("GameObject/Select Children %<", false)]
    public static void SelectChildren() {
        if (Selection.gameObjects.Length > 0) {
            List<GameObject> children = new List<GameObject>();
            foreach (GameObject go in Selection.gameObjects) {
                foreach(Transform child in go.transform)
                    children.Add(child.gameObject);

            }
            Selection.objects = children.ToArray();
        }
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

            Vector3 euler = Vector3.zero;
            if (player.eulerAngles.x % 90 > 45)
            {
                euler.x = player.eulerAngles.x + (90 - (player.eulerAngles.x % 90));
            } else
            {
                euler.x = player.eulerAngles.x - (player.eulerAngles.x % 90);
            }
            if (player.eulerAngles.y % 90 > 45)
            {
                euler.y = player.eulerAngles.y + (90 - (player.eulerAngles.y % 90));
            }
            else
            {
                euler.y = player.eulerAngles.y - (player.eulerAngles.y % 90);
            }
            if (player.eulerAngles.z % 90 > 45)
            {
                euler.z = player.eulerAngles.z + (90 - (player.eulerAngles.z % 90));
            }
            else
            {
                euler.z = player.eulerAngles.z - (player.eulerAngles.z % 90);
            }

            player.eulerAngles = euler;

        } else
        {
            player.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(viewPos.forward, Vector3.up), Vector3.up);
            player.position = player.position + Vector3.up * 1f;
        }
    }

    [MenuItem("PoS Debug/Get Scene View in World %#W", false)]
    public static void GetSceneView()
    {
        Debug.Log("get scene view !");
        Transform viewPos = SceneView.lastActiveSceneView.camera.transform;
        Game.World.WorldController worldController = FindObjectOfType<Game.World.WorldController>();


        Debug.Log("world : " + worldController.name);

        Vector3 min, max, size = Vector3.zero;

        Vector3 worldPos = worldController.transform.position;
        Vector3 worldSize = worldController.WorldSize;

        size.x = worldSize.x;
        min.x = worldPos.x - worldSize.x / 2f;
        max.x = worldPos.x + worldSize.x / 2f;

        size.y = worldSize.y;
        min.y = worldPos.y - worldSize.y / 2f;
        max.y = worldPos.y + worldSize.y / 2f;

        size.z = worldSize.z;
        min.z = worldPos.z - worldSize.z / 2f;
        max.z = worldPos.z + worldSize.z / 2f;


        SceneView.lastActiveSceneView.pivot += new Vector3((viewPos.position.x > max.x ? -size.x : 0), (viewPos.position.y > max.y ? -size.y : 0), (viewPos.position.z > max.z ? -size.z : 0));
        
        SceneView.lastActiveSceneView.pivot += new Vector3((viewPos.position.x < min.x ? size.x : 0), (viewPos.position.y < min.y ? size.y : 0), (viewPos.position.z < min.z ? size.z : 0));


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
