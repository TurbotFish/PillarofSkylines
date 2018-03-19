//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Game.World.ChunkSystem
//{
//    public class WorldController : MonoBehaviour
//    {
//        //##################################################################

//        [SerializeField]
//        Vector3 worldSize;      

//        [SerializeField]
//        Bool3 repeatAxes;      

//        [SerializeField]
//        int numberOfRepetitions;

//        //##################################################################

//        public ChunkSystemData ChunkSystemData { get; private set; }

//        GameControl.IGameControllerBase gameController;
//        Transform playerTransform;
//        Transform cameraTransform;

//        List<RegionController> regionList = new List<RegionController>();

//        List<GameObject> worldCopies = new List<GameObject>();

//        List<Interaction.CurrencyPickUp> favourList = new List<Interaction.CurrencyPickUp>();

//        bool isInitialized = false;

//        Queue<Renderer> rendererQueue = new Queue<Renderer>();
//        int regionUpdateIndex = 0;

//        System.Diagnostics.Stopwatch stopwatch;

//        //##################################################################

//        public Vector3 WorldSize { get { return worldSize; } }
//        public Bool3 RepeatAxes { get { return repeatAxes; } }

//        public GameControl.IGameControllerBase GameController { get { return gameController; } }

//        //##################################################################

//        #region initialization

//        public void InitializeWorldController(GameControl.IGameControllerBase gameController)
//        {
//            isInitialized = true;

//            this.gameController = gameController;
//            this.playerTransform = gameController.PlayerController.PlayerTransform;
//            this.cameraTransform = gameController.CameraController.transform;

//            ChunkSystemData = Resources.Load<ChunkSystemData>("ScriptableObjects/ChunkSystemData");

//            int childCount = transform.childCount;
//            for (int i = 0; i < childCount; i++)
//            {
//                var child = transform.GetChild(i);
//                var region = child.GetComponent<RegionController>();

//                if (region != null)
//                {
//                    regionList.Add(region);
//                    region.InitializeRegion(this);
//                }
//            }

//            //copying
//            CreateCopies();
//        }

//        #endregion initialization

//        //##################################################################

//        #region update

//        void Update()
//        {
//            if (!isInitialized)
//            {
//                return;
//            }

//            //******************************************
//            //updating world

//            //stopwatch = System.Diagnostics.Stopwatch.StartNew();

//            for (int i = 0; i < ChunkSystemData.RegionUpdatesPerFrame; i++)
//            {
//                //var renderersToSwitch = regionList[regionUpdateIndex++].UpdateChunkSystem(playerTransform.position, cameraTransform.position, cameraTransform.forward);
//                //for (int j = 0; j < renderersToSwitch.Count; j++)
//                //{
//                //    rendererQueue.Enqueue(renderersToSwitch[i]);
//                //}

//                //if (regionUpdateIndex == regionList.Count)
//                //{
//                //    regionUpdateIndex = 0;
//                //}
//            }

//            //stopwatch.Stop();
//            //var worldUpdateTime = stopwatch.Elapsed;

//            //******************************************
//            //object activation

//            //stopwatch = System.Diagnostics.Stopwatch.StartNew();

//            int quota = Mathf.Min(ChunkSystemData.ObjectActivationsPerFrame, rendererQueue.Count);
//            Renderer currentRenderer;

//            for (int i = 0; i < quota; i++)
//            {
//                currentRenderer = rendererQueue.Dequeue();

//                currentRenderer.enabled = !currentRenderer.enabled;
//            }

//            //stopwatch.Stop();

//            //var objectActivationTime = stopwatch.Elapsed;

//            //Debug.LogFormat("WorldController: Update: A={0}; B={1}", worldUpdateTime.Milliseconds /*/ 1000f*/, objectActivationTime.Milliseconds /*/ 1000f*/);
//        }

//        #endregion update

//        //##################################################################

//        #region copying

//        void CreateCopies()
//        {
//            var newRegionsList = new List<RegionController>();

//            if (repeatAxes.x || repeatAxes.y || repeatAxes.z)
//            {
//                for (int x = -1 * numberOfRepetitions; x <= numberOfRepetitions; x++)
//                {
//                    if (x != 0 && !repeatAxes.x)
//                    {
//                        continue;
//                    }

//                    for (int y = -1 * numberOfRepetitions; y <= numberOfRepetitions; y++)
//                    {
//                        if (y != 0 && !repeatAxes.y)
//                        {
//                            continue;
//                        }

//                        for (int z = -1 * numberOfRepetitions; z <= numberOfRepetitions; z++)
//                        {
//                            if (z != 0 && !repeatAxes.z)
//                            {
//                                continue;
//                            }
//                            else if (x == 0 && y == 0 && z == 0)
//                            {
//                                continue;
//                            }

//                            var go = new GameObject(string.Format("WorldCopy{0}{1}{2}", x, y, z));
//                            go.transform.parent = transform;

//                            for (int i = 0; i < regionList.Count; i++)
//                            {
//                                var region = regionList[i];

//                                var regionCopyGo = region.CreateCopy(go.transform);
//                                var regionCopyScript = regionCopyGo.GetComponent<RegionController>();

//                                regionCopyScript.InitializeRegion(this);
//                                newRegionsList.Add(regionCopyScript);
//                            }

//                            //moving the copy has to be done after creating all the children
//                            go.transform.Translate(new Vector3(x * worldSize.x, y * worldSize.y, z * worldSize.z));

//                            worldCopies.Add(go);
//                        }
//                    }
//                }
//            }

//            for (int i = 0; i < newRegionsList.Count; i++)
//            {
//                regionList.Add(newRegionsList[i]);
//            }
//        }

//        #endregion copying

//        //##################################################################

//        #region Gizmo

//        [Header("Gizmo"), SerializeField]
//        bool drawGizmo;
//        [SerializeField]
//        Color gizmoColor;

//        void OnDrawGizmos()
//        {
//            //if (this.isInitialized && Application.isEditor && Application.isPlaying)
//            //{
//            //    float size = this.data.GetRenderDistance(eSubChunkLayer.Near_VeryLarge).y * 2;
//            //    var colour = Color.yellow;
//            //    colour.a = 0.3f;
//            //    Gizmos.color = colour;
//            //    Gizmos.DrawCube(this.playerTransform.position, new Vector3(size, size, size));
//            //}

//            if (drawGizmo && Application.isEditor)
//            {
//                Gizmos.color = gizmoColor;
//                //Gizmos.DrawCube(transform.position, worldSize);
//                Gizmos.DrawWireCube(transform.position, worldSize);
//            }
//        }
//        #endregion

//        //##################################################################
//    }
//}