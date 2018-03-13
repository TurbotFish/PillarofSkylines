using UnityEngine;

namespace Game.World
{
    public static class WorldUtility
    {
        //========================================================================================

        /// <summary>
        /// Returns the name for a SubScene root game object.
        /// </summary>
        /// <param name="subSceneVariant"></param>
        /// <param name="subSceneLayer"></param>
        /// <returns></returns>
        public static string GetSubSceneRootName(eSubSceneVariant subSceneVariant, eSubSceneLayer subSceneLayer)
        {
            return string.Concat("SubScene_", subSceneVariant.ToString(), "_", subSceneLayer.ToString());
        }

        /// <summary>
        /// Returns the name for a SubScene scene object.
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="subSceneVariant"></param>
        /// <param name="subSceneLayer"></param>
        /// <returns></returns>
        public static string GetSubSceneName(string regionId, eSubSceneVariant subSceneVariant, eSubSceneLayer subSceneLayer, eSuperRegionType superRegionType)
        {
            return string.Concat("SubScene_", superRegionType, "_", subSceneVariant, "_", subSceneLayer, "_", regionId);
        }

        /// <summary>
        /// Returns the internal path to a SubScene scene object.
        /// </summary>
        /// <param name="worldScenePath"></param>
        /// <param name="regionId"></param>
        /// <param name="subSceneVariant"></param>
        /// <param name="subSceneLayer"></param>
        /// <returns></returns>
        public static string GetSubScenePath(string worldScenePath, string regionId, eSubSceneVariant subSceneVariant, eSubSceneLayer subSceneLayer, eSuperRegionType superRegionType)
        {
            string subScenePath = GetSubSceneFolderPath(worldScenePath);
            return string.Concat(subScenePath, "/", GetSubSceneName(regionId, subSceneVariant, subSceneLayer, superRegionType), ".unity");
        }

        //========================================================================================

        public static string GetSubSceneFolderPath(string worldScenePath)
        {
            string result = worldScenePath.Remove(worldScenePath.LastIndexOf('.')); //remove '.Unity'
            result += "_SubScenes";

            return result;
        }

        public static string GetSubSceneFolderName(string worldSceneName)
        {
            return worldSceneName + "_SubScenes";
        }

        //========================================================================================

        /// <summary>
        /// Returns the full path to a SubScene scene object.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFullPath(string path)
        {
            string appPath = Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets"));
            return string.Concat(appPath, path);
        }

        //========================================================================================

    }
}