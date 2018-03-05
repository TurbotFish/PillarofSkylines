using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utilities
{
    //[RequireComponent(typeof(UniqueId))]
    public abstract class UniqueIdOwner : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private UniqueId uniqueId;

        public string UniqueId { get { if (!uniqueId) { SetUniqueId(); } return uniqueId.Id; } }

        protected virtual void OnValidate()
        {
            //add UniqueId component (this is for updating existing gameObjects)
            if (uniqueId == null)
            {
                SetUniqueId();
            }
        }

        protected virtual void Reset()
        {
            if(uniqueId == null)
            {
                SetUniqueId();
            }
        }

        private void SetUniqueId()
        {
            var uniqueIds = GetComponents<UniqueId>();
            UniqueId myUniqueId = null;

            foreach(var uniqueId in uniqueIds)
            {
                if(uniqueId.Owner == this)
                {
                    myUniqueId = uniqueId;
                    break;
                }
                else if(uniqueId.Owner == null)
                {
                    uniqueId.Owner = this;
                    myUniqueId = uniqueId;
                    break;
                }
            }

            if(myUniqueId != null)
            {
                uniqueId = myUniqueId;
            }
            else
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
    }
} //end of namespace