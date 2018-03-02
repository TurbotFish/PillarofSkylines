using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.World
{
    [ExecuteInEditMode]
    public class SubScene : MonoBehaviour
    {
        //========================================================================================

        [SerializeField]
        [HideInInspector]
        eSubSceneMode subSceneMode;

        [SerializeField]
        [HideInInspector]
        [FormerlySerializedAs("subSceneType")]
        eSubSceneLayer subSceneLayer;

        //========================================================================================

        public eSubSceneMode SubSceneMode { get { return subSceneMode; } }
        public eSubSceneLayer SubSceneLayer { get { return subSceneLayer; } }

        public void Initialize(eSubSceneMode subSceneMode, eSubSceneLayer subSceneLayer)
        {
            this.subSceneMode = subSceneMode;
            this.subSceneLayer = subSceneLayer;
        }

        //========================================================================================

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            string subSceneName = WorldUtility.GetSubSceneRootName(subSceneMode, subSceneLayer);
            if (name != subSceneName)
            {
                name = subSceneName;
            }
        }
#endif

        //========================================================================================
    }
} //end of namespace