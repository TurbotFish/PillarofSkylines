using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Utilities
{
    [ExecuteInEditMode]
    public class SceneUniqueIdManager : MonoBehaviour
    {
        //========================================================================================

        private static SceneUniqueIdManager instance;

        public static SceneUniqueIdManager Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<SceneUniqueIdManager>();
                }

                return instance;
            }
        }

        //========================================================================================

        [SerializeField]
        [HideInInspector]
        private List<UniqueId> idList = new List<UniqueId>();

        //========================================================================================

        /// <summary>
        /// Registers a UniqueId with the Manager.
        /// </summary>
        /// <param name="id"></param>
        public void AddUniqueId(UniqueId id)
        {
            if (!idList.Contains(id) && !ContainsKey(id.Id))
            {
                idList.Add(id);
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
#endif
        }

        public bool ContainsKey(string id)
        {
            idList.RemoveAll(item => item == null);
            foreach (var uniqueId in idList)
            {
                if (uniqueId.Id == id)
                {
                    return true;
                }
            }

            return false;
        }

        public UniqueId GetUniqueId(string id)
        {
            var t = idList.First(item => item.Id == id);
            return t;
        }

        public void RemoveUniqueId(UniqueId id)
        {
            if (idList.Contains(id))
            {
                idList.Remove(id);

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
#endif
            }
        }

        //========================================================================================

        /// <summary>
        /// [ExecuteInEditMode] Cleaning up the list of unique id's.
        /// </summary>
#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            int r = idList.RemoveAll(item => item == null);
            bool d = false;

            var componentsToDestroy = new List<UniqueId>();
            foreach (var uniqueIdComp in idList)
            {
                if (uniqueIdComp.Owner == null)
                {
                    componentsToDestroy.Add(uniqueIdComp);
                    d = true;
                }
            }

            foreach (var comp in componentsToDestroy)
            {
                RemoveUniqueId(comp);
                DestroyImmediate(comp);
            }

            if (!Application.isPlaying && (r > 0 || d))
            {
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
        }
#endif

        //========================================================================================
    }
} //end of namespace