﻿using System;
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

        Transform myTransform;

        public bool IsActive { get; private set; }
        public bool IsCopy { get; private set; }

        List<Renderer> rendererList = new List<Renderer>();
        List<Interaction.IWorldObjectActivation> worldObjectActivationList = new List<Interaction.IWorldObjectActivation>();

        //##################################################################

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
            myTransform = transform;

            gameObject.SetActive(true);

            //fill renderer list
            FillRendererList();

            //initialize world objects
            var worldObjects = GetComponentsInChildren<Interaction.IWorldObjectInitialization>(true);
            for (int i = 0; i < worldObjects.Length; i++)
            {
                var worldObject = worldObjects[i];

                worldObject.Initialize(worldController, IsCopy);
            }

            worldObjectActivationList.Clear();
            worldObjectActivationList.AddRange(GetComponentsInChildren<Interaction.IWorldObjectActivation>(true));

            //
            IsActive = true;
        }

        /// <summary>
        /// 
        /// </summary>
        void InitializeSubChunkCopy(SubChunkController originalSubChunk)
        {
            layer = originalSubChunk.Layer;

            IsCopy = true;
        }

        //##################################################################

        void OnTransformChildrenChanged()
        {
            if (myTransform == null)
            {
                return;
            }

            Debug.LogFormat("SubChunk \"{0}\": OnTransformChildrenChanged called!", name);

            FillRendererList();
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
        public List<Renderer> SetSubChunkActive(bool active, bool immediate = false)
        {
            var result = new List<Renderer>();

            if (IsActive == active)
            {
                return result;
            }

            //handle renderers
            if (immediate)
            {
                for (int i = 0; i < rendererList.Count; i++)
                {
                    rendererList[i].enabled = active;
                }
            }
            else
            {
                result.AddRange(rendererList);
                //Debug.LogFormat("SubChunk: SetActive: name={0} ; value={1}", name, active);
            }

            //handle world objects
            if (active)
            {
                for (int i = 0; i < worldObjectActivationList.Count; i++)
                {
                    worldObjectActivationList[i].OnSubChunkActivated();
                }
            }
            else
            {
                for (int i = 0; i < worldObjectActivationList.Count; i++)
                {
                    worldObjectActivationList[i].OnSubChunkDeactivated();
                }
            }

            //
            IsActive = active;
            return result;
        }

        //##################################################################

        void FillRendererList()
        {
            rendererList.Clear();

            var candidateStack = new Stack<Transform>();
            for (int i = 0; i < myTransform.childCount; i++)
            {
                candidateStack.Push(myTransform.GetChild(i));
            }

            while (candidateStack.Count > 0)
            {
                var candidate = candidateStack.Pop();

                var renderers = candidate.GetComponents<Renderer>();
                if (renderers.Length > 0)
                {
                    rendererList.AddRange(renderers);
                }

                if (candidate.childCount > 0)
                {
                    for (int i = 0; i < candidate.childCount; i++)
                    {
                        candidateStack.Push(candidate.GetChild(i));
                    }
                }
            }
        }

        //##################################################################
    }
} //end of namespace