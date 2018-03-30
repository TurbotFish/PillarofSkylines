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
        [SerializeField] private float driftInputIntensity = 0.5f;

        [Header("ShellFX")]
        [SerializeField] private GameObject shell;

        private Animator playerAnimator;

        private IGameControllerBase gameController;
        private EchoCameraEffect echoCamera;
        private EchoParticleSystem echoParticles;

        private List<Echo> echoList = new List<Echo>();
        private int placedEchoes; // Number of echoes placed by the player
        private bool isEclipseActive;
        private bool isDoorActive;
        private float driftInputDown;
        private bool isActive; //set to true when a scene is loaded, false otherwise. This helps avoid errors ;)

        //##################################################################

        #region initialization

        public void Initialize(IGameControllerBase gameController)
        {
            this.gameController = gameController;
            echoCamera = gameController.CameraController.EchoCameraEffect;
            playerAnimator = gameController.PlayerController.CharController.animator;
            echoParticles = gameController.PlayerController.PlayerTransform.GetComponentInChildren<EchoParticleSystem>();
            echoParticles.numEchoes = 3;

            EventManager.EclipseEvent += OnEclipseEventHandler;
            EventManager.PreSceneChangeEvent += OnPreSceneChangeEvent;
            EventManager.SceneChangedEvent += OnSceneChangedEventHandler;
        }       

        #endregion initialization

        //##################################################################

        void Update()
        {
            if (isActive && !isEclipseActive)
            {
                //drift stuff (???)
                float driftInput = Input.GetAxis("Drift") + (Input.GetButtonDown("Drift") ? 1 : 0);

                if (driftInput > driftInputIntensity)
                {
                    if (driftInputDown == 0)
                    {
                        Drift();
                    }
                    driftInputDown += Time.deltaTime;
                }
                else if (driftInput < driftInputIntensity - 0.1f)
                {
                    if (driftInputDown > 0)
                    driftInputDown = 0;
                }

                //create new echo
                if (Input.GetButtonDown("Echo") && gameController.PlayerModel.CheckAbilityActive(eAbilityType.Echo))
                {
                    CreateEcho(true);
                }
            }
        }

        //##################################################################

        #region private methods

        void Drift()
        {
            if (echoList.Count > 0)
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

        public void CreateEcho(bool isPlayerEcho)
        {
            if (isEclipseActive)
                return;

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
            GameObject _shell;
            _shell = Instantiate(shell, gameController.PlayerController.PlayerTransform.position - new Vector3(0, -0.2f, 0), gameController.PlayerController.PlayerTransform.rotation) as GameObject;
            //_shell.GetComponent<Animator> ().runtimeAnimatorController = playerAnimator.runtimeAnimatorController;
            Animator _anim = _shell.GetComponent<Animator>();
            _anim.Play(playerAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash);
            int i = 0;
            foreach (var param in playerAnimator.parameters)
            {
                _anim.parameters[i] = playerAnimator.parameters[i];
            }
            _anim.speed = 0;
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
        }

        void OnSceneChangedEventHandler(object sender, EventManager.SceneChangedEventArgs args)
        {
            isActive = true;
        }

        #endregion event handlers

        //##################################################################
    }
} //end of namespace