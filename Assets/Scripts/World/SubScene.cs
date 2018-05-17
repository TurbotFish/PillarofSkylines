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
        SubSceneVariant subSceneVariant;

        [SerializeField]
        [HideInInspector]
        [FormerlySerializedAs("subSceneType")]
        SubSceneLayer subSceneLayer;

        //========================================================================================

        public SubSceneVariant SubSceneVariant { get { return subSceneVariant; } }
        public SubSceneLayer SubSceneLayer { get { return subSceneLayer; } }

        public void Initialize(SubSceneVariant subSceneVariant, SubSceneLayer subSceneLayer)
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