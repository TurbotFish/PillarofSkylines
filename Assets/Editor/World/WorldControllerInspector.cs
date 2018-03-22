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
        private SerializedProperty renderDistanceMediumProperty;
        private SerializedProperty renderDistanceFarProperty;

        private SerializedProperty preTeleportOffsetProperty;
        private SerializedProperty secondaryPositionDistanceModifierProperty;

        private SerializedProperty drawBoundsProperty;
        private SerializedProperty drawRegionBoundsProperty;
        private SerializedProperty subScenesLoaded;

        private SerializedProperty showRegionModeProperty;
        private SerializedProperty modeNearColorProperty;
        private SerializedProperty modeMediumColorProperty;
        private SerializedProperty modeFarColorProperty;

        private SerializedProperty unloadInvisibleRegionsProperty;
        private SerializedProperty invisibilityAngleProperty;

        private SerializedProperty measureTimesProperty;
        private SerializedProperty debugResultCountProperty;

        private void OnEnable()
        {
            self = target as WorldController;

            worldSizeProperty = serializedObject.FindProperty("worldSize");

            renderDistanceNearProperty = serializedObject.FindProperty("renderDistanceNear");
            renderDistanceMediumProperty = serializedObject.FindProperty("renderDistanceMedium");
            renderDistanceFarProperty = serializedObject.FindProperty("renderDistanceFar");

            preTeleportOffsetProperty = serializedObject.FindProperty("preTeleportOffset");
            secondaryPositionDistanceModifierProperty = serializedObject.FindProperty("secondaryPositionDistanceModifier");

            drawBoundsProperty = serializedObject.FindProperty("drawBounds");
            drawRegionBoundsProperty = serializedObject.FindProperty("drawRegionBounds");
            subScenesLoaded = serializedObject.FindProperty("editorSubScenesLoaded");

            showRegionModeProperty = serializedObject.FindProperty("showRegionMode");
            modeNearColorProperty = serializedObject.FindProperty("modeNearColor");
            modeMediumColorProperty = serializedObject.FindProperty("modeMediumColor");
            modeFarColorProperty = serializedObject.FindProperty("modeFarColor");

            unloadInvisibleRegionsProperty = serializedObject.FindProperty("unloadInvisibleRegions");
            invisibilityAngleProperty = serializedObject.FindProperty("invisibilityAngle");

            measureTimesProperty = serializedObject.FindProperty("measureTimes");
            debugResultCountProperty = serializedObject.FindProperty("debugResultCount");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("----");

            worldSizeProperty.vector3Value = EditorGUILayout.Vector3Field("World Size", worldSizeProperty.vector3Value);

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("--Render Distances--");

            renderDistanceNearProperty.floatValue = EditorGUILayout.FloatField("Near", renderDistanceNearProperty.floatValue);
            renderDistanceMediumProperty.floatValue = EditorGUILayout.FloatField("Medium", renderDistanceMediumProperty.floatValue);
            renderDistanceFarProperty.floatValue = EditorGUILayout.FloatField("Far", renderDistanceFarProperty.floatValue);

            preTeleportOffsetProperty.floatValue = EditorGUILayout.FloatField("PreTeleportOffset", preTeleportOffsetProperty.floatValue);
            secondaryPositionDistanceModifierProperty.floatValue = EditorGUILayout.FloatField("SecondaryPositionDistanceModifier", secondaryPositionDistanceModifierProperty.floatValue);

            EditorGUILayout.LabelField("");
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

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("--Bounds - Play--");
            EditorGUILayout.LabelField("  [playmode - scene window]");
            EditorGUILayout.LabelField("  Colors the regions according to their current mode.");

            showRegionModeProperty.boolValue = EditorGUILayout.Toggle("Show Region Modes", showRegionModeProperty.boolValue);

            modeNearColorProperty.colorValue = EditorGUILayout.ColorField("Mode Near", modeNearColorProperty.colorValue);
            modeMediumColorProperty.colorValue = EditorGUILayout.ColorField("Mode Medium", modeMediumColorProperty.colorValue);
            modeFarColorProperty.colorValue = EditorGUILayout.ColorField("Mode Far", modeFarColorProperty.colorValue);

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("-- Culling");

            unloadInvisibleRegionsProperty.boolValue = EditorGUILayout.Toggle("Unload Regions?", unloadInvisibleRegionsProperty.boolValue);
            invisibilityAngleProperty.floatValue = EditorGUILayout.FloatField("Angle", invisibilityAngleProperty.floatValue);

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("-- Debug");

            measureTimesProperty.boolValue = EditorGUILayout.Toggle("Measure Times", measureTimesProperty.boolValue);
            debugResultCountProperty.intValue = EditorGUILayout.IntField("Result Count", debugResultCountProperty.intValue);

            if (!Application.isPlaying)
            {
                EditorGUILayout.LabelField("");
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