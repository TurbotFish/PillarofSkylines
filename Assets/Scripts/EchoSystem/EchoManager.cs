using Game.GameControl;
using Game.Model;
using Game.Player.CharacterController;
using Game.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Game.EchoSystem
{
    public class EchoManager : MonoBehaviour
    {
        //##################################################################

        // -- CONSTANTS

        [SerializeField] private Echo echoPrefab;
        [SerializeField] private BreakEchoParticles breakEchoParticles;
        [SerializeField] private int maxEchoes = 3;

        [Header("ShellFX")]
        [SerializeField] private GameObject shell;
        [SerializeField] private bool useShells = false;

        [Header("Sound")]
        [SerializeField] private AudioClip holdClip;
        [SerializeField, Range(0, 2)] private float volumeHold = 1f;
        [SerializeField] private bool addRandomisationHold = false;
        [SerializeField] private AudioClip dropClip;
        [SerializeField, Range(0, 2)] private float volumeDrop = 1f;
        [SerializeField] private bool addRandomisationDrop = false;
        [SerializeField] private AudioClip breakClip;
        [SerializeField, Range(0, 2)] public float volumeBreak = 1f;
        [SerializeField] private bool addRandomisationBreak = false;
        [SerializeField] private AudioClip driftClip;
        [SerializeField, Range(0, 2)] public float volumeDrift = 1f;
        [SerializeField] private bool addRandomisationDrift = false;
        [SerializeField] private float minDistance = 10f;
        [SerializeField] private float maxDistance = 50f;
        [SerializeField] private float clipDuration = 0f;

        //##################################################################

        // ATTRIBUTES

        private Animator PlayerAnimator;
        private GameController GameController;
        private CharController CharController;
        private EchoCameraEffect EchoCameraEffect;
        private EchoParticleSystem EchoParticleSystem;

        private List<Echo> EchoList = new List<Echo>();
        private int PlacedEchoesCount; // Number of echoes placed by the player
        private bool IsEclipseActive;
        private bool IsDoorActive;
        private float DriftInputDown;

        private bool HasWaypoint = false;
        private IWaypoint Waypoint;

        private GameObject CurrentHoldSoundObject;

        //##################################################################

        // INITIALIZATION

        public void Initialize(GameController gameController)
        {
            GameController = gameController;
            EchoCameraEffect = gameController.CameraController.EchoCameraEffect;
            PlayerAnimator = gameController.PlayerController.CharController.animator;
            EchoParticleSystem = gameController.PlayerController.Transform.GetComponentInChildren<EchoParticleSystem>();
            EchoParticleSystem.numEchoes = 3;

            CharController = gameController.PlayerController.CharController;

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

            foreach (var echo in EchoList)
            {
                result.Add(echo.MyTransform.position);
            }

            return result;
        }

        //##################################################################

        // OPERATIONS

        public void Drift()
        {
            if (IsEclipseActive && !GameController.PlayerModel.PlayerHasNeedle)
            {
                return;
            }

            if (!IsEclipseActive && EchoList.Count > 0)
            {
                CreateShell();

                EchoCameraEffect.Play();

                int lastIndex = EchoList.Count - 1;
                var targetEcho = EchoList[lastIndex];

                SoundifierOfTheWorld.PlaySoundAtLocation(driftClip, GameController.PlayerController.Transform, maxDistance, volumeDrift, minDistance, clipDuration, addRandomisationDrift, false, .2f);

                var eventArgs = new EventManager.TeleportPlayerEventArgs(
                    GameController.PlayerController.Transform.position,
                    targetEcho.MyTransform.position)
                {
                    Drifting = true
                };
                EventManager.SendTeleportPlayerEvent(this, eventArgs);

                EventManager.SendEchoDestroyedEvent(this);
                Break(targetEcho);

            }
            else if (HasWaypoint)
            {
                CreateShell();

                EchoCameraEffect.Play();

                SoundifierOfTheWorld.PlaySoundAtLocation(driftClip, GameController.PlayerController.Transform, maxDistance, volumeDrift, minDistance, clipDuration, addRandomisationDrift, false, .2f);

                var eventArgs = new EventManager.TeleportPlayerEventArgs(GameController.PlayerController.Transform.position, Waypoint.Position)
                {
                    UseCameraAngle = Waypoint.UseCameraAngle,
                    CameraAngle = Waypoint.CameraAngle
                };
                EventManager.SendTeleportPlayerEvent(this, eventArgs);

                if (GameController.PlayerModel.PlayerHasNeedle)
                    (Waypoint as LevelElements.NeedleSlot).OnInteraction();
            }
        }

        public void Break(Echo target)
        {
            int index = 0;
            if (EchoList.Contains(target))
            {
                index = EchoList.IndexOf(target);
                EchoList.Remove(target);
            }
            if (target.playerEcho)
            {
                PlacedEchoesCount--;
                EchoParticleSystem.RemoveEcho(index);
            }
            Transform particles = Instantiate(breakEchoParticles, target.MyTransform.position, target.MyTransform.rotation).transform;
            SoundifierOfTheWorld.PlaySoundAtLocation(breakClip, particles, maxDistance, volumeBreak, minDistance, clipDuration, addRandomisationBreak, false, .2f);
            Destroy(target.gameObject);
        }

        public void BreakAll()
        {
            Echo[] killList = EchoList.ToArray();
            for (int i = 0; i < killList.Length; i++)
                Break(killList[i]);
        }

        public Echo CreateEcho(bool isPlayerEcho)
        {
            if (IsEclipseActive)
            {
                return null;
            }

            if (PlacedEchoesCount == maxEchoes)
            {
                int i = 0;
                var oldestEcho = EchoList[i];

                while (!oldestEcho.playerEcho)
                {
                    i++;
                    oldestEcho = EchoList[i];
                }
                Break(oldestEcho);
            }

            Echo newEcho = Instantiate(echoPrefab, GameController.PlayerController.Transform.position, GameController.PlayerController.Transform.rotation);
            newEcho.playerEcho = isPlayerEcho;
            newEcho.echoManager = this;
            EchoList.Add(newEcho);

            CurrentHoldSoundObject = SoundifierOfTheWorld.PlaySoundAtLocation(holdClip, newEcho.transform, maxDistance, volumeHold, minDistance, clipDuration, addRandomisationHold, true, 0f);

            CharController.fxManager.EchoPlay();

            if (isPlayerEcho)
            {
                PlacedEchoesCount++;
                EchoParticleSystem.AddEcho(newEcho.transform.position);
            }
            return newEcho;
        }

        public Echo CreateEcho(bool isPlayerEcho, Vector3 position)
        {
            if (IsEclipseActive)
                return null;

            if (PlacedEchoesCount == maxEchoes)
            {
                int i = 0;
                var oldestEcho = EchoList[i];

                while (!oldestEcho.playerEcho)
                {
                    i++;
                    oldestEcho = EchoList[i];
                }
                Break(oldestEcho);
            }

            Echo newEcho = Instantiate(echoPrefab, position, GameController.PlayerController.Transform.rotation);
            newEcho.playerEcho = isPlayerEcho;
            newEcho.echoManager = this;
            EchoList.Add(newEcho);

            CurrentHoldSoundObject = SoundifierOfTheWorld.PlaySoundAtLocation(holdClip, newEcho.transform, maxDistance, volumeHold, minDistance, clipDuration, addRandomisationHold, true, 0f);

            CharController.fxManager.EchoPlay();

            if (isPlayerEcho)
            {
                PlacedEchoesCount++;
                EchoParticleSystem.AddEcho(newEcho.transform.position);
            }
            return newEcho;
        }

        public void PlaceEcho(Transform echoTransform)
        {
            Destroy(CurrentHoldSoundObject);
            SoundifierOfTheWorld.PlaySoundAtLocation(dropClip, echoTransform, maxDistance, volumeDrop, minDistance, clipDuration, addRandomisationDrop, false, 0.2f);

            CharController.fxManager.EchoStop();
            EchoList[EchoList.Count - 1].StartParticles();
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

            HasWaypoint = true;
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
                HasWaypoint = false;
            }
        }

        private void FreezeAll()
        {
            for (int i = 0; i < EchoList.Count; i++)
                EchoList[i].Freeze();
        }

        private void UnfreezeAll()
        {
            for (int i = 0; i < EchoList.Count; i++)
                EchoList[i].Unfreeze();
        }

        private void CreateShell()
        {
            if (useShells)
            {
                GameObject _shell;
                _shell = Instantiate(shell, GameController.PlayerController.Transform.position, GameController.PlayerController.Transform.rotation) as GameObject;
                //_shell.GetComponent<Animator> ().runtimeAnimatorController = playerAnimator.runtimeAnimatorController;
                Animator _anim = _shell.GetComponent<Animator>();
                Debug.Log("player animator : " + PlayerAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash);
                _anim.Play(PlayerAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                int i = 0;
                foreach (var param in PlayerAnimator.parameters)
                {
                    _anim.parameters[i] = PlayerAnimator.parameters[i];
                }
                _anim.speed = 0;
            }

        }

        private void OnEclipseEventHandler(object sender, EventManager.EclipseEventArgs args)
        {
            if (args.EclipseOn)
            {
                IsEclipseActive = true;

                FreezeAll();
            }
            else
            {
                IsEclipseActive = false;

                UnfreezeAll();
            }
        }

        private void OnPreSceneChangeEvent(object sender, EventManager.PreSceneChangeEventArgs args)
        {
            Debug.LogFormat("EchoManager: OnPreSceneChangedEventHandler: echo count = {0}", EchoList.Count);

            IsEclipseActive = false;

            for (int i = 0; i < EchoList.Count; i++)
            {
                if (EchoList[i] == null)
                {
                    Debug.Log("echo is null");
                }
                Destroy(EchoList[i].gameObject);
            }

            PlacedEchoesCount = 0;
            EchoParticleSystem.RemoveAllEcho();
            EchoList.Clear();

            RemoveWaypoint();
        }

        private void OnSceneChangedEventHandler(object sender, EventManager.SceneChangedEventArgs args)
        {
        }
    }
} //end of namespace