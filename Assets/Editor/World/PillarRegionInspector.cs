using Game.Model;
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
            //
            base.OnInspectorGUI();

            //
            EditorGUILayout.LabelField("-- Pillar");

            pillarIdProperty.enumValueIndex = (int)(PillarId)EditorGUILayout.EnumPopup("Pillar Id", (PillarId)pillarIdProperty.enumValueIndex);

            serializedObject.ApplyModifiedProperties();
        }
    }
}