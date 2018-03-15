using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.World
{
    public abstract class RegionInspectorBase : Editor
    {
        protected RegionBase self;

        private SerializedProperty boundsCentreProperty;
        private SerializedProperty boundsSizeProperty;

        private SerializedProperty overrideRenderDistancesProperty;
        private SerializedProperty localRenderDistanceNearProperty;
        private SerializedProperty localRenderDistanceMediumProperty;
        private SerializedProperty localRenderDistanceFarProperty;        

        private SerializedProperty drawBoundsProperty;
        private SerializedProperty boundsColourProperty;

        protected virtual void OnEnable()
        {
            self = target as RegionBase;

            boundsCentreProperty = serializedObject.FindProperty("boundsCentre");
            boundsSizeProperty = serializedObject.FindProperty("boundsSize");

            overrideRenderDistancesProperty = serializedObject.FindProperty("overrideRenderDistances");
            localRenderDistanceNearProperty = serializedObject.FindProperty("localRenderDistanceNear");
            localRenderDistanceMediumProperty = serializedObject.FindProperty("localRenderDistanceMedium");
            localRenderDistanceFarProperty = serializedObject.FindProperty("localRenderDistanceFar");            

            drawBoundsProperty = serializedObject.FindProperty("drawBounds");
            boundsColourProperty = serializedObject.FindProperty("boundsColour");
        }

        public override void OnInspectorGUI()
        {
            //
            List<SubScene> loadedSubScenes = self.GetAllSubScenes();

            //
            base.OnInspectorGUI();

            //###########################################
            EditorGUILayout.LabelField("--Bounds--");

            boundsCentreProperty.vector3Value = EditorGUILayout.Vector3Field("Centre", boundsCentreProperty.vector3Value);
            boundsSizeProperty.vector3Value = EditorGUILayout.Vector3Field("Size", boundsSizeProperty.vector3Value);

            drawBoundsProperty.boolValue = EditorGUILayout.Toggle("Draw Bounds", drawBoundsProperty.boolValue);

            if (drawBoundsProperty.boolValue)
            {
                boundsColourProperty.colorValue = EditorGUILayout.ColorField("Colour", boundsColourProperty.colorValue);
            }

            //###########################################
            EditorGUILayout.LabelField("--Render Distances--");

            overrideRenderDistancesProperty.boolValue = EditorGUILayout.Toggle("Override", overrideRenderDistancesProperty.boolValue);

            if (overrideRenderDistancesProperty.boolValue)
            {
                localRenderDistanceNearProperty.floatValue = EditorGUILayout.FloatField("Near", localRenderDistanceNearProperty.floatValue);
                localRenderDistanceMediumProperty.floatValue = EditorGUILayout.FloatField("Medium", localRenderDistanceMediumProperty.floatValue);
                localRenderDistanceFarProperty.floatValue = EditorGUILayout.FloatField("Far", localRenderDistanceFarProperty.floatValue);
            }

            //###########################################
            GUILayout.Label("--Tools--");

            if (!Application.isPlaying && self.transform.parent.GetComponent<WorldController>().EditorSubScenesLoaded)
            {
                if(GUILayout.Button("Auto-adjust Bounds"))
                {
                    UnityEngine.EventSystems.ExecuteEvents.Execute<IRegionEventHandler>(self.gameObject, null, (x, y) => x.AdjustBounds());
                }
            }

            //###########################################
            GUILayout.Label("--SubScenes--");
            EditorGUILayout.LabelField("Child Count", (self.GetComponentsInChildren<Transform>(true).Length - 1 - loadedSubScenes.Count).ToString());

            if (!Application.isPlaying && self.transform.parent.GetComponent<WorldController>().EditorSubScenesLoaded)
            {
                foreach (var subSceneMode in self.AvailableSubSceneVariants)
                {
                    foreach (var subSceneLayer in Enum.GetValues(typeof(eSubSceneLayer)).Cast<eSubSceneLayer>())
                    {
                        if (!self.GetSubSceneRoot(subSceneLayer, subSceneMode, loadedSubScenes) && GUILayout.Button("Create " + WorldUtility.GetSubSceneRootName(subSceneMode, subSceneLayer)))
                        {
                            UnityEngine.EventSystems.ExecuteEvents.Execute<IRegionEventHandler>(self.gameObject, null, (x, y) => x.CreateSubScene(subSceneMode, subSceneLayer));
                        }
                    }
                }
            }

            //###########################################
            serializedObject.ApplyModifiedProperties();
        }
    }
}