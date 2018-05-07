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
        [FormerlySerializedAs("subSceneMode")]
        eSubSceneVariant subSceneVariant;

        [SerializeField]
        [HideInInspector]
        [FormerlySerializedAs("subSceneType")]
        eSubSceneLayer subSceneLayer;

        //========================================================================================

        public eSubSceneVariant SubSceneVariant { get { return subSceneVariant; } }
        public eSubSceneLayer SubSceneLayer { get { return subSceneLayer; } }

        public void Initialize(eSubSceneVariant subSceneVariant, eSubSceneLayer subSceneLayer)
        {
            this.subSceneVariant = subSceneVariant;
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

            string subSceneName = WorldUtility.GetSubSceneRootName(subSceneVariant, subSceneLayer);
            if (name != subSceneName)
            {
                name = subSceneName;
            }
        }
#endif

        //========================================================================================
    }
} //end of namespace