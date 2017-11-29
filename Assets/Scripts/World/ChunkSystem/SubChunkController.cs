using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class SubChunkController : MonoBehaviour
    {
        //##################################################################

        //layer
        [SerializeField]
        eSubChunkLayer layer = 0;
        public eSubChunkLayer Layer { get { return this.layer; } }

        //wrapping
        [SerializeField]
        bool doNotWrap = false;

        WorldController worldController;

        Transform myTransform;
        public bool IsActive { get; private set; }

        List<GameObject> childList = new List<GameObject>();
        System.Object childListLock = new System.Object();


#if UNITY_EDITOR
        public void Editor_InitializeSubChunk(eSubChunkLayer layer)
        {
            this.layer = layer;
        }
#endif

        //##################################################################

        /// <summary>
        /// 
        /// </summary>
        public void InitializeSubChunk(WorldController worldController)
        {
            this.worldController = worldController;

            myTransform = transform;

            gameObject.SetActive(true);

            childList.Clear();
            for (int i = 0; i < myTransform.childCount; i++)
            {
                childList.Add(myTransform.GetChild(i).gameObject);
            }

            var worldObjects = GetComponentsInChildren<Interaction.IWorldObject>();
            for (int i = 0; i < worldObjects.Length; i++)
            {
                var worldObject = worldObjects[i];

                worldObject.InitializeWorldObject(worldController);
            }

            IsActive = true;
        }

        /// <summary>
        /// 
        /// </summary>
        void InitializeSubChunkCopy(SubChunkController originalSubChunk)
        {
            layer = originalSubChunk.Layer;
        }

        //##################################################################

        /// <summary>
        /// 
        /// </summary>
        public void CreateCopy(Transform parent)
        {
            if (doNotWrap)
            {
                var go = new GameObject(gameObject.name);
                go.transform.parent = parent;
                go.transform.localPosition = transform.localPosition;

            }
            else
            {
                var go = Instantiate(gameObject, parent);

                go.GetComponent<SubChunkController>().InitializeSubChunkCopy(this);

                var colliders = go.GetComponentsInChildren<Collider>(true);
                for (int i = 0; i < colliders.Length; i++)
                {
                    Destroy(colliders[i]);
                }

                var renderers = go.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
        }

        //##################################################################

        /// <summary>
        /// Activates or deactivates all GameObjects in the SubChunk.
        /// If <paramref name="immediate"/> is true, all objects are de-activated at once which causes some lag.
        /// Otherwise the de-activation is spread out over several frames (handled by the <see cref="WorldController"/>).
        /// </summary>
        public void SetSubChunkActive(bool active, bool immediate = false)
        {
            if (IsActive == active)
            {
                return;
            }

            IsActive = active;
            StopAllCoroutines();

            lock (childListLock)
            {
                if (immediate)
                {
                    for (int i = 0; i < childList.Count; i++)
                    {
                        var go = childList[i];

                        go.SetActive(active);
                    }
                }
                else
                {
                    worldController.QueueObjectsToSetActive(childList, active);
                }
            }
        }

        //##################################################################

        void OnTransformChildrenChanged()
        {
            if (myTransform == null)
            {
                return;
            }

            Debug.LogFormat("SubChunk \"{0}\": OnTransformChildrenChanged called!", name);

            lock (childListLock)
            {
                childList.Clear();
                for (int i = 0; i < myTransform.childCount; i++)
                {
                    childList.Add(myTransform.GetChild(i).gameObject);
                }
            }
        }

        //##################################################################
    }
} //end of namespace