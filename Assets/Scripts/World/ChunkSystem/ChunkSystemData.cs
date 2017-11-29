using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ChunkSystemData", fileName = "ChunkSystemData")]
    public class ChunkSystemData : ScriptableObject
    {
        //##################################################################

        [SerializeField]
        [HideInInspector]
        List<Vector2> renderDistances = new List<Vector2>();

        [SerializeField]
        float minUpdateDistance = 1f;
        public float MinUpdateDistance { get { return minUpdateDistance; } }

        //##################################################################

        public Vector2 GetRenderDistance(eSubChunkLayer layer)
        {
            if (renderDistances.Count > (int)layer)
            {
                return renderDistances[(int)layer];
            }
            else
            {
                return Vector2.zero;
            }
        }

        public void SetRenderDistance(eSubChunkLayer layer, Vector2 value)
        {
            if (!Application.isPlaying)
            {
                if (value.x <= 0)
                {
                    value.x = 0;
                }
                if (value.y <= 0)
                {
                    value.y = 0;
                }

                if (renderDistances.Count > (int)layer)
                {
                    renderDistances[(int)layer] = value;
                }
                else
                {
                    while(renderDistances.Count <= (int)layer)
                    {
                        renderDistances.Add(Vector2.zero);
                    }

                    renderDistances[(int)layer] = value;
                }
            }
        }

        //######################################################
    }
}