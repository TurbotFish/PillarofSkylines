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
            string formattedID = idProperty.stringValue.Replace(' ', '_');
            EditorGUILayout.LabelField("Id", formattedID, EditorStyles.wordWrappedLabel);

            EditorGUILayout.LabelField("Owner Type", ownerProperty.objectReferenceValue.GetType().Name);
        }
    }
}