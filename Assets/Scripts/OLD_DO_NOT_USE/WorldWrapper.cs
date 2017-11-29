//using UnityEngine;
//using System.Linq;
//using System.Collections.Generic;

//public class WorldWrapper : MonoBehaviour {
    
//    public Vector3 worldSize;
//    public Bool3 repeatAxes;
//    public int numberOfRepetitions;
    
//    List<Renderer> renderers;

//    void Start() {
//        if (repeatAxes.x || repeatAxes.y || repeatAxes.z)
//            InstantiateCopies();
//    }
    
//    void InstantiateCopies() {
        
//        Transform world = Instantiate(transform);
//        Vector3 worldPos = world.position;
//        Vector3 pos = worldPos;
//        RemoveUselessComponents(world);

//        int copiesOnFullAxe = numberOfRepetitions * 2 + 1;
//        Vector3 copiesPerAxe = new Vector3( (repeatAxes.x ? copiesOnFullAxe : 1),
//                                            (repeatAxes.y ? copiesOnFullAxe : 1), 
//                                            (repeatAxes.z ? copiesOnFullAxe : 1));

//        GameObject cloneMaster = new GameObject();
//        cloneMaster.name = "WorldRepetition";
//        Transform cloneParent = cloneMaster.transform;
        
//        for (int xIndex = 0; xIndex < copiesPerAxe.x; xIndex++) {
//            if (repeatAxes.x) pos.x = worldPos.x + (xIndex - numberOfRepetitions) * worldSize.x;

//            for (int yIndex = 0; yIndex < copiesPerAxe.y; yIndex++) {
//                if (repeatAxes.y) pos.y = worldPos.y + (yIndex - numberOfRepetitions) * worldSize.y;

//                for (int zIndex = 0; zIndex < copiesPerAxe.z; zIndex++) {
//                    if (repeatAxes.z) pos.z = worldPos.z + (zIndex - numberOfRepetitions) * worldSize.z;

//                    // Skip the center instance since it's the original world
//                    if (pos == worldPos) continue;
//                    Instantiate(world, pos, Quaternion.identity, cloneParent);
//                }
//            }
//        }

//        Destroy(world.gameObject);
//    }


//    System.Type[] typesToCopy = {
//        typeof(Transform),
//        typeof(MeshFilter), typeof(MeshRenderer), typeof(SkinnedMeshRenderer),
//        typeof(ParticleSystem), typeof(ParticleSystemRenderer)
//    };
//    void RemoveUselessComponents(Transform transform) {
//        Component[] components = transform.GetComponentsInChildren<Component>();
//        for (int i = 0; i < components.Length; i++) {
//            Component comp = components[i];
//            if (!typesToCopy.Contains(comp.GetType()))
//                DestroyImmediate(comp);
//            else if (comp.GetType().BaseType == typeof(Renderer))
//                comp.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
//        }
//    }

//    #region Gizmo

//    [Header("Gizmo"), SerializeField]
//    bool drawGizmo;
//    [SerializeField]
//    Color gizmoColor;

//    void OnDrawGizmos() {
//        if (drawGizmo) {
//            Gizmos.color = gizmoColor;
//            Gizmos.DrawCube(transform.position, worldSize);
//        }
//    }
//    #endregion
//}
