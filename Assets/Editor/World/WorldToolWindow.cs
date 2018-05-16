//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using UnityEditor;
//using UnityEditor.SceneManagement;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//namespace Game.World
//{
//    public class WorldToolWindow : EditorWindow
//    {
//        //========================================================================================

//        private WorldController worldController;

//        private bool working;

//        //========================================================================================

//        #region public methods

//        public static void ShowWindow(WorldController worldController, SerializedObject serializedObject)
//        {
//            var window = EditorWindow.GetWindowWithRect<WorldToolWindow>(ComputeBounds(), true, "World Tool Window");
//            window.Initialize(worldController, serializedObject);
//            window.Show();
//        }

//        #endregion public methods

//        //========================================================================================

//        #region unity methods

//        private void OnGUI()
//        {
//            using (new EditorGUI.DisabledGroupScope(working))
//            {
//                if (worldController.EditorSubScenesLoaded)
//                {
//                    if (GUILayout.Button("Export SubScenes"))
//                    {
//                        working = true;
//                        ExportSubScenes(true);
//                        working = false;
//                    }

//                    if (GUILayout.Button("Export SubScenes - no baking"))
//                    {
//                        working = true;
//                        ExportSubScenes(false);
//                        working = false;
//                    }

//                    if (GUILayout.Button("Clear SubScene Folder"))
//                    {
//                        working = true;
//                        CleanSubSceneFolder();
//                        working = false;
//                    }
//                }
//                else
//                {
//                    if (GUILayout.Button("Import SubScenes"))
//                    {
//                        working = true;
//                        ImportSubScenes();
//                        working = false;
//                    }
//                }
//            }
//        }

//        #endregion unity methods

//        //========================================================================================

//        #region private methods

//        private void Initialize(WorldController worldController, SerializedObject serializedObject)
//        {
//            this.worldController = worldController;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        private void CleanSubSceneFolder()
//        {
//            if (!worldController.EditorSubScenesLoaded)
//            {
//                return;
//            }

//            string subSceneFolderPath = WorldUtility.GetSubSceneFolderPath(worldController.gameObject.scene.path);

//            //cleaning build settings
//            var scenes = EditorBuildSettings.scenes.ToList();
//            var scenesToRemove = new List<EditorBuildSettingsScene>();

//            foreach (var sceneEntry in scenes)
//            {
//                if (sceneEntry.path.Contains(subSceneFolderPath) || string.IsNullOrEmpty(sceneEntry.path))
//                {
//                    scenesToRemove.Add(sceneEntry);
//                }
//            }

//            foreach (var sceneEntry in scenesToRemove)
//            {
//                scenes.Remove(sceneEntry);
//            }

//            EditorBuildSettings.scenes = scenes.ToArray();

//            //deleting subScene folder (of the current scene only)
//            FileUtil.DeleteFileOrDirectory(subSceneFolderPath); //that's the folder with the subScenes
//            FileUtil.DeleteFileOrDirectory(subSceneFolderPath.Remove(subSceneFolderPath.LastIndexOf('_'))); //that's the folder Unity puts the occlusion data into
//            AssetDatabase.Refresh();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        private void ImportSubScenes()
//        {
//            //Debug.Log("ImportSubScenes: called");
//            if (worldController.EditorSubScenesLoaded)
//            {
//                return;
//            }

//            EditorUtility.DisplayProgressBar("Importing SubScenes", "", 0);

//            var regions = new List<RegionBase>();
//            for (int i = 0; i < worldController.transform.childCount; i++)
//            {
//                var region = worldController.transform.GetChild(i).GetComponent<RegionBase>();

//                if (region)
//                {
//                    regions.Add(region);
//                }
//            }
//            //Debug.LogFormat("ImportSubScenes: {0} regions found", regions.Count);

//            foreach (var region in regions)
//            {
//                foreach (var subSceneLayer in Enum.GetValues(typeof(eSubSceneLayer)).Cast<eSubSceneLayer>())
//                {
//                    foreach (var subSceneMode in region.AvailableSubSceneVariants)
//                    {
//                        if (region.GetSubSceneRoot(subSceneMode, subSceneLayer) != null)
//                        {
//                            //Debug.LogErrorFormat("The \"{0}\" of Region \"{1}\" is already loaded!", WorldUtility.GetSubSceneRootName(subSceneMode, subSceneLayer), region.name);
//                            continue;
//                        }
//                        //Debug.Log("ImportSubScenes: starting to import subScene");

//                        //paths
//                        string subScenePath = WorldUtility.GetSubScenePath(worldController.gameObject.scene.path, region.UniqueId, subSceneMode, subSceneLayer, eSuperRegionType.Centre);
//                        string subScenePathFull = WorldUtility.GetFullPath(subScenePath);

//                        Scene subScene = new Scene();

//                        if (System.IO.File.Exists(subScenePathFull))
//                        {
//                            subScene = EditorSceneManager.OpenScene(subScenePath, OpenSceneMode.Additive);
//                        }
//                        else
//                        {
//                            //Debug.LogFormat("ImportSubScenes: SubScene does not exist: {0}", subScenePath);
//                        }

//                        //move subScene content to open world scene
//                        if (subScene.IsValid())
//                        {
//                            var rootGO = subScene.GetRootGameObjects()[0];
//                            EditorSceneManager.MoveGameObjectToScene(rootGO, worldController.gameObject.scene);

//                            var root = rootGO.transform;
//                            root.SetParent(region.transform, true);

//                            if (!root.gameObject.activeSelf)
//                            {
//                                root.gameObject.SetActive(true);
//                            }
//                        }

//                        //end: close subScene
//                        EditorSceneManager.CloseScene(subScene, true);

//                        //Debug.Log("ImportSubScenes: subScene import finished");
//                    }
//                }
//            }

//            //Debug.Log("ImportSubScenes: aaaa");
//            //subScenesLoaded.boolValue = true;
//            //serializedObject.ApplyModifiedProperties();
//            worldController.EditorSubScenesLoaded = true;

//            //clear subScene folder
//            CleanSubSceneFolder();

//            //mark dirty
//            EditorUtility.SetDirty(this);
//            EditorSceneManager.MarkSceneDirty(worldController.gameObject.scene);

//            EditorUtility.ClearProgressBar();

//            //Debug.LogFormat("ImportSubScenes: all subScenes imported, loaded={0}", worldController.EditorSubScenesLoaded);
//        }

//        /// <summary>
//        /// Coroutine that creates the duplications of the world, moves them to separate scenes and does other stuff.
//        /// </summary>
//        private void ExportSubScenes(bool bakeOcclusion)
//        {
//            if (!worldController.EditorSubScenesLoaded)
//            {
//                return;
//            }

//            List<RegionBase> region_list = new List<RegionBase>();
//            var scenes = new Dictionary<string, Scene>();
//            var buildSettingsScenes = EditorBuildSettings.scenes.ToList();


//            //++++++++++++++++
//            EditorUtility.DisplayProgressBar("Exporting SubScenes", "cleaning", 0);
//            var stopWatch = Stopwatch.StartNew();
//            //++++++++++++++++

//            //clear subScene folder
//            CleanSubSceneFolder();

//            //delete Occlusion folder
//            string occlusionFolderPath = Application.dataPath;
//            occlusionFolderPath = occlusionFolderPath.Remove(occlusionFolderPath.LastIndexOf('A')); //removes the "Assets" part at the end
//            occlusionFolderPath += "Library/Occlusion";
//            FileUtil.DeleteFileOrDirectory(occlusionFolderPath);

//            //++++++++++++++++
//            stopWatch.Stop();
//            UnityEngine.Debug.LogFormat("Exporting SubScenes: cleaning: {0}", stopWatch.Elapsed.ToString());
//            EditorUtility.DisplayProgressBar("Exporting SubScenes", "duplicating the world", 0);
//            stopWatch.Restart();
//            //++++++++++++++++

//            //find the central regions
//            for (int i = 0; i < worldController.transform.childCount; i++)
//            {
//                var region = worldController.transform.GetChild(i).GetComponent<RegionBase>();

//                if (region != null)
//                {
//                    region_list.Add(region);
//                }
//            }

//            //++++++++++++++++
//            stopWatch.Stop();
//            UnityEngine.Debug.LogFormat("Exporting SubScenes: duplicating: {0}", stopWatch.Elapsed.ToString());
//            EditorUtility.DisplayProgressBar("Exporting SubScenes", "occlusion bake", 0);
//            stopWatch.Restart();
//            //++++++++++++++++

//            if (bakeOcclusion)
//            {
//                StaticOcclusionCulling.Compute();
//            }

//            //++++++++++++++++
//            stopWatch.Stop();
//            UnityEngine.Debug.LogFormat("Exporting SubScenes: occlusion: {0}", stopWatch.Elapsed.ToString());
//            EditorUtility.DisplayProgressBar("Exporting SubScenes", "creating scenes", 0);
//            stopWatch.Restart();
//            //++++++++++++++++

//            if (!buildSettingsScenes.Exists(item => item.path == worldController.gameObject.scene.path))
//            {
//                buildSettingsScenes.Add(new EditorBuildSettingsScene(worldController.gameObject.scene.path, true));
//            }

//            foreach (var region in region_list)
//            {
//                foreach (var subScene in region.GetAllSubScenes())
//                {
//                    if (subScene.transform.childCount == 0)
//                    {
//                        continue;
//                    }

//                    eSubSceneLayer layer = subScene.SubSceneLayer;
//                    eSubSceneVariant variant = subScene.SubSceneVariant;

//                    subScene.transform.SetParent(null, true);
//                    var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
//                    EditorSceneManager.MoveGameObjectToScene(subScene.gameObject, scene);

//                    string subScenePath = WorldUtility.GetSubScenePath(worldController.gameObject.scene.path, region.UniqueId, variant, layer, eSuperRegionType.Centre);

//                    EditorSceneManager.SaveScene(scene, subScenePath);
//                    scenes.Add(subScenePath, scene);

//                    buildSettingsScenes.Add(new EditorBuildSettingsScene(subScenePath, true));

//                    EditorSceneManager.CloseScene(scene, true);
//                }
//            }

//            //++++++++++++++++
//            stopWatch.Stop();
//            UnityEngine.Debug.LogFormat("Exporting SubScenes: creating scenes: {0}", stopWatch.Elapsed.ToString());
//            EditorUtility.DisplayProgressBar("Exporting SubScenes", "finishing", 0);
//            //++++++++++++++++

//            EditorBuildSettings.scenes = buildSettingsScenes.ToArray();

//            //subScenesLoaded.boolValue = false;
//            //serializedObject.ApplyModifiedProperties();
//            worldController.EditorSubScenesLoaded = false;

//            EditorUtility.SetDirty(worldController);
//            EditorSceneManager.MarkSceneDirty(worldController.gameObject.scene);

//            EditorUtility.ClearProgressBar();
//        }

//        private static Rect ComputeBounds()
//        {
//            float windowWidth = 400f;
//            float windowHeight = 400f;
//            float centerX = Screen.currentResolution.width * 0.5f;
//            float centerY = Screen.currentResolution.height * 0.5f;
//            return new Rect(centerX - windowWidth * 0.5f, centerY - windowHeight * 0.5f, windowWidth, windowHeight);
//        }

//        #endregion private methods

//        //========================================================================================
//    }
//} //end of namespace