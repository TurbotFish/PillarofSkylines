using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class WorldController : MonoBehaviour
    {
        //##################################################################

        [SerializeField]
        Vector3 worldSize;
        public Vector3 WorldSize { get { return this.worldSize; } }

        [SerializeField]
        Bool3 repeatAxes;
        public Bool3 RepeatAxes { get { return this.repeatAxes; } }

        [SerializeField]
        int numberOfRepetitions;

        //##################################################################

        public ChunkSystemData ChunkSystemData { get; private set; }

        Transform playerTransform;
        Transform cameraTransform;

        List<RegionController> regionList = new List<RegionController>();

        List<GameObject> worldCopies = new List<GameObject>();

        List<Interaction.Favour> favourList = new List<Interaction.Favour>();

        bool isInitialized = false;

        System.Object activationLock = new System.Object();
        LinkedList<GameObject> objectsToActivate = new LinkedList<GameObject>();
        LinkedList<GameObject> objectsToDeactivate = new LinkedList<GameObject>();

        int regionUpdateIndex = 0;

        System.Diagnostics.Stopwatch stopwatch;

        //##################################################################

        #region initialization

        public void InitializeWorldController(Transform playerTransform, Transform cameraTransform)
        {
            isInitialized = true;

            this.playerTransform = playerTransform;
            this.cameraTransform = cameraTransform;

            ChunkSystemData = Resources.Load<ChunkSystemData>("ScriptableObjects/ChunkSystemData");

            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                var region = child.GetComponent<RegionController>();

                if (region != null)
                {
                    regionList.Add(region);
                    region.InitializeRegion(this);
                }
            }

            //
            Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;

            //copying
            CreateCopies();
        }

        #endregion initialization

        //##################################################################

        #region update

        void Update()
        {
            if (!isInitialized)
            {
                return;
            }

            //******************************************
            //updating world
            stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < ChunkSystemData.RegionUpdatesPerFrame; i++)
            {
                regionList[regionUpdateIndex++].UpdateRegion(playerTransform.position, cameraTransform.position);

                if (regionUpdateIndex == regionList.Count)
                {
                    regionUpdateIndex = 0;
                }
            }
            stopwatch.Stop();
            var worldUpdateTime = stopwatch.Elapsed;

            //******************************************
            //object activation
            lock (activationLock)
            {
                //Debug.LogFormat("WorldController: Update: object activation: to activate = {0}; to deactivate = {1}", objectsToActivate.Count, objectsToDeactivate.Count);
                stopwatch = System.Diagnostics.Stopwatch.StartNew();

                int quota = ChunkSystemData.ObjectActivationsPerFrame;
                int activationQuota = Mathf.Min(quota, objectsToActivate.Count);
                quota -= activationQuota;
                int deactivationQuota = Mathf.Min(quota, objectsToDeactivate.Count);

                for (int i = 0; i < activationQuota; i++)
                {
                    objectsToActivate.First.Value.SetActive(true);
                    objectsToActivate.RemoveFirst();
                }

                for (int i = 0; i < deactivationQuota; i++)
                {
                    objectsToDeactivate.First.Value.SetActive(false);
                    objectsToDeactivate.RemoveFirst();
                }

                stopwatch.Stop();
            }

            var objectActivationTime = stopwatch.Elapsed;
            //Debug.LogFormat("WorldController: Update: A={0}; B={1}", worldUpdateTime.Milliseconds / 1000f, objectActivationTime.Milliseconds / 1000f);
        }

        #endregion update

        //##################################################################

        #region copying

        void CreateCopies()
        {
            var newRegionsList = new List<RegionController>();

            if (repeatAxes.x || repeatAxes.y || repeatAxes.z)
            {
                for (int x = -1 * numberOfRepetitions; x <= numberOfRepetitions; x++)
                {
                    if (x != 0 && !repeatAxes.x)
                    {
                        continue;
                    }

                    for (int y = -1 * numberOfRepetitions; y <= numberOfRepetitions; y++)
                    {
                        if (y != 0 && !repeatAxes.y)
                        {
                            continue;
                        }

                        for (int z = -1 * numberOfRepetitions; z <= numberOfRepetitions; z++)
                        {
                            if (z != 0 && !repeatAxes.z)
                            {
                                continue;
                            }
                            else if (x == 0 && y == 0 && z == 0)
                            {
                                continue;
                            }

                            var go = new GameObject(string.Format("WorldCopy{0}{1}{2}", x, y, z));
                            go.transform.parent = transform;

                            for (int i = 0; i < regionList.Count; i++)
                            {
                                var region = regionList[i];

                                var regionCopyGo = region.CreateCopy(go.transform);
                                var regionCopyScript = regionCopyGo.GetComponent<RegionController>();

                                regionCopyScript.InitializeRegion(this);
                                newRegionsList.Add(regionCopyScript);
                            }

                            //moving the copy has to be done after creating all the children
                            go.transform.Translate(new Vector3(x * worldSize.x, y * worldSize.y, z * worldSize.z));

                            worldCopies.Add(go);
                        }
                    }
                }
            }

            for (int i = 0; i < newRegionsList.Count; i++)
            {
                regionList.Add(newRegionsList[i]);
            }
        }

        #endregion copying

        //##################################################################

        #region favour methods

        public void RegisterFavour(Interaction.Favour favour)
        {
            if (!favourList.Contains(favour))
            {
                favourList.Add(favour);
            }
        }

        public Interaction.Favour FindNearestFavour(Vector3 position)
        {
            if (favourList.Count == 0)
            {
                return null;
            }

            var nearestFavour = favourList[0];
            float shortestDistance = Vector3.Distance(position, nearestFavour.MyTransform.position);

            for (int i = 1; i < favourList.Count; i++)
            {
                var favour = favourList[i];
                float distance = Vector3.Distance(position, favour.MyTransform.position);

                if (distance < shortestDistance)
                {
                    nearestFavour = favour;
                    shortestDistance = distance;
                }
            }

            return nearestFavour;
        }

        #endregion favour methods

        //##################################################################

        #region subChunk activation

        public void QueueObjectsToSetActive(List<GameObject> objectList, bool active)
        {
            lock (activationLock)
            {
                if (active)
                {
                    for (int i = 0; i < objectList.Count; i++)
                    {
                        var currentObject = objectList[i];

                        if (!objectsToActivate.Contains(currentObject))
                        {
                            objectsToActivate.AddLast(currentObject);
                        }

                        objectsToDeactivate.Remove(currentObject);
                    }
                }
                else
                {
                    for (int i = 0; i < objectList.Count; i++)
                    {
                        var currentObject = objectList[i];

                        objectsToActivate.Remove(currentObject);

                        if (!objectsToDeactivate.Contains(currentObject))
                        {
                            objectsToDeactivate.AddLast(currentObject);
                        }
                    }
                }
            }
        }

        #endregion subChunk activation

        //##################################################################

        #region event handlers

        void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
        {
            var favoursToRemove = new List<Interaction.Favour>();
            foreach (var favour in favourList)
            {
                if (favour.InstanceId == args.FavourId)
                {
                    favoursToRemove.Add(favour);
                }
            }

            foreach (var favour in favoursToRemove)
            {
                favourList.Remove(favour);
            }
        }

        #endregion event handlers

        //##################################################################

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

            if (drawGizmo && Application.isEditor) {
                Gizmos.color = gizmoColor;
                //Gizmos.DrawCube(transform.position, worldSize);
                Gizmos.DrawWireCube(transform.position, worldSize);
            }
        }
        #endregion

        //##################################################################
    }
}