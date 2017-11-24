using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class SubChunkController : MonoBehaviour
    {
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

        //############################################################################

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
            foreach (var worldObject in worldObjects)
            {
                worldObject.InitializeWorldObject(worldController);
            }

            IsActive = true;
        }

        /// <summary>
        /// 
        /// </summary>
        void InitializeSubChunkCopy(SubChunkController originalSubChunk)
        {
            this.layer = originalSubChunk.Layer;
        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateCopy(Transform parent)
        {
            if (this.doNotWrap)
            {
                var go = new GameObject(this.gameObject.name);
                go.transform.parent = parent;
                go.transform.localPosition = this.transform.localPosition;

            }
            else
            {
                var go = Instantiate(this.gameObject, parent);

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

        //############################################################################
        //############################################################################

        /// <summary>
        /// Activates or deactivates all GameObjects in the SubChunk.
        /// If <paramref name="immediate"/> is true, all objects are de-activated at once which causes some lag.
        /// Otherwise the de-activation is spread out over several frames (handled by the <see cref="WorldController"/>).
        /// </summary>
        public void SetSubChunkActive(bool active, bool immediate = false)
        {
            if (this.IsActive == active)
            {
                return;
            }

            this.IsActive = active;
            StopAllCoroutines();

            lock (this.childListLock)
            {
                if (immediate)
                {
                    foreach (var go in this.childList)
                    {
                        go.SetActive(active);
                    }
                }
                else
                {
                    this.worldController.QueueObjectsToSetActive(this.childList, active);
                    //StartCoroutine(SetActiveRoutine(active));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        IEnumerator SetActiveRoutine(bool active)
        {
            lock (this.childListLock)
            {
                for (int i = 0; i < this.childList.Count; i++)
                {
                    this.childList[i].SetActive(true);

                    if (i % 10 == 0)
                    {
                        yield return null;
                    }
                }
            }
        }

        //############################################################################
        //############################################################################

        void OnTransformChildrenChanged()
        {
            if (this.myTransform == null)
            {
                return;
            }

            Debug.LogErrorFormat("SubChunk \"{0}\": OnTransformChildrenChanged called!", this.name);

            lock (this.childListLock)
            {
                this.childList.Clear();
                for (int i = 0; i < this.myTransform.childCount; i++)
                {
                    this.childList.Add(this.myTransform.GetChild(i).gameObject);
                }
            }
        }

        //############################################################################
    }
} //end of namespace