using UnityEngine;
using System.Collections.Generic;

namespace Game.World.ChunkSystem
{
    public class WrappableObject : MonoBehaviour
    {

        [SerializeField]
        List<Transform> followers;
        List<Vector3> followOffsets = new List<Vector3>();

        WorldController wrapper;
        Transform my;
        bool teleporting;

        void Awake()
        {
            if (followers.Count > 0)
            {
                for (int i = 0; i < followers.Count; i++)
                {
                    followOffsets.Add(Vector3.zero);
                }
            }
        }

        void Start()
        {
            wrapper = FindObjectOfType<WorldController>();
            my = transform;
        }

        void Update()
        {
            if (!wrapper) return;

            Vector3 pos = my.position;
            Vector3 wrapPos = wrapper.transform.position;
            Vector3 worldSize = wrapper.WorldSize;
            if (followers.Count > 0)
            {
                for (int i = 0; i < followers.Count; i++)
                {
                    Vector3 _offset = followers[i].position - pos;
                    followOffsets[i] = _offset;
                }
            }
            //followOffset = follower.position - pos;
            teleporting = false;

            if (wrapper.RepeatAxes.x)
            {
                if (pos.x > wrapPos.x + worldSize.x / 2)
                {
                    pos.x -= worldSize.x;
                    teleporting = true;
                }
                else if (pos.x < wrapPos.x - worldSize.x / 2)
                {
                    pos.x += worldSize.x;
                    teleporting = true;
                }
            }

            if (wrapper.RepeatAxes.y)
            {
                if (pos.y > wrapPos.y + worldSize.y / 2)
                {
                    pos.y -= worldSize.y;
                    teleporting = true;
                }
                else if (pos.y < wrapPos.y - worldSize.y / 2)
                {
                    pos.y += worldSize.y;
                    teleporting = true;
                }
            }

            if (wrapper.RepeatAxes.z)
            {
                if (pos.z > wrapPos.z + worldSize.z / 2)
                {
                    pos.z -= worldSize.z;
                    teleporting = true;
                }
                else if (pos.z < wrapPos.z - worldSize.z / 2)
                {
                    pos.z += worldSize.z;
                    teleporting = true;
                }
            }

            if (teleporting)
                SetPosition(pos);
        }

        public virtual void SetPosition(Vector3 pos)
        {
            my.position = pos;
            if (followers.Count > 0)
            {
                for (int i = 0; i < followers.Count; i++)
                {
                    followers[i].position = pos + followOffsets[i];
                }
            }

        }
    }
}