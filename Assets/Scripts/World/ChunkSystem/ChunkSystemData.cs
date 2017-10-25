using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ChunkSystemData", fileName = "ChunkSystemData")]
    public class ChunkSystemData : ScriptableObject
    {
        //######################################################

        [SerializeField]
        [HideInInspector]
        Dictionary<eSubChunkLayer, Vector2> renderDistances = new Dictionary<eSubChunkLayer, Vector2>();

        public Vector2 GetRenderDistance(eSubChunkLayer layer)
        {
            if (this.renderDistances.ContainsKey(layer))
            {
                return this.renderDistances[layer];
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

                if (this.renderDistances.ContainsKey(layer))
                {
                    this.renderDistances[layer] = value;
                }
                else
                {
                    this.renderDistances.Add(layer, value);
                }
            }
        }

        //######################################################

        [SerializeField]
        float minUpdateDistance = 1f;

        public float GetMinUpdateDistance()
        {
            return this.minUpdateDistance;
        }

        //######################################################

        //void OnValidate()
        //{
        //    int enumSize = Enum.GetValues(typeof(eSubChunkLayer)).Length;

        //    if (this.RenderDistances.Count < enumSize)
        //    {
        //        for (int i = this.RenderDistances.Count + 1; i <= enumSize; i++)
        //        {
        //            this.RenderDistances.Add(Vector2.zero);
        //        }
        //    }
        //    else if (this.RenderDistances.Count > enumSize)
        //    {
        //        for (int i = this.RenderDistances.Count - 1; i >= enumSize; i--)
        //        {
        //            this.RenderDistances.RemoveAt(i);
        //        }
        //    }

        //    foreach (var range in this.RenderDistances)
        //    {
        //        if (range.x <= 0)
        //        {
        //            range.Set(0, range.y);
        //        }

        //        if (range.y <= 0)
        //        {
        //            range.Set(range.x, 0);
        //        }
        //    }
        //}
    }
}