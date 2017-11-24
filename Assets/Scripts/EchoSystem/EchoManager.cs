using System.Collections.Generic;
using UnityEngine;

namespace Game.EchoSystem
{
    public class EchoManager : MonoBehaviour
    {
        public bool demo;
        public Vector3 demoVelocity;

        [SerializeField]
        Echo echoPrefab;
        [SerializeField]
        BreakEchoParticles breakEchoParticles;

        //[TestButton("Freeze All Echoes", "FreezeAll", isActiveInEditor = false)]
        //[TestButton("Unfreeze All Echoes", "UnfreezeAll", isActiveInEditor = false)]

        public List<Echo> echoes;
        [SerializeField]
        int maxEchoes = 3;

        public List<Echo> nonEchoes; // For echoes that were not created by the player

        [HideInInspector]
        public Transform pool;
        Transform playerTransform;
        new EchoCameraEffect camera;

        bool eclipse;

        //##################################################################

        #region Singleton
        //public static EchoManager instance;
        //void Awake()
        //{
        //    if (!instance)
        //    {
        //        instance = this;
        //    }
        //    else if (instance != this)
        //        Destroy(gameObject);
        //}
        #endregion

        //##################################################################
        //##################################################################

        #region initialization

        public void InitializeEchoManager(GameControl.IGameControllerBase gameController)
        {
            this.camera = gameController.CameraController.EchoCameraEffect;
            this.playerTransform = gameController.PlayerController.Player.transform;

            this.pool = new GameObject("Echo Pool").transform;
            this.pool.SetParent(this.transform);

            Utilities.EventManager.OnEclipseEvent += OnEclipseEventHandler;
        }

        #endregion initialization

        //##################################################################
        //##################################################################

        void Update()
        {
            if (!eclipse)
            {
                if (Input.GetButtonDown("Drift"))
                    Drift();
                if (Input.GetButtonDown("Echo"))
                    CreateEcho();
            }
        }

        //##################################################################

        void Drift()
        {
            if (echoes.Count > 0)
            {
                camera.SetFov(70, 0.15f, true);
                Echo targetEcho = echoes[echoes.Count - 1];
                playerTransform.position = targetEcho.transform.position; // We should reference Player and move this script in a Manager object
                if (!targetEcho.isActive)
                    targetEcho.Break();

                if (demo)
                {
                    GetComponent<Rigidbody>().velocity = demoVelocity;
                }
            }
        }

        void CreateEcho()
        {
            Echo newEcho = InstantiateFromPool(echoPrefab, playerTransform.position);
            newEcho.playerEcho = true;
            echoes.Add(newEcho);
            if (echoes.Count > maxEchoes)
                echoes[0].Break();
        }

        void FreezeAll()
        {
            for (int i = 0; i < echoes.Count; i++)
            {
                echoes[i].Freeze();
            }

            for (int i = 0; i < nonEchoes.Count; i++)
            {
                nonEchoes[i].Freeze();
            }
        }

        void UnfreezeAll()
        {
            for (int i = 0; i < echoes.Count; i++)
            {
                echoes[i].Unfreeze();
            }

            for (int i = 0; i < nonEchoes.Count; i++)
            {
                nonEchoes[i].Unfreeze();
            }
        }

        //public void BreakAll()
        //{
        //    for (int i = echoes.Count - 1; i >= 0; i--)
        //        echoes[i].Break();
        //}

        T InstantiateFromPool<T>(T prefab, Vector3 position) where T : MonoBehaviour
        {
            T poolObject = pool.GetComponentInChildren<T>();

            if (poolObject != null)
            {
                poolObject.transform.parent = null;
                poolObject.transform.position = position;
                poolObject.gameObject.SetActive(true);

            }
            else
                poolObject = Instantiate(prefab, position, Quaternion.identity);
            return poolObject;
        }

        public void BreakParticles(Vector3 position)
        {
            InstantiateFromPool(breakEchoParticles, position);
        }

        //##################################################################

        void OnEclipseEventHandler(object sender, Utilities.EventManager.OnEclipseEventArgs args)
        {
            if (args.EclipseOn)
            {
                eclipse = true;

                FreezeAll();
            }
            else
            {
                eclipse = false;

                UnfreezeAll();
            }
        }

        void OnSceneChangedEventHandler(object sender, Utilities.EventManager.OnSceneChangedEventArgs args)
        {
            eclipse = false;

            for(int i = 0; i < echoes.Count; i++)
            {
                echoes[i].Break();
            }
        }

        //##################################################################
    }
} //end of namespace