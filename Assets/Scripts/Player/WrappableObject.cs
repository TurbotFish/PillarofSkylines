using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Game.World;
using Game.GameControl;

namespace Game.Player
{
    public class WrappableObject : MonoBehaviour
    {

        GameController gameController;
        Transform myTransform;

        AxisInfo xAxisInfo;
        AxisInfo yAxisInfo;
        AxisInfo zAxisInfo;

        private bool isInitialized;
        private bool isAxisInfoInitialized;
        private bool isActive;

        //#####################################################

        public void Initialize(GameController gameController)
        {
            Debug.Log("WrappableObject: Initialize");

            this.gameController = gameController;
            myTransform = transform;

            Utilities.EventManager.SceneChangedEvent += OnSceneChangedEventHandler;

            isInitialized = true;           
        }

        //#####################################################
        //#####################################################

        void Update()
        {
            if (!isInitialized || !isActive)
            {
                return;
            }
            else if(gameController.WorldController == null)
            {
                //Debug.LogWarning("WrappableObject: WorldController is null!");
                return;
            }
            else if (!isAxisInfoInitialized)
            {
                InitAxisInfo();
            }

            bool teleporting = false;
            Vector3 playerPos = this.myTransform.position;
            Vector3 teleportOffset = Vector3.zero;

            //if (this.worldController.RepeatAxes.x)
            //{
            //    if (playerPos.x < this.xAxisInfo.minPos)
            //    {
            //        teleportOffset.x = this.xAxisInfo.worldSize;
            //        teleporting = true;
            //    }
            //    else if (playerPos.x > this.xAxisInfo.maxPos)
            //    {
            //        teleportOffset.x = -this.xAxisInfo.worldSize;
            //        teleporting = true;
            //    }
            //}

            //if (this.worldController.RepeatAxes.y)
            //{
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
            //}

            //if (this.worldController.RepeatAxes.z)
            //{
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
            //}

            if (teleporting)
            {
                Vector3 newPlayerPos = playerPos + teleportOffset;

                var teleportPlayerEventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(playerPos, newPlayerPos);

                Utilities.EventManager.SendTeleportPlayerEvent(this, teleportPlayerEventArgs);
            }
        }

        //#####################################################
        //#####################################################

        void OnSceneChangedEventHandler(object sender, Utilities.EventManager.SceneChangedEventArgs args)
        {
            if (args.HasChangedToPillar)
            {
                isActive = false;
            }
            else
            {
                isActive = true;
            }
        }

        //#####################################################

        private void InitAxisInfo()
        {
            Vector3 worldPos = gameController.WorldController.transform.position;
            Vector3 worldSize = gameController.WorldController.WorldSize;

            xAxisInfo.worldSize = worldSize.x;
            xAxisInfo.minPos = worldPos.x - worldSize.x / 2f;
            xAxisInfo.maxPos = worldPos.x + worldSize.x / 2f;

            yAxisInfo.worldSize = worldSize.y;
            yAxisInfo.minPos = worldPos.y - worldSize.y / 2f;
            yAxisInfo.maxPos = worldPos.y + worldSize.y / 2f;

            zAxisInfo.worldSize = worldSize.z;
            zAxisInfo.minPos = worldPos.z - worldSize.z / 2f;
            zAxisInfo.maxPos = worldPos.z + worldSize.z / 2f;

            isAxisInfoInitialized = true;
        }

        struct AxisInfo
        {
            public float minPos;
            public float maxPos;
            public float worldSize;
        }
    }
}