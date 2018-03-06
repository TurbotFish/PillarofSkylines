using UnityEditor;

namespace Game.Utilities
{
    [CustomEditor(typeof(UniqueId))]
    public class UniqueIdInspector : Editor
    {
        private SerializedProperty idProperty;
        private SerializedProperty ownerProperty;

        private void OnEnable()
        {
            idProperty = serializedObject.FindProperty("uniqueId");
            ownerProperty = serializedObject.FindProperty("owner");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Id", idProperty.stringValue);

            EditorGUILayout.ObjectField("Owner", ownerProperty.objectReferenceValue, typeof(UniqueIdOwner), false);
        }
    }
}