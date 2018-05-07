using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utilities
{
    [ExecuteInEditMode]
    public class UniqueId : MonoBehaviour
    {
        //========================================================================================

        [SerializeField]
        [HideInInspector]
        private string uniqueId;

        [SerializeField]
        [HideInInspector]
        private UniqueIdOwner owner;

        //========================================================================================

        public string Id
        {
            get
            {
                if (string.IsNullOrEmpty(uniqueId))
                {
                    SetId();
                }
                return uniqueId;
            }
        }

        public UniqueIdOwner Owner
        {
            get { return owner; }
            set
            {
                if (owner == null)
                {
                    owner = value;
                }
                else
                {
                    Debug.LogError("UniqueId: SetOwner: owner is already set!");
                }
            }
        }

        //========================================================================================

#if UNITY_EDITOR
        private void Awake()
        {

        }
#endif

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
            {
                SetId();
            }
        }
#endif

#if UNITY_EDITOR
        private void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                if (SceneUniqueIdManager.Instance)
                {
                    SceneUniqueIdManager.Instance.RemoveUniqueId(this);
                }
            }
        }
#endif

        //========================================================================================

        private void SetId()
        {
            // Construct the name of the scene with an underscore to prefix to the Guid
            string sceneName = gameObject.scene.name + "_";

            // if we are not part of a scene then we are a prefab so do not attempt to set the id
            if (sceneName == null || SceneUniqueIdManager.Instance == null) return;

            // Test if we need to make a new id
            bool hasSceneNameAtBeginning = (
                uniqueId != null &&
                uniqueId.Length > sceneName.Length &&
                uniqueId.Substring(0, sceneName.Length) == sceneName
            );


            bool anotherComponentAlreadyHasThisID = (
                uniqueId != null &&
                SceneUniqueIdManager.Instance.ContainsKey(uniqueId) &&
                SceneUniqueIdManager.Instance.GetUniqueId(uniqueId) != this
            );

            if (!hasSceneNameAtBeginning || anotherComponentAlreadyHasThisID)
            {
                //if (!hasSceneNameAtBeginning)
                //{
                //    Debug.Log("aaaa");
                //}
                //else
                //{
                //    Debug.Log("bbbb");
                //}

                string oldId = uniqueId;
                uniqueId = sceneName + Guid.NewGuid();

                //if (!string.IsNullOrEmpty(uniqueId) /*&& !string.IsNullOrEmpty(oldId)*/)
                //{
                Debug.LogWarningFormat("UniqueId: id changed from \"{0}\" to \"{1}\"", oldId, uniqueId);
                //}


#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
#endif
            }

            // We can be sure that the key is unique - now make sure we have 
            // it in our list
            if (!SceneUniqueIdManager.Instance.ContainsKey(uniqueId))
            {
                SceneUniqueIdManager.Instance.AddUniqueId(this);
            }
        }

        //========================================================================================
    }
} //end of namespace