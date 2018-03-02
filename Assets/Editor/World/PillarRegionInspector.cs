using UnityEditor;

namespace Game.World
{
    [CustomEditor(typeof(PillarRegion))]
    public class PillarRegionInspector : RegionInspectorBase
    {
        private SerializedProperty pillarIdProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            pillarIdProperty = serializedObject.FindProperty("pillarId");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("--Pillar--");

            pillarIdProperty.enumValueIndex = (int)(ePillarId)EditorGUILayout.EnumPopup("Pillar Id", (ePillarId)pillarIdProperty.enumValueIndex);

            serializedObject.ApplyModifiedProperties();

            //
            base.OnInspectorGUI();
        }
    }
}