using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        eSubSceneType subSceneType;

        //========================================================================================

        public eSubSceneMode SubSceneMode { get { return subSceneMode; } }
        public eSubSceneType SubSceneType { get { return subSceneType; } }

        public void Initialize(eSubSceneMode subSceneMode, eSubSceneType subSceneType)
        {
            this.subSceneMode = subSceneMode;
            this.subSceneType = subSceneType;
        }

        //========================================================================================

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            string subSceneName = WorldUtility.GetSubSceneRootName(subSceneMode, subSceneType);
            if (name != subSceneName)
            {
                name = subSceneName;
            }
        }
#endif

        //========================================================================================
    }
} //end of namespace