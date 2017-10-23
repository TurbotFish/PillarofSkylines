using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ChunkSystemData", fileName = "ChunkSystemData")]
    public class ChunkSystemData : ScriptableObject
    {
        [SerializeField]
        List<Vector2> RenderDistances = new List<Vector2>();

        void OnValidate()
        {
            int enumSize = Enum.GetValues(typeof(eSubChunkLayer)).Length;

            if (this.RenderDistances.Count < enumSize)
            {
                for (int i = this.RenderDistances.Count + 1; i <= enumSize; i++)
                {
                    this.RenderDistances.Add(Vector2.zero);
                }
            }
            else if (this.RenderDistances.Count > enumSize)
            {
                for (int i = this.RenderDistances.Count - 1; i >= enumSize; i--)
                {
                    this.RenderDistances.RemoveAt(i);
                }
            }

            foreach (var range in this.RenderDistances)
            {
                if (range.x <= 0)
                {
                    range.Set(0, range.y);
                }

                if (range.y <= 0)
                {
                    range.Set(range.x, 0);
                }
            }
        }
    }
}