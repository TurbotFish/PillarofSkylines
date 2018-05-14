using UnityEditor;
using UnityEngine;

namespace Game.World
{
    [CustomEditor(typeof(WorldController))]
    public class WorldInspector : Editor
    {
        //========================================================================================

        #region member variables

        private WorldController self;

        private SerializedProperty worldSizeProperty;

        private SerializedProperty renderDistanceNearProperty;
        private SerializedProperty renderDistanceMediumProperty;
        private SerializedProperty renderDistanceFarProperty;

        private SerializedProperty preTeleportOffsetProperty;
        private SerializedProperty secondaryPositionDistanceModifierProperty;

        private SerializedProperty drawBoundsProperty;
        private SerializedProperty drawRegionBoundsProperty;

        private SerializedProperty showRegionModeProperty;
        private SerializedProperty modeNearColorProperty;
        private SerializedProperty modeMediumColorProperty;
        private SerializedProperty modeFarColorProperty;

        private SerializedProperty unloadInvisibleRegionsProperty;
        private SerializedProperty invisibilityAngleProperty;

        private SerializedProperty debugResultCountProperty;

        #endregion member variables

        //========================================================================================

        #region initialization

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

            showRegionModeProperty = serializedObject.FindProperty("showRegionMode");
            modeNearColorProperty = serializedObject.FindProperty("modeNearColor");
            modeMediumColorProperty = serializedObject.FindProperty("modeMediumColor");
            modeFarColorProperty = serializedObject.FindProperty("modeFarColor");

            unloadInvisibleRegionsProperty = serializedObject.FindProperty("unloadInvisibleRegions");
            invisibilityAngleProperty = serializedObject.FindProperty("invisibilityAngle");

            debugResultCountProperty = serializedObject.FindProperty("debugResultCount");
        }

        #endregion initialization

        //========================================================================================

        #region update

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                if (GUILayout.Button("Open Tool Window"))
                {
                    WorldToolWindow.ShowWindow(self, serializedObject);
                }
            }

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
                        region.SetDrawBounds(drawRegionBoundsProperty.boolValue);

                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(self.gameObject.scene);
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

            debugResultCountProperty.intValue = EditorGUILayout.IntField("Result Count", debugResultCountProperty.intValue);

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("-- Tools");

            if (!Application.isPlaying && self.EditorSubScenesLoaded)
            {
                if (GUILayout.Button("Auto-adjust Bounds"))
                {
                    AutoAdjustAllBounds();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void AutoAdjustAllBounds()
        {
            for(int child_index = 0; child_index < self.transform.childCount; child_index++)
            {
                var region = self.transform.GetChild(child_index).GetComponent<RegionBase>();

                if(region != null)
                {
                    region.AdjustBounds();
                }
            }
        }

        #endregion update      

        //========================================================================================
    }
} //end of namespace