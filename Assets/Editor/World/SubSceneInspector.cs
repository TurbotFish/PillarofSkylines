using System.Collections;
using System.Collections.Generic;
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

        private void OnEnable()
        {
            self = target as SubScene;

            subSceneVariantProperty = serializedObject.FindProperty("subSceneVariant");
            subSceneLayerProperty = serializedObject.FindProperty("subSceneLayer");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Variant", subSceneVariantProperty.enumDisplayNames[subSceneVariantProperty.enumValueIndex]);
            EditorGUILayout.LabelField("Layer", subSceneLayerProperty.enumDisplayNames[subSceneLayerProperty.enumValueIndex]);

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Child Count", (self.GetComponentsInChildren<Transform>(true).Length - 1).ToString());

            EditorGUILayout.LabelField("");
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
    }
} //end of namespace