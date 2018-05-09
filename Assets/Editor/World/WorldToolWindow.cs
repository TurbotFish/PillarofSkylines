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
    public class WorldToolWindow : EditorWindow
    {
        //========================================================================================

        private WorldController worldController;
        private SerializedObject serializedObject;

        private SerializedProperty subScenesLoaded;

        private bool working;

        //========================================================================================

        #region public methods

        public static void ShowWindow(WorldController worldController, SerializedObject serializedObject)
        {
            var window = EditorWindow.GetWindowWithRect<WorldToolWindow>(ComputeBounds(), true, "World Tool Window");
            window.Initialize(worldController, serializedObject);
            window.Show();
        }

        #endregion public methods

        //========================================================================================

        #region unity methods

        private void OnGUI()
        {
            using (new EditorGUI.DisabledGroupScope(working))
            {
                if (worldController.EditorSubScenesLoaded)
                {
                    if (GUILayout.Button("Export SubScenes"))
                    {
                        working = true;
                        ExportSubScenes(true);
                        working = false;
                    }

                    if (GUILayout.Button("Export SubScenes - no baking"))
                    {
                        working = true;
                        ExportSubScenes(false);
                        working = false;
                    }

                    if (GUILayout.Button("Clear SubScene Folder"))
                    {
                        working = true;
                        CleanSubSceneFolder();
                        working = false;
                    }
                }
                else
                {
                    if (GUILayout.Button("Import SubScenes"))
                    {
                        working = true;
                        ImportSubScenes();
                        working = false;
                    }
                }
            }
        }

        #endregion unity methods

        //========================================================================================

        #region private methods

        private void Initialize(WorldController worldController, SerializedObject serializedObject)
        {
            this.worldController = worldController;
            this.serializedObject = serializedObject;

            subScenesLoaded = serializedObject.FindProperty("editorSubScenesLoaded");
        }

        /// <summary>
        /// 
        /// </summary>
        private void CleanSubSceneFolder()
        {
            if (!worldController.EditorSubScenesLoaded)
            {
                return;
            }

            string subSceneFolderPath = WorldUtility.GetSubSceneFolderPath(worldController.gameObject.scene.path);

            //cleaning build settings
            var scenes = EditorBuildSettings.scenes.ToList();
            var scenesToRemove = new List<EditorBuildSettingsScene>();

            foreach (var sceneEntry in scenes)
            {
                if (sceneEntry.path.Contains(subSceneFolderPath) || string.IsNullOrEmpty(sceneEntry.path))
                {
                    scenesToRemove.Add(sceneEntry);
                }
            }

            foreach (var sceneEntry in scenesToRemove)
            {
                scenes.Remove(sceneEntry);
            }

            EditorBuildSettings.scenes = scenes.ToArray();

            //deleting subScene folder (of the current scene only)
            FileUtil.DeleteFileOrDirectory(subSceneFolderPath); //that's the folder with the subScenes
            FileUtil.DeleteFileOrDirectory(subSceneFolderPath.Remove(subSceneFolderPath.LastIndexOf('_'))); //that's the folder Unity puts the occlusion data into
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void ImportSubScenes()
        {
            //Debug.Log("ImportSubScenes: called");
            if (worldController.EditorSubScenesLoaded)
            {
                return;
            }

            EditorUtility.DisplayProgressBar("Importing SubScenes", "", 0);

            var regions = new List<RegionBase>();
            for (int i = 0; i < worldController.transform.childCount; i++)
            {
                var region = worldController.transform.GetChild(i).GetComponent<RegionBase>();

                if (region)
                {
                    regions.Add(region);
                }
            }
            //Debug.LogFormat("ImportSubScenes: {0} regions found", regions.Count);

            foreach (var region in regions)
            {
                foreach (var subSceneLayer in Enum.GetValues(typeof(eSubSceneLayer)).Cast<eSubSceneLayer>())
                {
                    foreach (var subSceneMode in region.AvailableSubSceneVariants)
                    {
                        if (region.GetSubSceneRoot(subSceneLayer, subSceneMode) != null)
                        {
                            //Debug.LogErrorFormat("The \"{0}\" of Region \"{1}\" is already loaded!", WorldUtility.GetSubSceneRootName(subSceneMode, subSceneLayer), region.name);
                            continue;
                        }
                        //Debug.Log("ImportSubScenes: starting to import subScene");

                        //paths
                        string subScenePath = WorldUtility.GetSubScenePath(worldController.gameObject.scene.path, region.UniqueId, subSceneMode, subSceneLayer, eSuperRegionType.Centre);
                        string subScenePathFull = WorldUtility.GetFullPath(subScenePath);

                        Scene subScene = new Scene();

                        if (System.IO.File.Exists(subScenePathFull))
                        {
                            subScene = EditorSceneManager.OpenScene(subScenePath, OpenSceneMode.Additive);
                        }
                        else
                        {
                            //Debug.LogFormat("ImportSubScenes: SubScene does not exist: {0}", subScenePath);
                        }

                        //move subScene content to open world scene
                        if (subScene.IsValid())
                        {
                            var rootGO = subScene.GetRootGameObjects()[0];
                            EditorSceneManager.MoveGameObjectToScene(rootGO, worldController.gameObject.scene);

                            var root = rootGO.transform;
                            root.SetParent(region.transform, true);

                            if (!root.gameObject.activeSelf)
                            {
                                root.gameObject.SetActive(true);
                            }
                        }

                        //end: close subScene
                        EditorSceneManager.CloseScene(subScene, true);

                        //Debug.Log("ImportSubScenes: subScene import finished");
                    }
                }
            }

            //Debug.Log("ImportSubScenes: aaaa");
            //subScenesLoaded.boolValue = true;
            //serializedObject.ApplyModifiedProperties();
            worldController.EditorSubScenesLoaded = true;

            //clear subScene folder
            CleanSubSceneFolder();

            //mark dirty
            EditorUtility.SetDirty(this);
            EditorSceneManager.MarkSceneDirty(worldController.gameObject.scene);

            EditorUtility.ClearProgressBar();

            //Debug.LogFormat("ImportSubScenes: all subScenes imported, loaded={0}", worldController.EditorSubScenesLoaded);
        }

        /// <summary>
        /// Coroutine that creates the duplications of the world, moves them to separate scenes and does other stuff.
        /// </summary>
        /// <returns></returns>
        private void ExportSubScenes(bool bakeOcclusion)
        {
            if (!worldController.EditorSubScenesLoaded)
            {
                return;
            }

            List<RegionBase> region_list = new List<RegionBase>();
            var regionDict = new Dictionary<eSuperRegionType, Dictionary<string, RegionBase>>(); //dictionary<SuperRegion, dictionary<regionId, region>>
            var scenes = new Dictionary<string, Scene>();
            var buildSettingsScenes = EditorBuildSettings.scenes.ToList();


            //++++++++++++++++
            EditorUtility.DisplayProgressBar("Exporting SubScenes", "cleaning", 0);
            var stopWatch = Stopwatch.StartNew();
            //++++++++++++++++

            //clear subScene folder
            CleanSubSceneFolder();

            //delete Occlusion folder
            string occlusionFolderPath = Application.dataPath;
            occlusionFolderPath = occlusionFolderPath.Remove(occlusionFolderPath.LastIndexOf('A')); //removes the "Assets" part at the end
            occlusionFolderPath += "Library/Occlusion";
            FileUtil.DeleteFileOrDirectory(occlusionFolderPath);

            //++++++++++++++++
            stopWatch.Stop();
            UnityEngine.Debug.LogFormat("Exporting SubScenes: cleaning: {0}", stopWatch.Elapsed.ToString());
            EditorUtility.DisplayProgressBar("Exporting SubScenes", "duplicating the world", 0);
            stopWatch.Restart();
            //++++++++++++++++

            //find the central regions
            for (int i = 0; i < worldController.transform.childCount; i++)
            {
                var region = worldController.transform.GetChild(i).GetComponent<RegionBase>();

                if (region != null)
                {
                    region_list.Add(region);
                }
            }

            //++++++++++++++++
            stopWatch.Stop();
            UnityEngine.Debug.LogFormat("Exporting SubScenes: duplicating: {0}", stopWatch.Elapsed.ToString());
            EditorUtility.DisplayProgressBar("Exporting SubScenes", "occlusion bake", 0);
            stopWatch.Restart();
            //++++++++++++++++

            if (bakeOcclusion)
            {
                StaticOcclusionCulling.Compute();
            }

            //++++++++++++++++
            stopWatch.Stop();
            UnityEngine.Debug.LogFormat("Exporting SubScenes: occlusion: {0}", stopWatch.Elapsed.ToString());
            EditorUtility.DisplayProgressBar("Exporting SubScenes", "creating scenes", 0);
            stopWatch.Restart();
            //++++++++++++++++

            if (!buildSettingsScenes.Exists(item => item.path == worldController.gameObject.scene.path))
            {
                buildSettingsScenes.Add(new EditorBuildSettingsScene(worldController.gameObject.scene.path, true));
            }

            foreach (var region in region_list)
            {
                foreach (var subScene in region.GetAllSubScenes())
                {
                    if (subScene.transform.childCount == 0)
                    {
                        continue;
                    }

                    eSubSceneLayer layer = subScene.SubSceneLayer;
                    eSubSceneVariant variant = subScene.SubSceneVariant;

                    subScene.transform.SetParent(null, true);
                    var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                    EditorSceneManager.MoveGameObjectToScene(subScene.gameObject, scene);

                    string subScenePath = WorldUtility.GetSubScenePath(worldController.gameObject.scene.path, region.UniqueId, variant, layer, eSuperRegionType.Centre);

                    EditorSceneManager.SaveScene(scene, subScenePath);
                    scenes.Add(subScenePath, scene);

                    buildSettingsScenes.Add(new EditorBuildSettingsScene(subScenePath, true));

                    EditorSceneManager.CloseScene(scene, true);
                }
            }

            //++++++++++++++++
            stopWatch.Stop();
            UnityEngine.Debug.LogFormat("Exporting SubScenes: creating scenes: {0}", stopWatch.Elapsed.ToString());
            EditorUtility.DisplayProgressBar("Exporting SubScenes", "finishing", 0);
            //++++++++++++++++

            EditorBuildSettings.scenes = buildSettingsScenes.ToArray();

            //subScenesLoaded.boolValue = false;
            //serializedObject.ApplyModifiedProperties();
            worldController.EditorSubScenesLoaded = false;

            EditorUtility.SetDirty(worldController);
            EditorSceneManager.MarkSceneDirty(worldController.gameObject.scene);

            EditorUtility.ClearProgressBar();




            ////éééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééééé



            ////++++++++++++++++
            ////searching all the regions
            //EditorUtility.DisplayProgressBar("Exporting SubScenes", "searching all the regions", 0);
            //var regions = new List<RegionBase>();
            //for (int i = 0; i < worldController.transform.childCount; i++)
            //{
            //    var region = worldController.transform.GetChild(i).GetComponent<RegionBase>();

            //    if (region != null)
            //    {
            //        regions.Add(region);
            //    }
            //}



            ////++++++++++++++++
            ////adding open world scene to build settings
            //if (!buildSettingsScenes.Exists(item => item.path == worldController.gameObject.scene.path))
            //{
            //    buildSettingsScenes.Add(new EditorBuildSettingsScene(worldController.gameObject.scene.path, true));
            //}

            ////++++++++++++++++
            ////creating duplicated SubScenes and moving them to new scenes
            //EditorUtility.DisplayProgressBar("Exporting SubScenes", "creating duplicated subScenes", 0);
            //var subScenes = new List<Scene>();
            //foreach (var region in regions)
            //{
            //    foreach (var subSceneLayer in Enum.GetValues(typeof(eSubSceneLayer)).Cast<eSubSceneLayer>())
            //    {
            //        foreach (var subSceneMode in region.AvailableSubSceneVariants)
            //        {
            //            var root = region.GetSubSceneRoot(subSceneLayer, subSceneMode);

            //            if (!root || root.childCount == 0) //if root is null or empty there is no need to create a subScene
            //            {
            //                continue;
            //            }

            //            //duplicating the SubScene
            //            foreach (var superRegionType in Enum.GetValues(typeof(eSuperRegionType)).Cast<eSuperRegionType>())
            //            {
            //                if (superRegionType == eSuperRegionType.Centre || region.DoNotDuplicate) //the centre already exists, there is no need to create a duplicate for it
            //                {
            //                    continue;
            //                }

            //                //paths
            //                string subScenePath = WorldUtility.GetSubScenePath(worldController.gameObject.scene.path, region.UniqueId, subSceneMode, subSceneLayer, superRegionType);

            //                //creating copy
            //                var rootCopy = Instantiate(root.gameObject).transform;
            //                rootCopy.SetParent(null, true);
            //                var offset = WorldController.SUPERREGION_OFFSETS[superRegionType];
            //                var translate = new Vector3(offset.x * worldController.WorldSize.x, offset.y * worldController.WorldSize.y, offset.z * worldController.WorldSize.z);
            //                rootCopy.Translate(translate);

            //                //informing world objects
            //                var worldObjects = rootCopy.GetComponentsInChildren<IWorldObjectDuplication>();
            //                for (int i = 0; i < worldObjects.Length; i++)
            //                {
            //                    worldObjects[i].OnDuplication();
            //                }

            //                //removing "do not repeat" objects
            //                if (superRegionType != eSuperRegionType.Centre)
            //                {
            //                    List<DoNotRepeatTag> doNotRepeatTags = rootCopy.GetComponentsInChildren<DoNotRepeatTag>(true).ToList();
            //                    while (doNotRepeatTags.Count > 0)
            //                    {
            //                        var tag = doNotRepeatTags[0];
            //                        doNotRepeatTags.RemoveAt(0);
            //                        DestroyImmediate(tag.gameObject);
            //                    }
            //                }

            //                //moving root to subScene
            //                var subScene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
            //                    UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
            //                    UnityEditor.SceneManagement.NewSceneMode.Additive
            //                );
            //                UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(rootCopy.gameObject, subScene);

            //                //saving the scene
            //                UnityEditor.SceneManagement.EditorSceneManager.SaveScene(subScene, subScenePath);
            //                subScenes.Add(subScene);

            //                //add subScene to buildsettings
            //                buildSettingsScenes.Add(new EditorBuildSettingsScene(subScenePath, true));
            //            }

            //            //handle centre root (can't be duplicated because that breaks the prefabs)

            //            //path
            //            string centreSubScenePath = WorldUtility.GetSubScenePath(worldController.gameObject.scene.path, region.UniqueId, subSceneMode, subSceneLayer, eSuperRegionType.Centre);

            //            //informing world objects
            //            var centreWorldObjects = root.GetComponentsInChildren<IWorldObjectDuplication>();
            //            for (int i = 0; i < centreWorldObjects.Length; i++)
            //            {
            //                centreWorldObjects[i].OnDuplication();
            //            }

            //            //moving root to subScene
            //            var centreSubScene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
            //                UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
            //                UnityEditor.SceneManagement.NewSceneMode.Additive
            //            );
            //            root.SetParent(null, true);
            //            UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(root.gameObject, centreSubScene);

            //            //saving and closing the sub scene
            //            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(centreSubScene, centreSubScenePath);
            //            UnityEditor.SceneManagement.EditorSceneManager.CloseScene(centreSubScene, true);

            //            //add subScene to buildsettings
            //            buildSettingsScenes.Add(new EditorBuildSettingsScene(centreSubScenePath, true));
            //        }
            //    }
            //}

            ////++++++++++++++++
            ////baking occlusion culling
            //EditorUtility.DisplayProgressBar("Exporting SubScenes", "computing occlusion", 0);
            //StaticOcclusionCulling.Compute();

            ////++++++++++++++++
            ////unloading the newly created scenes
            //EditorUtility.DisplayProgressBar("Exporting SubScenes", "unloading subScenes", 0);
            //foreach (var scene in subScenes)
            //{
            //    UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
            //}

            ////++++++++++++++++
            ////finishing
            //EditorUtility.DisplayProgressBar("Exporting SubScenes", "finishing", 0);
            //EditorBuildSettings.scenes = buildSettingsScenes.ToArray();

            //subScenesLoaded.boolValue = false;
            //serializedObject.ApplyModifiedProperties();

            //EditorUtility.SetDirty(worldController);
            //UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(worldController.gameObject.scene);

            //EditorUtility.ClearProgressBar();
        }

        private static Rect ComputeBounds()
        {
            float windowWidth = 400f;
            float windowHeight = 400f;
            float centerX = Screen.currentResolution.width * 0.5f;
            float centerY = Screen.currentResolution.height * 0.5f;
            return new Rect(centerX - windowWidth * 0.5f, centerY - windowHeight * 0.5f, windowWidth, windowHeight);
        }

        #endregion private methods

        //========================================================================================
    }
} //end of namespace