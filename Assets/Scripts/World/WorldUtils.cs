using UnityEngine;

namespace Game.World
{
    public static class WorldUtility
    {
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
        public static string GetSubSceneName(string regionId, eSubSceneVariant subSceneVariant, eSubSceneLayer subSceneLayer)
        {
            return string.Concat(GetSubSceneRootName(subSceneVariant, subSceneLayer), "_", regionId, "_", subSceneVariant.ToString());
        }

        /// <summary>
        /// Returns the internal path to a SubScene scene object.
        /// </summary>
        /// <param name="worldScenePath"></param>
        /// <param name="regionId"></param>
        /// <param name="subSceneVariant"></param>
        /// <param name="subSceneLayer"></param>
        /// <returns></returns>
        public static string GetSubScenePath(string worldScenePath, string regionId, eSubSceneVariant subSceneVariant, eSubSceneLayer subSceneLayer)
        {
            string worldScenePathCleaned = worldScenePath.Remove(worldScenePath.LastIndexOf('.'));
            return string.Concat(worldScenePathCleaned, "/", GetSubSceneName(regionId, subSceneVariant, subSceneLayer), ".unity");
        }

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
    }
}