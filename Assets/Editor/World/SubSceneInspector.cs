//using EditorCoroutines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.World
{
    [CustomEditor(typeof(SubScene))]
    public class SubSceneInspector : Editor
    {
        private SubScene self;

        private SerializedProperty subSceneVariantProperty;
        private SerializedProperty subSceneLayerProperty;

        private int childCount;
        private int rendererCount;
        private int meshColliderCount;
        private int meshColliderAverageVertexCount;
        private int meshColliderHighestVertexCount;
        private string meshColliderLargestMeshName;

        private void OnEnable()
        {
            self = target as SubScene;

            subSceneVariantProperty = serializedObject.FindProperty("subSceneVariant");
            subSceneLayerProperty = serializedObject.FindProperty("subSceneLayer");

            GetInfo();
        }

        private void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Variant", subSceneVariantProperty.enumDisplayNames[subSceneVariantProperty.enumValueIndex]);
            EditorGUILayout.LabelField("Layer", subSceneLayerProperty.enumDisplayNames[subSceneLayerProperty.enumValueIndex]);

            EditorGUILayout.LabelField("-- Info", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Child Count", childCount.ToString());
            EditorGUILayout.LabelField("Renderer Count", rendererCount.ToString());
            EditorGUILayout.LabelField("-- Mesh Colliders");
            EditorGUILayout.LabelField("Collider Count", meshColliderCount.ToString());
            EditorGUILayout.LabelField("Average Vertex Count", meshColliderAverageVertexCount.ToString());
            EditorGUILayout.LabelField("Highest Vertex Count", meshColliderHighestVertexCount.ToString() + " (" + meshColliderLargestMeshName + ")");
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("-- Tools", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Use at your own risk - Save Scene before using!");
            if (GUILayout.Button("Clean Up Scales"))
            {
                CleanUpScales();
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(self.gameObject.scene);
            }
        }

        private void CleanUpScales()
        {
            foreach (Transform child in self.transform)
            {
                CleanUpScale(child, 1, 1, 1);
            }
        }

        private void CleanUpScale(Transform transform, float multiplicationX, float multiplicationY, float multiplicationZ)
        {
            var localScale = transform.localScale;
            var localPosition = transform.localPosition;

            transform.localPosition = new Vector3(localPosition.x * multiplicationX, localPosition.y * multiplicationY, localPosition.z * multiplicationZ);

            multiplicationX *= localScale.x;
            multiplicationY *= localScale.y;
            multiplicationZ *= localScale.z;

            if (transform.GetComponent<Renderer>() == null)
            {
                transform.localScale = new Vector3(1, 1, 1);

                foreach (Transform child in transform)
                {
                    CleanUpScale(child, multiplicationX, multiplicationY, multiplicationZ);
                }

                BoxCollider boxCollider = transform.GetComponent<BoxCollider>();
                if (boxCollider != null)
                {
                    var centre = boxCollider.center;
                    centre += localPosition - transform.localPosition;
                    boxCollider.center = new Vector3(centre.x * multiplicationX, centre.y * multiplicationY, centre.z * multiplicationZ);

                    var size = boxCollider.size;
                    boxCollider.size = new Vector3(size.x * multiplicationX, size.y * multiplicationY, size.z * multiplicationZ);
                }
            }
            else
            {
                transform.localScale = new Vector3(multiplicationX, multiplicationY, multiplicationZ);
            }
        }

        private void GetInfo()
        {
            childCount = self.GetComponentsInChildren<Transform>(true).Length - 1;
            rendererCount = self.GetComponentsInChildren<Renderer>().Length;

            List<MeshCollider> meshColliders = self.GetComponentsInChildren<MeshCollider>().ToList();
            meshColliderCount = meshColliders.Count;
            int meshColliderTotalVertesCount = 0;
            meshColliderHighestVertexCount = 0;

            foreach (var meshCollider in meshColliders)
            {
                if (meshCollider == null || meshCollider.sharedMesh == null)
                {
                    continue;
                }

                int vertexCount = meshCollider.sharedMesh.vertexCount;
                meshColliderTotalVertesCount += vertexCount;
                if (vertexCount > meshColliderHighestVertexCount)
                {
                    meshColliderHighestVertexCount = vertexCount;
                    meshColliderLargestMeshName = meshCollider.sharedMesh.name;
                }
            }

            if (meshColliders.Count > 0)
            {
                meshColliderAverageVertexCount = meshColliderTotalVertesCount / meshColliders.Count;
            }
        }
    }
} // end of namespace