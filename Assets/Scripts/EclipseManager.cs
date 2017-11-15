using UnityEngine;
using System.Collections;

namespace Game
{
    public class EclipseManager : MonoBehaviour
    {
        //[TestButton("Start Eclipse", "StartEclipse", isActiveInEditor = false)]
        //[TestButton("Stop Eclipse", "StopEclipse", isActiveInEditor = false)]        

        [SerializeField]
        float rotationDuration = 0.5f;

        [SerializeField]
        Vector3 regularGravity = new Vector3(0, -1, 0);

        [SerializeField]
        Vector3 eclipseGravity = new Vector3(1, 0, 0);

        global::Player player;

        bool isEclipseActive;

        //###############################################################

        #region Singleton
        //public static EclipseManager instance;
        //void Awake()
        //{
        //    if (!instance)
        //    {
        //        instance = this;
        //        //DontDestroyOnLoad(gameObject);
        //    }
        //    else if (instance != this)
        //        Destroy(gameObject);
        //}
        #endregion Singleton

        //###############################################################
        //###############################################################

        #region initialization

        public void InitializeEclipseManager(GameControl.IGameControllerBase gameController)
        {
            this.player = gameController.PlayerController.Player;

            Utilities.EventManager.OnEclipseEvent += OnEclipseEventHandler;
            Utilities.EventManager.OnSceneChangedEvent += OnSceneChangedEventHandler;
        }

        #endregion initialization

        //###############################################################
        //###############################################################

        #region event handlers

        void OnEclipseEventHandler(object sender, Game.Utilities.EventManager.OnEclipseEventArgs args)
        {
            if (args.EclipseOn)
            {
                StartEclipse();
            }
            else
            {
                StopEclipse();
            }
        }

        void OnSceneChangedEventHandler(object sender, Utilities.EventManager.OnSceneChangedEventArgs args)
        {
            StopEclipse();
        }

        #endregion event handlers

        //###############################################################
        //###############################################################

        void StartEclipse()
        {
            if (this.isEclipseActive)
            {
                return;
            }

            this.isEclipseActive = true;
            StopAllCoroutines();
            StartCoroutine(ChangeGravityRoutine(true));
        }

        void StopEclipse()
        {
            if (!this.isEclipseActive)
            {
                return;
            }

            this.isEclipseActive = false;
            StopAllCoroutines();
            StartCoroutine(ChangeGravityRoutine(false));
        }

        IEnumerator ChangeGravityRoutine(bool eclipseOn)
        {
            float gravityTimer = 0;
            player.SetVelocity(new Vector3(0f, 10f, 0f), false, false);
            while (gravityTimer < rotationDuration)
            {
                if (eclipseOn)
                {
                    player.ChangeGravityDirection(Vector3.Lerp(regularGravity, eclipseGravity, gravityTimer / rotationDuration));
                }
                else
                {
                    player.ChangeGravityDirection(Vector3.Lerp(eclipseGravity, regularGravity, gravityTimer / rotationDuration));
                }
                gravityTimer += Time.deltaTime;
                yield return null;
            }
            if (eclipseOn)
            {
                player.ChangeGravityDirection(eclipseGravity);
            }
            else
            {
                player.ChangeGravityDirection(regularGravity);
            }
        }

        //###############################################################
    }
} //end of namespace