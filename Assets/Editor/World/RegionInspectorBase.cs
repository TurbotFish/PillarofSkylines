//using EditorCoroutines;
using System;
using System.Collections;
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

        private SerializedProperty doNotDuplicateProperty;

        private int childCount;
        private int rendererCount;
        private int meshColliderCount;
        private int meshColliderAverageVertexCount;
        private int meshColliderHighestVertexCount;
        private string meshColliderLargestMeshName;

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

            GetInfo();
        }

        protected void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //###########################################
            EditorGUILayout.LabelField("-- Bounds", EditorStyles.boldLabel);

            boundsCentreProperty.vector3Value = EditorGUILayout.Vector3Field("Centre", boundsCentreProperty.vector3Value);
            boundsSizeProperty.vector3Value = EditorGUILayout.Vector3Field("Size", boundsSizeProperty.vector3Value);

            drawBoundsProperty.boolValue = EditorGUILayout.Toggle("Draw Bounds", drawBoundsProperty.boolValue);
            boundsColourProperty.colorValue = EditorGUILayout.ColorField("Colour", boundsColourProperty.colorValue);

            //###########################################
            EditorGUILayout.LabelField("-- Render Distances", EditorStyles.boldLabel);

            overrideRenderDistancesProperty.boolValue = EditorGUILayout.Toggle("Override", overrideRenderDistancesProperty.boolValue);

            if (overrideRenderDistancesProperty.boolValue)
            {
                localRenderDistanceNearProperty.floatValue = EditorGUILayout.FloatField("Near", localRenderDistanceNearProperty.floatValue);
                localRenderDistanceMediumProperty.floatValue = EditorGUILayout.FloatField("Medium", localRenderDistanceMediumProperty.floatValue);
                localRenderDistanceFarProperty.floatValue = EditorGUILayout.FloatField("Far", localRenderDistanceFarProperty.floatValue);
            }

            //###########################################
            EditorGUILayout.LabelField("-- Info", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("Child Count", childCount.ToString());
            EditorGUILayout.LabelField("Renderer Count", rendererCount.ToString());

            EditorGUILayout.LabelField("-- Mesh Colliders");
            EditorGUILayout.LabelField("Collider Count", meshColliderCount.ToString());
            EditorGUILayout.LabelField("Average Vertex Count", meshColliderAverageVertexCount.ToString());
            EditorGUILayout.LabelField("Highest Vertex Count", meshColliderHighestVertexCount.ToString() + " (" + meshColliderLargestMeshName + ")");

            EditorGUI.indentLevel--;

            //###########################################
            EditorGUILayout.LabelField("-- Tools", EditorStyles.boldLabel);

            if (!Application.isPlaying && self.transform.parent.GetComponent<WorldController>().EditorSubScenesLoaded)
            {
                if (GUILayout.Button("Auto-adjust Bounds"))
                {
                    self.AdjustBounds();
                }
            }

            //###########################################
            EditorGUILayout.LabelField("-- SubScenes", EditorStyles.boldLabel);

            if (!Application.isPlaying && self.transform.parent.GetComponent<WorldController>().EditorSubScenesLoaded)
            {
                foreach (var subSceneVariant in self.AvailableSubSceneVariants)
                {
                    foreach (var subSceneLayer in Enum.GetValues(typeof(SubSceneLayer)).Cast<SubSceneLayer>())
                    {
                        if (!self.GetSubSceneRoot(subSceneVariant, subSceneLayer) && GUILayout.Button("Create " + WorldUtility.GetSubSceneRootName(subSceneVariant, subSceneLayer)))
                        {
                            CreateSubScene(subSceneVariant, subSceneLayer);
                        }
                    }
                }
            }

            //###########################################
            serializedObject.ApplyModifiedProperties();
        }

        ///// <summary>
        ///// Editor method that adjusts the size and centre of the region bounds to encompass all contained renderers.
        ///// </summary>
        //private void AdjustBounds()
        //{
        //    Bounds bounds = new Bounds();
        //    List<Renderer> renderers = self.GetComponentsInChildren<Renderer>().ToList();
        //    Vector3 position = Vector3.zero;

        //    renderers.RemoveAll(item => item is LineRenderer || item is ParticleSystemRenderer || item is TrailRenderer);      

        //    if (renderers.Count() == 0)
        //    {
        //        boundsCentreProperty.vector3Value = self.transform.position;
        //        boundsSizeProperty.vector3Value = Vector3.zero;
        //    }
        //    else
        //    {
        //        foreach (var renderer in renderers)
        //        {
        //            position += renderer.bounds.center;
        //        }

        //        bounds.center = position / renderers.Count();

        //        foreach (var renderer in renderers)
        //        {
        //            bounds.Encapsulate(renderer.bounds);
        //        }
        //    }

        //    boundsCentreProperty.vector3Value = bounds.center;
        //    boundsSizeProperty.vector3Value = bounds.size;
        //}

        private void GetInfo()
        {
            List<SubScene> loadedSubScenes = self.GetAllSubScenes();
            childCount = self.GetComponentsInChildren<Transform>(true).Length - 1 - loadedSubScenes.Count;
            rendererCount = self.GetComponentsInChildren<Renderer>().Length;

            List<MeshCollider> meshColliders = self.GetComponentsInChildren<MeshCollider>().ToList();
            meshColliderCount = meshColliders.Count;
            int meshColliderTotalVertesCount = 0;
            meshColliderHighestVertexCount = 0;

            foreach (var meshCollider in meshColliders)
            {
                if (meshCollider.sharedMesh == null)
                {
                    continue;
                }

                int vertexCount = meshCollider.sharedMesh.vertexCount;
                meshColliderTotalVertesCount += vertexCount;
                if (vertexCount > meshColliderHighestVertexCount)
                {
                    meshColliderHighestVertexCount = vertexCount;
                    meshColliderLargestMeshName = meshCollider.sharedMesh.name;
                }
            }

            if (meshColliders.Count > 0)
            {
                meshColliderAverageVertexCount = meshColliderTotalVertesCount / meshColliders.Count;
            }
        }

        /// <summary>
        /// Editor method that creates a SubScene object and initializes it.
        /// </summary>
        /// <param name="subSceneVariant"></param>
        /// <param name="subSceneLayer"></param>
        private void CreateSubScene(SubSceneVariant subSceneVariant, SubSceneLayer subSceneLayer)
        {
            string subScenePath = WorldUtility.GetSubScenePath(self.gameObject.scene.path, self.UniqueId, subSceneVariant, subSceneLayer);
            string subScenePathFull = WorldUtility.GetFullPath(subScenePath);

            if (self.GetSubSceneRoot(subSceneVariant, subSceneLayer) != null)
            {
                return;
            }
            else if (System.IO.File.Exists(subScenePathFull))
            {
                Debug.LogFormat("SubScene \"{0}\" already exists but is not loaded!", subScenePath);
                return;
            }
            else
            {
                var rootGO = new GameObject(WorldUtility.GetSubSceneRootName(subSceneVariant, subSceneLayer), typeof(SubScene));
                rootGO.GetComponent<SubScene>().Initialize(subSceneVariant, subSceneLayer);

                var root = rootGO.transform;
                root.SetParent(self.transform, false);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(self.gameObject.scene);
        }
    }
} // end of namespace