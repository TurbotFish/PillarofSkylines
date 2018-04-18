using Game.GameControl;
using Game.Model;
using Game.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Game.EchoSystem
{
    public class EchoManager : MonoBehaviour
    {
        //##################################################################

        [SerializeField] private Echo echoPrefab;
        [SerializeField] public BreakEchoParticles breakEchoParticles; //why public?
        [SerializeField] private int maxEchoes = 3;

        [Header("ShellFX")]
        [SerializeField] private GameObject shell;
        public bool useShells = false;

        private Animator playerAnimator;

        private IGameControllerBase gameController;
        public Player.CharacterController.CharController charController;
        private EchoCameraEffect echoCamera;
        private EchoParticleSystem echoParticles;

        private List<Echo> echoList = new List<Echo>();
        private int placedEchoes; // Number of echoes placed by the player
        private bool isEclipseActive;
        private bool isDoorActive;
        private float driftInputDown;
        private bool isActive; //set to true when a scene is loaded, false otherwise. This helps avoid errors ;)

        private Vector3 wayPointPosition;
        private bool hasWaypoint = false;

        //##################################################################

        #region initialization

        public void Initialize(IGameControllerBase gameController)
        {
            this.gameController = gameController;
            echoCamera = gameController.CameraController.EchoCameraEffect;
            playerAnimator = gameController.PlayerController.CharController.animator;
            echoParticles = gameController.PlayerController.PlayerTransform.GetComponentInChildren<EchoParticleSystem>();
            echoParticles.numEchoes = 3;

            charController = gameController.PlayerController.CharController;

            EventManager.EclipseEvent += OnEclipseEventHandler;
            EventManager.PreSceneChangeEvent += OnPreSceneChangeEvent;
            EventManager.SceneChangedEvent += OnSceneChangedEventHandler;
            EventManager.SetWaypointEvent += OnSetWaypointEventHandler;
        }

        #endregion initialization

        //##################################################################

        void Update()
        {
            if (isActive && !isEclipseActive)
            {
                //drift stuff (???)
                //bool driftInput = Input.GetButtonDown("Drift");

                //if (driftInput)
                //    Drift();

                //create new echo
                //if (charController.InputInfo.echoButtonUp && charController.InputInfo.echoButtonTimePressed < 1f && gameController.PlayerModel.CheckAbilityActive(eAbilityType.Echo))
                //{
                //    CreateEcho(true);
                //}
            }
        }

        //##################################################################

        #region private methods

        public void Drift()
        {
            if (isEclipseActive && !gameController.PlayerModel.hasNeedle)
            {
                return;
            }

            if (!isEclipseActive && echoList.Count > 0)
            {
                CreateShell();

                echoCamera.SetFov(70, 0.15f, true);

                int lastIndex = echoList.Count - 1;
                var targetEcho = echoList[lastIndex];

                var eventArgs = new EventManager.TeleportPlayerEventArgs(targetEcho.MyTransform.position, false);
                EventManager.SendTeleportPlayerEvent(this, eventArgs);

                EventManager.SendEchoDestroyedEvent(this);
                Break(targetEcho);

            }
            else if (hasWaypoint)
            {
                CreateShell();

                echoCamera.SetFov(70, 0.15f, true);

                var eventArgs = new EventManager.TeleportPlayerEventArgs(wayPointPosition, false);
                EventManager.SendTeleportPlayerEvent(this, eventArgs);
            }
        }

        public void Break(Echo target)
        {
            int index = 0;
            if (echoList.Contains(target))
            {
                index = echoList.IndexOf(target);
                echoList.Remove(target);
            }
            if (target.playerEcho)
            {
                placedEchoes--;
                echoParticles.RemoveEcho(index);
            }
            Instantiate(breakEchoParticles, target.MyTransform.position, target.MyTransform.rotation);
            Destroy(target.gameObject);
        }

        public void BreakAll()
        {
            Echo[] killList = echoList.ToArray();
            for (int i = 0; i < killList.Length; i++)
                Break(killList[i]);
        }

        public Echo CreateEcho(bool isPlayerEcho)
        {
            if (isEclipseActive || charController.createdEchoOnThisInput)
                return null;

            if (placedEchoes == maxEchoes)
            {
                int i = 0;
                var oldestEcho = echoList[i];

                while (!oldestEcho.playerEcho)
                {
                    i++;
                    oldestEcho = echoList[i];
                }
                Break(oldestEcho);
            }

            Echo newEcho = Instantiate(echoPrefab, gameController.PlayerController.PlayerTransform.position, gameController.PlayerController.PlayerTransform.rotation);
            newEcho.playerEcho = isPlayerEcho;
            newEcho.echoManager = this;
            echoList.Add(newEcho);

            if (isPlayerEcho)
            {
                placedEchoes++;
                echoParticles.AddEcho(newEcho.transform.position);
            }
            charController.createdEchoOnThisInput = true;
            return newEcho;
        }

        public Echo CreateEcho(bool isPlayerEcho, Vector3 position)
        {
            if (isEclipseActive || charController.createdEchoOnThisInput)
                return null;

            if (placedEchoes == maxEchoes)
            {
                int i = 0;
                var oldestEcho = echoList[i];

                while (!oldestEcho.playerEcho)
                {
                    i++;
                    oldestEcho = echoList[i];
                }
                Break(oldestEcho);
            }

            Echo newEcho = Instantiate(echoPrefab, position, gameController.PlayerController.PlayerTransform.rotation);
            newEcho.playerEcho = isPlayerEcho;
            newEcho.echoManager = this;
            echoList.Add(newEcho);

            if (isPlayerEcho)
            {
                placedEchoes++;
                echoParticles.AddEcho(newEcho.transform.position);
            }
            charController.createdEchoOnThisInput = true;
            return newEcho;
        }

        void FreezeAll()
        {
            for (int i = 0; i < echoList.Count; i++)
                echoList[i].Freeze();
        }

        void UnfreezeAll()
        {
            for (int i = 0; i < echoList.Count; i++)
                echoList[i].Unfreeze();
        }

        void CreateShell()
        {
            if (useShells)
            {
                GameObject _shell;
                _shell = Instantiate(shell, gameController.PlayerController.PlayerTransform.position, gameController.PlayerController.PlayerTransform.rotation) as GameObject;
                //_shell.GetComponent<Animator> ().runtimeAnimatorController = playerAnimator.runtimeAnimatorController;
                Animator _anim = _shell.GetComponent<Animator>();
                Debug.Log("player animator : " + playerAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash);
                _anim.Play(playerAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                int i = 0;
                foreach (var param in playerAnimator.parameters)
                {
                    _anim.parameters[i] = playerAnimator.parameters[i];
                }
                _anim.speed = 0;
            }

        }


        #endregion private methods

        //##################################################################

        #region event handlers

        void OnEclipseEventHandler(object sender, EventManager.EclipseEventArgs args)
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

        private void OnPreSceneChangeEvent(object sender, EventManager.PreSceneChangeEventArgs args)
        {
            isActive = false;

            Debug.LogFormat("EchoManager: OnPreSceneChangedEventHandler: echo count = {0}", echoList.Count);

            isEclipseActive = false;

            for (int i = 0; i < echoList.Count; i++)
            {
                if (echoList[i] == null)
                {
                    Debug.Log("echo is null");
                }
                Destroy(echoList[i].gameObject);
            }

            placedEchoes = 0;
            echoParticles.RemoveAllEcho();
            echoList.Clear();

            hasWaypoint = false;
        }

        void OnSceneChangedEventHandler(object sender, EventManager.SceneChangedEventArgs args)
        {
            isActive = true;
        }

        void OnSetWaypointEventHandler(object sender, EventManager.SetWaypointEventArgs args)
        {
            hasWaypoint = true;
            wayPointPosition = args.Position;
        }

        #endregion event handlers

        //##################################################################
    }
} //end of namespace