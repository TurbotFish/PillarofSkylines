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
        List<GameObject> childList = new List<GameObject>();

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
        public void InitializeSubChunk()
        {
            this.myTransform = this.transform;

            this.childList.Clear();
            for (int i = 0; i < this.myTransform.childCount; i++)
            {
                this.childList.Add(this.myTransform.GetChild(i).gameObject);
            }
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
        public void ActivateSubChunk()
        {
            StartCoroutine(ActivateSubChunkRoutine());
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeactivateSubChunk()
        {
            StartCoroutine(DeactivateSubChunkRoutine());
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

            this.childList.Clear();
            for (int i = 0; i < this.myTransform.childCount; i++)
            {
                this.childList.Add(this.myTransform.GetChild(i).gameObject);
            }
        }

        //############################################################################
        //############################################################################

        IEnumerator ActivateSubChunkRoutine()
        {
            foreach (var go in this.childList)
            {
                go.SetActive(true);

                yield return null;
            }
        }

        IEnumerator DeactivateSubChunkRoutine()
        {
            foreach (var go in this.childList)
            {
                go.SetActive(false);

                yield return null;
            }
        }

        //############################################################################
    }
}