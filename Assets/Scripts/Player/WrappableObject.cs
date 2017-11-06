using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Game.Player
{
    public class WrappableObject : MonoBehaviour
    {
        [SerializeField]
        List<Transform> followers;

        World.ChunkSystem.WorldController worldController;
        Transform myTransform;

        AxisInfo xAxisInfo;
        AxisInfo yAxisInfo;
        AxisInfo zAxisInfo;

        bool isInitialized = false;

        public void InitializeWrappableObject(World.ChunkSystem.WorldController worldController)
        {
            this.isInitialized = true;

            this.worldController = worldController;
            this.myTransform = this.transform;


            Vector3 worldPos = this.worldController.transform.position;
            Vector3 worldSize = this.worldController.WorldSize;

            this.xAxisInfo.worldSize = worldSize.x;
            this.xAxisInfo.minPos = worldPos.x - worldSize.x / 2f;
            this.xAxisInfo.maxPos = worldPos.x + worldSize.x / 2f;

            this.yAxisInfo.worldSize = worldSize.y;
            this.yAxisInfo.minPos = worldPos.y - worldSize.y / 2f;
            this.yAxisInfo.maxPos = worldPos.y + worldSize.y / 2f;

            this.zAxisInfo.worldSize = worldSize.z;
            this.zAxisInfo.minPos = worldPos.z - worldSize.z / 2f;
            this.zAxisInfo.maxPos = worldPos.z + worldSize.z / 2f;
        }

        void Update()
        {
            if (!this.isInitialized)
            {
                return;
            }

            bool teleporting = false;
            Vector3 playerPos = this.myTransform.position;
            Vector3 teleportOffset = Vector3.zero;

            if (this.worldController.RepeatAxes.x)
            {
                if (playerPos.x < this.xAxisInfo.minPos)
                {
                    teleportOffset.x = this.xAxisInfo.worldSize;
                    teleporting = true;
                }
                else if (playerPos.x > this.xAxisInfo.maxPos)
                {
                    teleportOffset.x = -this.xAxisInfo.worldSize;
                    teleporting = true;
                }
            }

            if (this.worldController.RepeatAxes.y)
            {
                if (playerPos.y < this.yAxisInfo.minPos)
                {
                    teleportOffset.y = this.yAxisInfo.worldSize;
                    teleporting = true;
                }
                else if (playerPos.y > this.yAxisInfo.maxPos)
                {
                    teleportOffset.y = -this.yAxisInfo.worldSize;
                    teleporting = true;
                }
            }

            if (this.worldController.RepeatAxes.z)
            {
                if (playerPos.z < this.zAxisInfo.minPos)
                {
                    teleportOffset.z = this.zAxisInfo.worldSize;
                    teleporting = true;
                }
                else if (playerPos.z > this.zAxisInfo.maxPos)
                {
                    teleportOffset.z = -this.zAxisInfo.worldSize;
                    teleporting = true;
                }
            }

            if (teleporting)
            {
                Vector3 newPlayerPos = playerPos + teleportOffset;

                //this.myTransform.position = newPlayerPos;

                var teleportPlayerEventArgs = new Utilities.EventManager.OnTeleportPlayerEventArgs(newPlayerPos, false);
                Utilities.EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);

                //foreach (var follower in this.followers)
                //{
                    //Vector3 newFollowerPos = follower.position + teleportOffset;
                    //follower.position = newFollowerPos;
                //}
            }
        }

        struct AxisInfo
        {
            public float minPos;
            public float maxPos;
            public float worldSize;
        }
    }
}