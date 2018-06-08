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

        // ATTRIBUTES

        [SerializeField] private Echo echoPrefab;
        [SerializeField] public BreakEchoParticles breakEchoParticles; //why public?
        [SerializeField] private int maxEchoes = 3;

        [Header("ShellFX")]
        [SerializeField] private GameObject shell;
        public bool useShells = false;

        private Animator playerAnimator;

        private GameController gameController;
        public Player.CharacterController.CharController charController;
        private EchoCameraEffect echoCamera;
        private EchoParticleSystem echoParticles;

        private List<Echo> echoList = new List<Echo>();
        private int placedEchoes; // Number of echoes placed by the player
        private bool isEclipseActive;
        private bool isDoorActive;
        private float driftInputDown;

        private bool hasWaypoint = false;
        private IWaypoint Waypoint;

        [Header("Sound")]
        public AudioClip holdClip;
        [Range(0, 2)] public float volumeHold = 1f;
        public bool addRandomisationHold = false;
        public AudioClip dropClip;
        [Range(0, 2)] public float volumeDrop = 1f;
        public bool addRandomisationDrop = false;
        public float minDistance = 10f;
        public float maxDistance = 50f;
        public float clipDuration = 0f;
        GameObject CurrentHoldSoundObject;

        //##################################################################

        // INITIALIZATION

        public void Initialize(GameController gameController)
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
        }

        //##################################################################

        // INQUIRIES

        /// <summary>
        /// Returns a list with the position of all echos.
        /// </summary>
        /// <returns></returns>
        public List<Vector3> GetEchoPositions()
        {
            var result = new List<Vector3>();

            foreach (var echo in echoList)
            {
                result.Add(echo.MyTransform.position);
            }

            return result;
        }

        //##################################################################

        // OPERATIONS

        public void Drift()
        {
            if (isEclipseActive && !gameController.PlayerModel.PlayerHasNeedle)
            {
                return;
            }

            if (!isEclipseActive && echoList.Count > 0)
            {
                CreateShell();
                
                echoCamera.SetFov(70, 0.15f, true);

                int lastIndex = echoList.Count - 1;
                var targetEcho = echoList[lastIndex];

                var eventArgs = new EventManager.TeleportPlayerEventArgs(targetEcho.MyTransform.position, true, false);
                EventManager.SendTeleportPlayerEvent(this, eventArgs);

                EventManager.SendEchoDestroyedEvent(this);
                Break(targetEcho);

            }
            else if (hasWaypoint)
            {
                CreateShell();
                
                echoCamera.SetFov(70, 0.15f, true);

                var eventArgs = new EventManager.TeleportPlayerEventArgs(Waypoint.Position, false);
                EventManager.SendTeleportPlayerEvent(this, eventArgs);

                if (gameController.PlayerModel.PlayerHasNeedle)
                    (Waypoint as LevelElements.NeedleSlot).OnInteraction();
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
            if (isEclipseActive)
            {
                return null;
            }

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

            CurrentHoldSoundObject = SoundifierOfTheWorld.PlaySoundAtLocation(holdClip, newEcho.transform, maxDistance, volumeHold, minDistance, clipDuration, addRandomisationHold, true, 0f);

            charController.fxManager.EchoPlay();

            if (isPlayerEcho)
            {
                placedEchoes++;
                echoParticles.AddEcho(newEcho.transform.position);
            }
            return newEcho;
        }

        public Echo CreateEcho(bool isPlayerEcho, Vector3 position)
        {
            if (isEclipseActive)
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

            CurrentHoldSoundObject = SoundifierOfTheWorld.PlaySoundAtLocation(holdClip, newEcho.transform, maxDistance, volumeHold, minDistance, clipDuration, addRandomisationHold, true, 0f);

            charController.fxManager.EchoPlay();

            if (isPlayerEcho)
            {
                placedEchoes++;
                echoParticles.AddEcho(newEcho.transform.position);
            }
            return newEcho;
        }

        public void PlaceEcho(Transform echoTransform)
        {
            Destroy(CurrentHoldSoundObject);
            SoundifierOfTheWorld.PlaySoundAtLocation(dropClip, echoTransform, maxDistance, volumeDrop, minDistance, clipDuration, addRandomisationDrop, false, 0.2f);

            charController.fxManager.EchoStop();
            echoList[echoList.Count - 1].StartParticles();
        }

        /// <summary>
        /// Sets the current waypoint.
        /// <param name="waypoint"></param>
        public void SetWaypoint(IWaypoint waypoint)
        {
            if (waypoint == null)
            {
                Debug.LogError("EchoManager: SetWaypoint: waypoint is null!");
                return;
            }

            if (Waypoint != null && Waypoint != waypoint)
            {
                Waypoint.OnWaypointRemoved();
            }

            hasWaypoint = true;
            Waypoint = waypoint;
        }

        /// <summary>
        /// Removes the current waypoint. If "waypoint_id" is set the waypoint will only be removed if it has this id.
        /// </summary>
        /// <param name="waypoint_id"></param>
        public void RemoveWaypoint(string waypoint_id = "")
        {
            if (Waypoint == null)
            {
                return;
            }

            if (waypoint_id == "" || Waypoint.UniqueId == waypoint_id)
            {
                Waypoint.OnWaypointRemoved();
                Waypoint = null;
                hasWaypoint = false;
            }
        }

        private void FreezeAll()
        {
            for (int i = 0; i < echoList.Count; i++)
                echoList[i].Freeze();
        }

        private void UnfreezeAll()
        {
            for (int i = 0; i < echoList.Count; i++)
                echoList[i].Unfreeze();
        }

        private void CreateShell()
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

        private void OnEclipseEventHandler(object sender, EventManager.EclipseEventArgs args)
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

            RemoveWaypoint();
        }

        private void OnSceneChangedEventHandler(object sender, EventManager.SceneChangedEventArgs args)
        {
        }
    }
} //end of namespace