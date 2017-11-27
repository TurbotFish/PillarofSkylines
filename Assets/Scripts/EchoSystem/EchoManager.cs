using System.Collections.Generic;
using UnityEngine;

namespace Game.EchoSystem
{
    public class EchoManager : MonoBehaviour
    {
        [SerializeField]
        Echo echoPrefab;
        [SerializeField]
        BreakEchoParticles breakEchoParticles;

        public List<Echo> echoes;
        [SerializeField]
        int maxEchoes = 3;

        public List<Echo> nonEchoes; // For echoes that were not created by the player

        [HideInInspector]
        public Transform pool;
        Transform playerTransform;
        EchoCameraEffect echoCamera;

        List<Echo> echoList = new List<Echo>();

        bool isEclipseActive;

        Transform MyTransform { get; set; }

        //##################################################################

        #region initialization

        public void InitializeEchoManager(GameControl.IGameControllerBase gameController)
        {
            echoCamera = gameController.CameraController.EchoCameraEffect;
            playerTransform = gameController.PlayerController.Player.transform;

            MyTransform = transform;

            pool = new GameObject("Echo Pool").transform;
            pool.SetParent(transform);

            Utilities.EventManager.OnEclipseEvent += OnEclipseEventHandler;
            Utilities.EventManager.OnSceneChangedEvent += OnSceneChangedEventHandler;
        }

        #endregion initialization

        //##################################################################

        void Update()
        {
            if (!isEclipseActive)
            {
                if (Input.GetButtonDown("Drift"))
                    Drift();
                if (Input.GetButtonDown("Echo"))
                    CreateEcho();
            }
        }

        //##################################################################

        #region private methods

        void Drift()
        {
            //Debug.LogErrorFormat("EchoManager: Drift: echo count = {0}", echoList.Count);

            if (echoList.Count > 0)
            {
                echoCamera.SetFov(70, 0.15f, true);

                var targetEcho = echoList[echoList.Count - 1];
                echoList.Remove(targetEcho);

                var eventArgs = new Utilities.EventManager.OnTeleportPlayerEventArgs(targetEcho.MyTransform.position, false);
                Utilities.EventManager.SendTeleportPlayerEvent(this, eventArgs);

                Instantiate(breakEchoParticles, targetEcho.MyTransform.position, targetEcho.MyTransform.rotation);

                Destroy(targetEcho.gameObject);
            }

            //**
            //if (echoes.Count > 0)
            //{
            //    echoCamera.SetFov(70, 0.15f, true);

            //    Echo targetEcho = echoes[echoes.Count - 1];

            //    playerTransform.position = targetEcho.transform.position; // We should reference Player and move this script in a Manager object

            //    if (!targetEcho.isActive)
            //    {
            //        targetEcho.Break();
            //    }
            //}
        }

        void CreateEcho()
        {
            if (echoList.Count >= maxEchoes || isEclipseActive)
            {
                return;
            }

            var newEcho = Instantiate(echoPrefab, playerTransform.position, playerTransform.rotation);
            echoList.Add(newEcho);

            //**
            //Echo newEcho = InstantiateFromPool(echoPrefab, playerTransform.position);
            //newEcho.playerEcho = true;
            //echoes.Add(newEcho);
            //if (echoes.Count > maxEchoes)
            //    echoes[0].Break();
        }

        void FreezeAll()
        {
            for (int i = 0; i < echoList.Count; i++)
            {
                echoList[i].Freeze();
            }

            //**
            //for (int i = 0; i < echoes.Count; i++)
            //{
            //    echoes[i].Freeze();
            //}

            //for (int i = 0; i < nonEchoes.Count; i++)
            //{
            //    nonEchoes[i].Freeze();
            //}
        }

        void UnfreezeAll()
        {
            for (int i = 0; i < echoList.Count; i++)
            {
                echoList[i].Unfreeze();
            }

            //**
            //for (int i = 0; i < echoes.Count; i++)
            //{
            //    echoes[i].Unfreeze();
            //}

            //for (int i = 0; i < nonEchoes.Count; i++)
            //{
            //    nonEchoes[i].Unfreeze();
            //}
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

        #endregion private methods

        //##################################################################

        #region event handlers

        void OnEclipseEventHandler(object sender, Utilities.EventManager.OnEclipseEventArgs args)
        {
            if (args.EclipseOn)
            {
                isEclipseActive = true;

                FreezeAll();
            }
            else
            {
                isEclipseActive = false;

                UnfreezeAll();
            }
        }

        void OnSceneChangedEventHandler(object sender, Utilities.EventManager.OnSceneChangedEventArgs args)
        {
            //Debug.LogErrorFormat("EchoManager: OnSceneChangedEventHandler: echo count = {0}", echoList.Count);

            isEclipseActive = false;

            for (int i = 0; i < echoList.Count; i++)
            {
                Destroy(echoList[i].gameObject);
            }

            echoList.Clear();
        }

        #endregion event handlers

        //##################################################################
    }
} //end of namespace