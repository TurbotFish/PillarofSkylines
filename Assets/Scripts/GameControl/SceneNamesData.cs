//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Game.GameControl
//{
//    [CreateAssetMenu(menuName = "ScriptableObjects/SceneNamesData", fileName = "SceneNamesData")]
//    public class SceneNamesData : ScriptableObject
//    {
//        [SerializeField]
//        [HideInInspector]
//        Object openWorldScene;

//        [SerializeField]
//        [HideInInspector]
//        string openWorldSceneName;

//        public string GetOpenWorldSceneName()
//        {
//            return this.openWorldSceneName;
//        }

//        [SerializeField]
//        [HideInInspector]
//        List<Object> pillarScenes = new List<Object>();

//        [SerializeField]
//        [HideInInspector]
//        List<string> pillarSceneNames = new List<string>();

//        public string GetPillarSceneName(World.ePillarId pillarId)
//        {
//            if (this.pillarScenes.Count > (int)pillarId /*&& this.pillarScenes[(int)pillarId] != null*/)
//            {
//                return this.pillarSceneNames[(int)pillarId];
//            }
//            else
//            {
//                return string.Empty;
//            }
//        }

//#if UNITY_EDITOR
//        public Object OpenWorldScene_Editor { get { return this.openWorldScene; } set { this.openWorldScene = value; } }

//        public List<Object> PillarScenes_Editor { get { return this.pillarScenes; } set { this.pillarScenes = value; } }

//        public void ResetStrings()
//        {
//            if (this.openWorldScene != null)
//            {
//                this.openWorldSceneName = this.openWorldScene.name;
//            }
//            else
//            {
//                this.openWorldSceneName = string.Empty;
//            }

//            this.pillarSceneNames.Clear();
//            foreach (var scene in this.pillarScenes)
//            {
//                if (scene != null)
//                {
//                    this.pillarSceneNames.Add(scene.name);
//                }
//                else
//                {
//                    this.pillarSceneNames.Add(string.Empty);
//                }
//            }
//        }
//#endif

//    }
//} //end of namespace