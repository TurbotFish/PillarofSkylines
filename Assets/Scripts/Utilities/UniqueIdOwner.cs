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

            //checking if an owned id already exists (owned by this object)
            foreach (var uniqueIdComponent in uniqueIds)
            {
                if (uniqueIdComponent.Owner == this)
                {
                    uniqueId = uniqueIdComponent;
                    return;
                }
            }

            //checking if an unowned id exists and appropriate it
            if (uniqueId == null)
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
            if (uniqueId == null)
            {
                uniqueId = gameObject.AddComponent<UniqueId>();
                uniqueId.Owner = this;

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
    }
} //end of namespace