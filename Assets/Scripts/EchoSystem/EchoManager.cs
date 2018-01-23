﻿using System.Collections.Generic;
using UnityEngine;

namespace Game.EchoSystem
{
    public class EchoManager : MonoBehaviour
    {
        //##################################################################

        [SerializeField] Echo echoPrefab;
        public BreakEchoParticles breakEchoParticles;
        [SerializeField] int maxEchoes = 3;
        [SerializeField] float driftInputIntensity = 0.5f;

        [Header("Home")]
        [SerializeField] GameObject homeDoor;
        [SerializeField, Tooltip("Only for GameControllerLite")] Transform homePoint;
        [SerializeField] float timeToHoldForDoor = 1.5f;

		[Header("ShellFX")]
		[SerializeField] GameObject shell;
		Animator playerAnimator;

        /// <summary>
        /// Number of echoes placed by the player.
        /// </summary>
        int placedEchoes;

        Transform playerTransform;
        new PoS_Camera camera;
        EchoCameraEffect echoCamera;
        EchoParticleSystem echoParticles;

        List<Echo> echoList = new List<Echo>();

        bool isEclipseActive;
        bool isDoorActive;
        float driftInputDown;

        Transform MyTransform { get; set; }
        World.SpawnPointSystem.SpawnPointManager spawnPointManager;

        //##################################################################

        #region initialization

        public void InitializeEchoManager(GameControl.IGameControllerBase gameController, World.SpawnPointSystem.SpawnPointManager spawnPointManager = null) {

            camera = gameController.CameraController.PoS_Camera;
            echoCamera = gameController.CameraController.EchoCameraEffect;
            playerTransform = gameController.PlayerController.PlayerTransform;
            echoParticles = playerTransform.GetComponentInChildren<EchoParticleSystem>();

            MyTransform = transform;
            
            this.spawnPointManager = spawnPointManager;
            if (spawnPointManager)
                homePoint = spawnPointManager.GetHomeSpawnTransform();
            homeDoor = Instantiate(homeDoor);
            homeDoor.GetComponentInChildren<HomePortalCamera>().anchorPoint = homePoint;
            homeDoor.SetActive(false);

			playerAnimator = gameController.PlayerController.CharController.animator;

            Utilities.EventManager.EclipseEvent += OnEclipseEventHandler;
            Utilities.EventManager.SceneChangedEvent += OnSceneChangedEventHandler;
        }

        #endregion initialization

        //##################################################################

        void Update() {
            if (!isEclipseActive) {
                float driftInput = Input.GetAxis("Drift") + (Input.GetButtonUp("Drift") ? 1 : 0);

                if (driftInput > driftInputIntensity) {
                    driftInputDown += Time.deltaTime;
                    if (driftInputDown >= timeToHoldForDoor && !isDoorActive) {
                        // do the door thing!
                        isDoorActive = true;

                        if (homePoint) {
                            
                            Vector3 homeDoorPos = new Vector3(0, 0, 0);

                            float minDistance = 1.5f, maxDistance = 4f;

                            RaycastHit hit;
                            if (Physics.Raycast(playerTransform.position, playerTransform.forward * maxDistance, out hit, maxDistance))
                            {
                                homeDoorPos = playerTransform.position + playerTransform.forward * (hit.distance - 0.2f);
                                homeDoor.transform.rotation = playerTransform.rotation;
                            } else
                            {
                                homeDoorPos = playerTransform.position + playerTransform.forward * maxDistance;
                                homeDoor.transform.rotation = playerTransform.rotation;
                            }

                            homeDoor.transform.position = homeDoorPos;
                            homeDoor.transform.rotation = playerTransform.rotation;

                            homeDoor.SetActive(true);
                            camera.LookAtHomeDoor(homeDoor.transform.position, homeDoor.transform.forward, homePoint.position);

                        } else {
                            print("Assign HomePoint to the EchoManager if you want it to work with the GameControllerLite");
                        }
                    }

                } else if (driftInput < 0.4f) {
                    if (isDoorActive) {
                        isDoorActive = false;
                        homeDoor.SetActive(false);
                        camera.StopLookingAtHomeDoor();
                    }
                    else if (driftInputDown > 0)
                        Drift();
                    driftInputDown = 0;
                }

                if (Input.GetButtonDown("Echo"))
                    CreateEcho(true);
            }
        }

        //##################################################################

        #region private methods

        void Drift() {
            if (echoList.Count > 0) {
				CreateShell ();

                echoCamera.SetFov(70, 0.15f, true);

                int lastIndex = echoList.Count - 1;
                var targetEcho = echoList[lastIndex];

                var eventArgs = new Utilities.EventManager.TeleportPlayerEventArgs(targetEcho.MyTransform.position, targetEcho.MyTransform.rotation, false);
                Utilities.EventManager.SendTeleportPlayerEvent(this, eventArgs);

                Utilities.EventManager.SendEchoDestroyedEvent(this);
                Break(targetEcho);
            }
        }
        
        public void Break(Echo target) {
            if (echoList.Contains(target)) {
                echoList.Remove(target);
            }
            if (target.playerEcho) {
                placedEchoes--;
                echoParticles.SetEchoNumber(maxEchoes - placedEchoes);
            }
            Instantiate(breakEchoParticles, target.MyTransform.position, target.MyTransform.rotation);
            Destroy(target.gameObject);
        }

        public void BreakAll() {
            Echo[] killList = echoList.ToArray();
            for (int i = 0; i < killList.Length; i++)
                Break(killList[i]);
        }

        public void CreateEcho(bool isPlayerEcho) {
            if (isEclipseActive)
                return;

            if (placedEchoes == maxEchoes) {
                int i = 0;
                var oldestEcho = echoList[i];

                while(!oldestEcho.playerEcho) {
                    i++;
                    oldestEcho = echoList[i];
                }
                Break(oldestEcho);
            }

            Echo newEcho = Instantiate(echoPrefab, playerTransform.position, Quaternion.identity);
            newEcho.playerEcho = isPlayerEcho;
            newEcho.echoManager = this;
            echoList.Add(newEcho);

            if (isPlayerEcho) {
                placedEchoes++;
                echoParticles.SetEchoNumber(maxEchoes - placedEchoes);
            }
        }

        void FreezeAll() {
            for (int i = 0; i < echoList.Count; i++)
                echoList[i].Freeze();
        }

        void UnfreezeAll() {
            for (int i = 0; i < echoList.Count; i++)
                echoList[i].Unfreeze();
        }

		void CreateShell()
		{
			GameObject _shell;
			_shell = Instantiate (shell, playerTransform.position - new Vector3 (0,-0.2f,0), playerTransform.rotation) as GameObject;
			//_shell.GetComponent<Animator> ().runtimeAnimatorController = playerAnimator.runtimeAnimatorController;
			Animator _anim = _shell.GetComponent<Animator> ();
			_anim.Play ("Start Run", 0, 0.5f);
			_anim.SetFloat ("Speed", 1);
			_anim.speed = 0;
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

            placedEchoes = 0;
            echoParticles.SetEchoNumber(maxEchoes);
            echoList.Clear();
        }

        #endregion event handlers

        //##################################################################
    }
} //end of namespace