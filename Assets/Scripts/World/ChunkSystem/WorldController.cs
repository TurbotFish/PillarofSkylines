using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class WorldController : MonoBehaviour
    {
        [SerializeField]
        Vector3 worldSize;
        public Vector3 WorldSize { get { return this.worldSize; } }

        [SerializeField]
        Bool3 repeatAxes;
        public Bool3 RepeatAxes { get { return this.repeatAxes; } }

        [SerializeField]
        int numberOfRepetitions;

        //################

        ChunkSystemData data;

        Transform playerTransform;

        List<RegionController> regionList = new List<RegionController>();

        List<GameObject> worldCopies = new List<GameObject>();
        List<RegionController> regionCopyList = new List<RegionController>();

        bool isInitialized = false;
        Vector3 previousPlayerPos = Vector3.zero;

        //####################################################################
        //####################################################################

        public void InitializeWorldController(Transform playerTransform)
        {
            this.isInitialized = true;

            this.playerTransform = playerTransform;

            this.data = Resources.Load<ChunkSystemData>("ScriptableObjects/ChunkSystemData");

            int childCount = this.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = this.transform.GetChild(i);
                var region = child.GetComponent<RegionController>();

                if (region != null)
                {
                    this.regionList.Add(region);
                    region.InitializeRegion(this.data);
                }
            }

            //copying
            CreateCopies();
        }

        //####################################################################
        //####################################################################

        void FixedUpdate()
        {
            if (!this.isInitialized)
            {
                return;
            }

            var currentPlayerPos = this.playerTransform.position;
            float posDelta = Vector3.Distance(this.previousPlayerPos, currentPlayerPos);

            if (posDelta >= this.data.GetMinUpdateDistance())
            {
                foreach (var region in this.regionList)
                {
                    region.UpdateRegion(currentPlayerPos);
                }

                foreach (var region in this.regionCopyList)
                {
                    region.UpdateRegion(currentPlayerPos);
                }

                this.previousPlayerPos = currentPlayerPos;
            }
        }

        //####################################################################
        //####################################################################

        void CreateCopies()
        {
            if (repeatAxes.x || repeatAxes.y || repeatAxes.z)
            {
                for (int x = -1 * this.numberOfRepetitions; x <= this.numberOfRepetitions; x++)
                {
                    if (x != 0 && !this.repeatAxes.x)
                    {
                        continue;
                    }

                    for (int y = -1 * this.numberOfRepetitions; y <= this.numberOfRepetitions; y++)
                    {
                        if (y != 0 && !this.repeatAxes.y)
                        {
                            continue;
                        }

                        for (int z = -1 * this.numberOfRepetitions; z <= this.numberOfRepetitions; z++)
                        {
                            if (z != 0 && !this.repeatAxes.z)
                            {
                                continue;
                            }
                            else if (x == 0 && y == 0 && z == 0)
                            {
                                continue;
                            }

                            var go = new GameObject(string.Format("WorldCopy{0}{1}{2}", x, y, z));
                            go.transform.parent = this.transform;

                            foreach (var region in this.regionList)
                            {
                                var regionCopyGo = region.CreateCopy(go.transform);
                                var regionCopyScript = regionCopyGo.GetComponent<RegionController>();

                                regionCopyScript.InitializeRegion(this.data);
                                this.regionCopyList.Add(regionCopyScript);
                            }

                            //moving the copy has to be done after creating all the children
                            go.transform.Translate(new Vector3(x * this.worldSize.x, y * this.worldSize.y, z * this.worldSize.z));
                        }
                    }
                }
            }
        }

        //####################################################################
        //####################################################################

        #region Gizmo

        [Header("Gizmo"), SerializeField]
        bool drawGizmo;
        [SerializeField]
        Color gizmoColor;

        void OnDrawGizmos()
        {
            //if (this.isInitialized && Application.isEditor && Application.isPlaying)
            //{
            //    float size = this.data.GetRenderDistance(eSubChunkLayer.Near_VeryLarge).y * 2;
            //    var colour = Color.yellow;
            //    colour.a = 0.3f;
            //    Gizmos.color = colour;
            //    Gizmos.DrawCube(this.playerTransform.position, new Vector3(size, size, size));
            //}

            if (drawGizmo)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawCube(transform.position, worldSize);
            }
        }
        #endregion

        //####################################################################
        //####################################################################
    }
}