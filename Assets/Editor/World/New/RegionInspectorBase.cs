using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.World.New
{
    public abstract class RegionInspectorBase : Editor
    {
        protected RegionBase self;

        private SerializedProperty boundsCentreProperty;
        private SerializedProperty boundsSizeProperty;

        private SerializedProperty overrideRenderDistancesProperty;
        private SerializedProperty localRenderDistanceFarProperty;
        private SerializedProperty localRenderDistanceInactiveProperty;

        private SerializedProperty drawBoundsProperty;
        private SerializedProperty boundsColourProperty;

        protected virtual void OnEnable()
        {
            self = target as RegionBase;

            boundsCentreProperty = serializedObject.FindProperty("boundsCentre");
            boundsSizeProperty = serializedObject.FindProperty("boundsSize");

            overrideRenderDistancesProperty = serializedObject.FindProperty("overrideRenderDistances");
            localRenderDistanceFarProperty = serializedObject.FindProperty("localRenderDistanceFar");
            localRenderDistanceInactiveProperty = serializedObject.FindProperty("localRenderDistanceInactive");

            drawBoundsProperty = serializedObject.FindProperty("drawBounds");
            boundsColourProperty = serializedObject.FindProperty("boundsColour");
        }

        public override void OnInspectorGUI()
        {
            //
            List<SubScene> loadedSubScenes = self.GetAllSubScenes();

            //
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("--Bounds--");

            boundsCentreProperty.vector3Value = EditorGUILayout.Vector3Field("Centre", boundsCentreProperty.vector3Value);
            boundsSizeProperty.vector3Value = EditorGUILayout.Vector3Field("Size", boundsSizeProperty.vector3Value);

            drawBoundsProperty.boolValue = EditorGUILayout.Toggle("Draw Bounds", drawBoundsProperty.boolValue);

            if (drawBoundsProperty.boolValue)
            {
                boundsColourProperty.colorValue = EditorGUILayout.ColorField("Colour", boundsColourProperty.colorValue);
            }

            EditorGUILayout.LabelField("--Render Distances--");

            overrideRenderDistancesProperty.boolValue = EditorGUILayout.Toggle("Override", overrideRenderDistancesProperty.boolValue);

            if (overrideRenderDistancesProperty.boolValue)
            {
                localRenderDistanceFarProperty.floatValue = EditorGUILayout.FloatField("Far", localRenderDistanceFarProperty.floatValue);
                localRenderDistanceInactiveProperty.floatValue = EditorGUILayout.FloatField("Inactive", localRenderDistanceInactiveProperty.floatValue);
            }

            if (!Application.isPlaying && self.transform.parent.GetComponent<WorldController>().EditorSubScenesLoaded)
            {
                GUILayout.Label("--Tools--");

                if(GUILayout.Button("Auto-adjust Bounds"))
                {
                    UnityEngine.EventSystems.ExecuteEvents.Execute<IRegionEventHandler>(self.gameObject, null, (x, y) => x.AdjustBounds());
                }

                if (loadedSubScenes.Count != (self.AvailableSubSceneModes.Count * Enum.GetNames(typeof(eSubSceneType)).Count()))
                {
                    GUILayout.Label("--Create SubScenes--");
                }               

                foreach (var subSceneMode in self.AvailableSubSceneModes)
                {
                    foreach (var subSceneType in Enum.GetValues(typeof(eSubSceneType)).Cast<eSubSceneType>())
                    {
                        if (!self.GetSubSceneRoot(subSceneType, subSceneMode, loadedSubScenes) && GUILayout.Button("Create " + WorldUtility.GetSubSceneRootName(subSceneMode, subSceneType)))
                        {
                            UnityEngine.EventSystems.ExecuteEvents.Execute<IRegionEventHandler>(self.gameObject, null, (x, y) => x.CreateSubScene(subSceneMode, subSceneType));
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}