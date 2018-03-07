using UnityEditor;
using UnityEngine;

namespace Game.World
{
    [CustomEditor(typeof(WorldController))]
    public class WorldInspector : Editor
    {
        private WorldController self;

        private SerializedProperty worldSizeProperty;

        private SerializedProperty renderDistanceFarProperty;
        private SerializedProperty renderDistanceInactiveProperty;
        private SerializedProperty preTeleportOffsetProperty;
        private SerializedProperty secondaryPositionDistanceModifierProperty;

        private SerializedProperty drawBoundsProperty;
        private SerializedProperty drawRegionBoundsProperty;
        private SerializedProperty subScenesLoaded;

        private void OnEnable()
        {
            self = target as WorldController;

            worldSizeProperty = serializedObject.FindProperty("worldSize");

            renderDistanceFarProperty = serializedObject.FindProperty("renderDistanceFar");
            renderDistanceInactiveProperty = serializedObject.FindProperty("renderDistanceInactive");
            preTeleportOffsetProperty = serializedObject.FindProperty("preTeleportOffset");
            secondaryPositionDistanceModifierProperty = serializedObject.FindProperty("secondaryPositionDistanceModifier");

            drawBoundsProperty = serializedObject.FindProperty("drawBounds");
            drawRegionBoundsProperty = serializedObject.FindProperty("drawRegionBounds");
            subScenesLoaded = serializedObject.FindProperty("editorSubScenesLoaded");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("----");

            worldSizeProperty.vector3Value = EditorGUILayout.Vector3Field("World Size", worldSizeProperty.vector3Value);

            EditorGUILayout.LabelField("Render Distances");

            renderDistanceFarProperty.floatValue = EditorGUILayout.FloatField("Far", renderDistanceFarProperty.floatValue);
            renderDistanceInactiveProperty.floatValue = EditorGUILayout.FloatField("Inactive", renderDistanceInactiveProperty.floatValue);
            preTeleportOffsetProperty.floatValue = EditorGUILayout.FloatField("PreTeleportOffset", preTeleportOffsetProperty.floatValue);
            secondaryPositionDistanceModifierProperty.floatValue = EditorGUILayout.FloatField("SecondaryPositionDistanceModifier", secondaryPositionDistanceModifierProperty.floatValue);

            EditorGUILayout.LabelField("----");

            drawBoundsProperty.boolValue = EditorGUILayout.Toggle("Draw Bounds", drawBoundsProperty.boolValue);

            bool drawRegion = drawRegionBoundsProperty.boolValue;
            drawRegionBoundsProperty.boolValue = EditorGUILayout.Toggle("Draw Region Bounds", drawRegionBoundsProperty.boolValue);
            if(drawRegion != drawRegionBoundsProperty.boolValue)
            {
                foreach(Transform child in self.transform)
                {
                    var region = child.GetComponent<RegionBase>();

                    if (region)
                    {
                        UnityEngine.EventSystems.ExecuteEvents.Execute<IRegionEventHandler>(region.gameObject, null, (x, y) => x.SetDrawBounds(drawRegionBoundsProperty.boolValue));
                    }
                }
            }

            if (!Application.isPlaying)
            {
                GUILayout.Label("");

                if (subScenesLoaded.boolValue)
                {
                    if (GUILayout.Button("Export SubScenes"))
                    {
                        UnityEngine.EventSystems.ExecuteEvents.Execute<IWorldEventHandler>(self.gameObject, null, (x, y) => x.ExportSubScenes());
                    }

                    if (GUILayout.Button("Clear SubScene Folder"))
                    {
                        UnityEngine.EventSystems.ExecuteEvents.Execute<IWorldEventHandler>(self.gameObject, null, (x, y) => x.ClearSubSceneFolder());
                    }
                }
                else
                {
                    if (GUILayout.Button("Import SubScenes"))
                    {
                        UnityEngine.EventSystems.ExecuteEvents.Execute<IWorldEventHandler>(self.gameObject, null, (x, y) => x.ImportSubScenes());
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}