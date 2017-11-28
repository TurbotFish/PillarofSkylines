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

        [SerializeField]
        int maxEchoes = 3;

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

            Utilities.EventManager.EclipseEvent += OnEclipseEventHandler;
            Utilities.EventManager.SceneChangedEvent += OnSceneChangedEventHandler;
        }

        #endregion initialization

        //##################################################################

        void Update()
        {
            if (!isEclipseActive)
            {
                if (Input.GetButtonDown("Drift"))
                {
                    Drift();
                }

                if (Input.GetButtonDown("Echo"))
                {
                    CreateEcho();
                }
            }
        }

        //##################################################################

        #region private methods

        void Drift()
        {
            if (echoList.Count > 0)
            {
                echoCamera.SetFov(70, 0.15f, true);

                var targetEcho = echoList[echoList.Count - 1];
                echoList.Remove(targetEcho);

                var eventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(targetEcho.MyTransform.position, targetEcho.MyTransform.rotation, false);
                Utilities.EventManager.SendTeleportPlayerEvent(this, eventArgs);

                Instantiate(breakEchoParticles, targetEcho.MyTransform.position, targetEcho.MyTransform.rotation);

                Destroy(targetEcho.gameObject);
            }
        }

        void CreateEcho()
        {
            if (isEclipseActive)
            {
                return;
            }

            if (echoList.Count == maxEchoes)
            {
                var oldestEcho = echoList[0];
                Destroy(oldestEcho.gameObject);
                echoList.RemoveAt(0);
            }

            var newEcho = Instantiate(echoPrefab, playerTransform.position, playerTransform.rotation);
            echoList.Add(newEcho);
        }

        void FreezeAll()
        {
            for (int i = 0; i < echoList.Count; i++)
            {
                echoList[i].Freeze();
            }
        }

        void UnfreezeAll()
        {
            for (int i = 0; i < echoList.Count; i++)
            {
                echoList[i].Unfreeze();
            }
        }

        #endregion private methods

        //##################################################################

        #region event handlers

        void OnEclipseEventHandler(object sender, Utilities.EventManager.EclipseEventArgs args)
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

        void OnSceneChangedEventHandler(object sender, Utilities.EventManager.SceneChangedEventArgs args)
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