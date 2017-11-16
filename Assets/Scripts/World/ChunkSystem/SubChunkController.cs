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
            this.myTransform = this.transform;

            this.childList.Clear();
            for (int i = 0; i < this.myTransform.childCount; i++)
            {
                this.childList.Add(this.myTransform.GetChild(i).gameObject);
            }

            var worldObjects = GetComponentsInChildren<Interaction.IWorldObject>();
            foreach(var worldObject in worldObjects)
            {
                worldObject.InitializeWorldObject(worldController);
            }

            this.IsActive = true;
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
        public void ActivateSubChunk(bool immediate = false)
        {
            if (this.IsActive)
            {
                return;
            }

            this.IsActive = true;
            StopAllCoroutines();

            if (immediate)
            {
                foreach (var go in this.childList)
                {
                    go.SetActive(true);
                }
            }
            else
            {               
                StartCoroutine(ActivateSubChunkRoutine());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeactivateSubChunk(bool immediate = false)
        {
            if (!this.IsActive)
            {
                return;
            }

            this.IsActive = false;
            StopAllCoroutines();

            if (immediate)
            {
                foreach (var go in this.childList)
                {
                    go.SetActive(false);
                }
            }
            else
            {
                StartCoroutine(DeactivateSubChunkRoutine());
            }
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

                foreach (var collider in colliders)
                {
                    Destroy(collider);
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
        //############################################################################

        IEnumerator ActivateSubChunkRoutine()
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

        IEnumerator DeactivateSubChunkRoutine()
        {
            lock (this.childListLock)
            {
                for (int i = 0; i < this.childList.Count; i++)
                {
                    this.childList[i].SetActive(false);

                    if (i % 10 == 0)
                    {
                        yield return null;
                    }
                }
            }
        }

        //############################################################################
    }
}