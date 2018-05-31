using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.World
{
    [CustomEditor(typeof(WorldController))]
    public class WorldInspector : Editor
    {
        //###############################################################

        // -- ATTRIBUTES

        private WorldController Self;

        private SerializedProperty WorldSizeProperty;

        private SerializedProperty RenderDistanceNearProperty;
        private SerializedProperty RenderDistanceMediumProperty;
        private SerializedProperty RenderDistanceFarProperty;

        private SerializedProperty DrawBoundsProperty;
        private SerializedProperty DrawRegionBoundsProperty;

        private SerializedProperty ShowRegionModeProperty;
        private SerializedProperty ModeNearColorProperty;
        private SerializedProperty ModeMediumColorProperty;
        private SerializedProperty ModeFarColorProperty;

        //###############################################################

        // -- INITIALIZATION

        private void OnEnable()
        {
            Self = target as WorldController;

            WorldSizeProperty = serializedObject.FindProperty("worldSize");

            RenderDistanceNearProperty = serializedObject.FindProperty("renderDistanceNear");
            RenderDistanceMediumProperty = serializedObject.FindProperty("renderDistanceMedium");
            RenderDistanceFarProperty = serializedObject.FindProperty("renderDistanceFar");

            DrawBoundsProperty = serializedObject.FindProperty("drawBounds");
            DrawRegionBoundsProperty = serializedObject.FindProperty("drawRegionBounds");

            ShowRegionModeProperty = serializedObject.FindProperty("showRegionMode");
            ModeNearColorProperty = serializedObject.FindProperty("modeNearColor");
            ModeMediumColorProperty = serializedObject.FindProperty("modeMediumColor");
            ModeFarColorProperty = serializedObject.FindProperty("modeFarColor");
        }

        //###############################################################

        // -- OPERATIONS

        /// <summary>
        /// Unity method called when the inspector is being drawn.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("----");

            WorldSizeProperty.vector3Value = EditorGUILayout.Vector3Field("World Size", WorldSizeProperty.vector3Value);

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("--Render Distances--");

            RenderDistanceNearProperty.floatValue = EditorGUILayout.FloatField("Near", RenderDistanceNearProperty.floatValue);
            RenderDistanceMediumProperty.floatValue = EditorGUILayout.FloatField("Medium", RenderDistanceMediumProperty.floatValue);
            RenderDistanceFarProperty.floatValue = EditorGUILayout.FloatField("Far", RenderDistanceFarProperty.floatValue);

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("--Bounds - Editor--");

            DrawBoundsProperty.boolValue = EditorGUILayout.Toggle("Draw Bounds", DrawBoundsProperty.boolValue);

            bool drawRegion = DrawRegionBoundsProperty.boolValue;
            DrawRegionBoundsProperty.boolValue = EditorGUILayout.Toggle("Draw Region Bounds", DrawRegionBoundsProperty.boolValue);
            if (drawRegion != DrawRegionBoundsProperty.boolValue)
            {
                foreach (Transform child in Self.transform)
                {
                    var region = child.GetComponent<RegionBase>();

                    if (region)
                    {
                        region.SetDrawBounds(DrawRegionBoundsProperty.boolValue);

                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(Self.gameObject.scene);
                    }
                }
            }

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("--Bounds - Play--");
            EditorGUILayout.LabelField("  [playmode - scene window]");
            EditorGUILayout.LabelField("  Colors the regions according to their current mode.");

            ShowRegionModeProperty.boolValue = EditorGUILayout.Toggle("Show Region Modes", ShowRegionModeProperty.boolValue);

            ModeNearColorProperty.colorValue = EditorGUILayout.ColorField("Mode Near", ModeNearColorProperty.colorValue);
            ModeMediumColorProperty.colorValue = EditorGUILayout.ColorField("Mode Medium", ModeMediumColorProperty.colorValue);
            ModeFarColorProperty.colorValue = EditorGUILayout.ColorField("Mode Far", ModeFarColorProperty.colorValue);

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("-- Tools");

            if (!Application.isPlaying)
            {
                if (Self.EditorSubScenesLoaded)
                {
                    if (GUILayout.Button("Export SubScenes"))
                    {
                        ExportSubScenes(true);
                    }

                    if (GUILayout.Button("Export SubScenes - no baking"))
                    {
                        ExportSubScenes(false);
                    }

                    if (GUILayout.Button("Clear SubScene Folder"))
                    {
                        CleanSubSceneFolder();
                    }

                    if (GUILayout.Button("Auto-adjust All Region Bounds"))
                    {
                        AutoAdjustAllBounds();
                    }
                }
                else
                {
                    if (GUILayout.Button("Import SubScenes"))
                    {
                        ImportSubScenes();
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Makes all child regions adjust their bounds.
        /// </summary>
        private void AutoAdjustAllBounds()
        {
            for (int child_index = 0; child_index < Self.transform.childCount; child_index++)
            {
                var region = Self.transform.GetChild(child_index).GetComponent<RegionBase>();

                if (region != null)
                {
                    region.AdjustBounds();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CleanSubSceneFolder()
        {
            if (!Self.EditorSubScenesLoaded)
            {
                return;
            }

            string sub_scene_folder_path = WorldUtility.GetSubSceneFolderPath(Self.gameObject.scene.path);

            /*
             * cleaning build settings
             */
            var build_settings_scenes = EditorBuildSettings.scenes.ToList();
            var scenes_to_remove = new List<EditorBuildSettingsScene>();

            foreach (var scene_entry in build_settings_scenes)
            {
                if (scene_entry.path.Contains(sub_scene_folder_path) || string.IsNullOrEmpty(scene_entry.path))
                {
                    scenes_to_remove.Add(scene_entry);
                }
            }

            foreach (var scene_entry in scenes_to_remove)
            {
                build_settings_scenes.Remove(scene_entry);
            }

            EditorBuildSettings.scenes = build_settings_scenes.ToArray();

            /*
             * deleting subScene folder (of the current scene only)
             */
            FileUtil.DeleteFileOrDirectory(sub_scene_folder_path); // That's the folder with the subScenes.
            FileUtil.DeleteFileOrDirectory(sub_scene_folder_path.Remove(sub_scene_folder_path.LastIndexOf('_'))); // That's the folder Unity puts the occlusion data into.
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ImportSubScenes()
        {
            if (Self.EditorSubScenesLoaded)
            {
                return;
            }

            EditorUtility.DisplayProgressBar("Importing SubScenes", "", 0);

            var regions = new List<RegionBase>();
            for (int i = 0; i < Self.transform.childCount; i++)
            {
                var region = Self.transform.GetChild(i).GetComponent<RegionBase>();

                if (region)
                {
                    regions.Add(region);
                }
            }

            foreach (var region in regions)
            {
                foreach (var sub_scene_layer in Enum.GetValues(typeof(SubSceneLayer)).Cast<SubSceneLayer>())
                {
                    foreach (var sub_scene_mode in region.AvailableSubSceneVariants)
                    {
                        if (region.GetSubSceneRoot(sub_scene_mode, sub_scene_layer) != null)
                        {
                            continue;
                        }

                        string sub_scene_path = WorldUtility.GetSubScenePath(Self.gameObject.scene.path, region.UniqueId, sub_scene_mode, sub_scene_layer);
                        string sub_scene_path_full = WorldUtility.GetFullPath(sub_scene_path);

                        Scene sub_scene = new Scene();

                        if (System.IO.File.Exists(sub_scene_path_full))
                        {
                            sub_scene = EditorSceneManager.OpenScene(sub_scene_path, OpenSceneMode.Additive);
                        }

                        if (sub_scene.IsValid())
                        {
                            var root_game_object = sub_scene.GetRootGameObjects()[0];
                            EditorSceneManager.MoveGameObjectToScene(root_game_object, Self.gameObject.scene);

                            var root_transform = root_game_object.transform;
                            root_transform.SetParent(region.transform, true);

                            if (!root_transform.gameObject.activeSelf)
                            {
                                root_transform.gameObject.SetActive(true);
                            }
                        }

                        EditorSceneManager.CloseScene(sub_scene, true);
                    }
                }
            }

            Self.EditorSubScenesLoaded = true;

            CleanSubSceneFolder();

            EditorUtility.SetDirty(this);
            EditorSceneManager.MarkSceneDirty(Self.gameObject.scene);

            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="do_baking"></param>
        private void ExportSubScenes(bool do_baking)
        {
            if (!Self.EditorSubScenesLoaded)
            {
                return;
            }

            List<RegionBase> region_list = new List<RegionBase>();
            var scenes = new Dictionary<string, Scene>();
            var build_settings_scenes = EditorBuildSettings.scenes.ToList();

            //++++++++++++++++
            EditorUtility.DisplayProgressBar("Exporting SubScenes", "cleaning", 0);
            var stop_watch = Stopwatch.StartNew();
            //++++++++++++++++

            CleanSubSceneFolder();

            string occlusion_folder_path = Application.dataPath;
            occlusion_folder_path = occlusion_folder_path.Remove(occlusion_folder_path.LastIndexOf('A')); //removes the "Assets" part at the end
            occlusion_folder_path += "Library/Occlusion";
            FileUtil.DeleteFileOrDirectory(occlusion_folder_path);

            //++++++++++++++++
            stop_watch.Stop();
            UnityEngine.Debug.LogFormat("Exporting SubScenes: cleaning: {0}", stop_watch.Elapsed.ToString());
            EditorUtility.DisplayProgressBar("Exporting SubScenes", "baking", 0);
            stop_watch.Restart();
            //++++++++++++++++

            if (do_baking)
            {
                StaticOcclusionCulling.Compute();
            }

            //++++++++++++++++
            stop_watch.Stop();
            UnityEngine.Debug.LogFormat("Exporting SubScenes: baking: {0}", stop_watch.Elapsed.ToString());
            EditorUtility.DisplayProgressBar("Exporting SubScenes", "creating scenes", 0);
            stop_watch.Restart();
            //++++++++++++++++

            for (int i = 0; i < Self.transform.childCount; i++)
            {
                var region = Self.transform.GetChild(i).GetComponent<RegionBase>();

                if (region != null)
                {
                    region_list.Add(region);
                }
            }

            if (!build_settings_scenes.Exists(item => item.path == Self.gameObject.scene.path))
            {
                build_settings_scenes.Add(new EditorBuildSettingsScene(Self.gameObject.scene.path, true));
            }

            foreach (var region in region_list)
            {
                foreach (var sub_scene in region.GetAllSubScenes())
                {
                    if (sub_scene.transform.childCount == 0)
                    {
                        continue;
                    }

                    SubSceneLayer layer = sub_scene.SubSceneLayer;
                    SubSceneVariant variant = sub_scene.SubSceneVariant;

                    sub_scene.transform.SetParent(null, true);
                    var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                    EditorSceneManager.MoveGameObjectToScene(sub_scene.gameObject, scene);

                    string sub_scene_path = WorldUtility.GetSubScenePath(Self.gameObject.scene.path, region.UniqueId, variant, layer);

                    EditorSceneManager.SaveScene(scene, sub_scene_path);
                    scenes.Add(sub_scene_path, scene);

                    build_settings_scenes.Add(new EditorBuildSettingsScene(sub_scene_path, true));

                    EditorSceneManager.CloseScene(scene, true);
                }
            }

            //++++++++++++++++
            stop_watch.Stop();
            UnityEngine.Debug.LogFormat("Exporting SubScenes: creating scenes: {0}", stop_watch.Elapsed.ToString());
            EditorUtility.DisplayProgressBar("Exporting SubScenes", "finishing", 0);
            //++++++++++++++++

            EditorBuildSettings.scenes = build_settings_scenes.ToArray();

            Self.EditorSubScenesLoaded = false;

            EditorUtility.SetDirty(Self);
            EditorSceneManager.MarkSceneDirty(Self.gameObject.scene);

            EditorUtility.ClearProgressBar();
        }
    }
} //end of namespace