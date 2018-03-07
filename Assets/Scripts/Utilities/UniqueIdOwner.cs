using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utilities
{
    //[RequireComponent(typeof(UniqueId))]
    public abstract class UniqueIdOwner : MonoBehaviour
    {
        //========================================================================================

        [SerializeField]
        [HideInInspector]
        private UniqueId uniqueId;

        //========================================================================================

        public string UniqueId { get { if (!uniqueId) { SetUniqueId(); } return uniqueId.Id; } }

        //========================================================================================

        protected virtual void OnValidate()
        {
            //add UniqueId component (this is for updating existing gameObjects)
            if (uniqueId == null || uniqueId.Owner != this)
            {
                SetUniqueId();
            }
        }

        protected virtual void Reset()
        {
            if (uniqueId == null || uniqueId.Owner != this)
            {
                SetUniqueId();
            }
        }

        //========================================================================================

        private void SetUniqueId()
        {
            var uniqueIds = GetComponents<UniqueId>();
            UniqueId myUniqueId = null;

            //checking if an owned id already exists (owned by this object)
            foreach (var uniqueId in uniqueIds)
            {
                if (uniqueId.Owner == this)
                {
                    this.uniqueId = uniqueId;
                    return;
                }
            }

            //checking if an unowned id exists and appropriate it
            if (this.uniqueId == null)
            {
                foreach (var uniqueId in uniqueIds)
                {
                    if (uniqueId.Owner == null)
                    {
                        uniqueId.Owner = this;
                        this.uniqueId = uniqueId;
                        return;
                    }
                }
            }

            //creating a new UniqueId component
            if (this.uniqueId == null)
            {
                uniqueId = gameObject.AddComponent<UniqueId>();
                uniqueId.Owner = this;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
#endif
            }
        }

        //========================================================================================
    }
} //end of namespace