using UnityEditor;
using UnityEngine;

namespace Game.World
{
    [CustomEditor(typeof(WorldController))]
    public class WorldInspector : Editor
    {
        private WorldController self;

        private SerializedProperty worldSizeProperty;

        private SerializedProperty renderDistanceNearProperty;
        private SerializedProperty renderDistanceAlwaysProperty;
        private SerializedProperty renderDistanceFarProperty;

        private SerializedProperty preTeleportOffsetProperty;
        private SerializedProperty secondaryPositionDistanceModifierProperty;

        private SerializedProperty drawBoundsProperty;
        private SerializedProperty drawRegionBoundsProperty;
        private SerializedProperty subScenesLoaded;

        private SerializedProperty showRegionModeProperty;
        private SerializedProperty modeNearColorProperty;
        private SerializedProperty modeAlwaysColorProperty;
        private SerializedProperty modeFarColorProperty;

        private void OnEnable()
        {
            self = target as WorldController;

            worldSizeProperty = serializedObject.FindProperty("worldSize");

            renderDistanceNearProperty = serializedObject.FindProperty("renderDistanceNear");
            renderDistanceAlwaysProperty = serializedObject.FindProperty("renderDistanceAlways");
            renderDistanceFarProperty = serializedObject.FindProperty("renderDistanceFar");

            preTeleportOffsetProperty = serializedObject.FindProperty("preTeleportOffset");
            secondaryPositionDistanceModifierProperty = serializedObject.FindProperty("secondaryPositionDistanceModifier");

            drawBoundsProperty = serializedObject.FindProperty("drawBounds");
            drawRegionBoundsProperty = serializedObject.FindProperty("drawRegionBounds");
            subScenesLoaded = serializedObject.FindProperty("editorSubScenesLoaded");

            showRegionModeProperty = serializedObject.FindProperty("showRegionMode");
            modeNearColorProperty = serializedObject.FindProperty("modeNearColor");
            modeAlwaysColorProperty = serializedObject.FindProperty("modeAlwaysColor");
            modeFarColorProperty = serializedObject.FindProperty("modeFarColor");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("----");

            worldSizeProperty.vector3Value = EditorGUILayout.Vector3Field("World Size", worldSizeProperty.vector3Value);

            EditorGUILayout.LabelField("--Render Distances--");

            renderDistanceNearProperty.floatValue = EditorGUILayout.FloatField("Near", renderDistanceNearProperty.floatValue);
            renderDistanceAlwaysProperty.floatValue = EditorGUILayout.FloatField("Always", renderDistanceAlwaysProperty.floatValue);
            renderDistanceFarProperty.floatValue = EditorGUILayout.FloatField("Far", renderDistanceFarProperty.floatValue);

            preTeleportOffsetProperty.floatValue = EditorGUILayout.FloatField("PreTeleportOffset", preTeleportOffsetProperty.floatValue);
            secondaryPositionDistanceModifierProperty.floatValue = EditorGUILayout.FloatField("SecondaryPositionDistanceModifier", secondaryPositionDistanceModifierProperty.floatValue);

            EditorGUILayout.LabelField("--Bounds - Editor--");

            drawBoundsProperty.boolValue = EditorGUILayout.Toggle("Draw Bounds", drawBoundsProperty.boolValue);

            bool drawRegion = drawRegionBoundsProperty.boolValue;
            drawRegionBoundsProperty.boolValue = EditorGUILayout.Toggle("Draw Region Bounds", drawRegionBoundsProperty.boolValue);
            if (drawRegion != drawRegionBoundsProperty.boolValue)
            {
                foreach (Transform child in self.transform)
                {
                    var region = child.GetComponent<RegionBase>();

                    if (region)
                    {
                        UnityEngine.EventSystems.ExecuteEvents.Execute<IRegionEventHandler>(region.gameObject, null, (x, y) => x.SetDrawBounds(drawRegionBoundsProperty.boolValue));
                    }
                }
            }

            EditorGUILayout.LabelField("--Bounds - Play--");
            EditorGUILayout.LabelField("  [playmode - scene window]");
            EditorGUILayout.LabelField("  Colors the regions according to their current mode.");

            showRegionModeProperty.boolValue = EditorGUILayout.Toggle("Show Region Modes", showRegionModeProperty.boolValue);

            if (showRegionModeProperty.boolValue)
            {
                modeNearColorProperty.colorValue = EditorGUILayout.ColorField("Mode Near", modeNearColorProperty.colorValue);
                modeAlwaysColorProperty.colorValue = EditorGUILayout.ColorField("Mode Always", modeAlwaysColorProperty.colorValue);
                modeFarColorProperty.colorValue = EditorGUILayout.ColorField("Mode Far", modeFarColorProperty.colorValue);
            }

            if (!Application.isPlaying)
            {
                GUILayout.Label("--Tools--");

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