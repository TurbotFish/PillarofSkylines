using UnityEngine;
using System.Collections;
using Game.Player.CharacterController;
using Game.Utilities;

namespace Game.GameControl
{
    public class EclipseManager : MonoBehaviour
    {
        //[TestButton("Start Eclipse", "StartEclipse", isActiveInEditor = false)]
        //[TestButton("Stop Eclipse", "StopEclipse", isActiveInEditor = false)]    

        //###############################################################

        // -- CONSTANTS

        [SerializeField] private float rotationDuration = 0.5f;
        [SerializeField] private Vector3 regularGravity = new Vector3(0, -1, 0);
        [SerializeField] private Vector3 eclipseGravity = new Vector3(1, 0, 0);
        [SerializeField] private Eclipse eclipsePostEffect;

        //###############################################################

        // -- ATTRIBUTES

        private CharController Player;

        private bool IsEclipseActive;

        //###############################################################

        // -- INITIALIZATION

        /// <summary>
        /// Initializes the eclipse manager
        /// </summary>
        /// <param name="game_controller"></param>
        public void Initialize(GameController game_controller)
        {
            Player = game_controller.PlayerController.CharController;
            eclipsePostEffect = game_controller.CameraController.EclipseEffect;

            EventManager.EclipseEvent += OnEclipseEventHandler;
            EventManager.PreSceneChangeEvent += OnPreSceneChangedEvent;
        }

        /// <summary>
        /// Monobehaviour's OnDestroy methods.
        /// </summary>
        private void OnDestroy()
        {
            EventManager.EclipseEvent -= OnEclipseEventHandler;
            EventManager.PreSceneChangeEvent -= OnPreSceneChangedEvent;
        }

        //###############################################################

        // -- INQUIRIES

        /// <summary>
        /// Returns the gravity direction of the eclipse mode.
        /// </summary>
        public Vector3 EclipseGravity { get { return eclipseGravity; } }

        //###############################################################

        // -- OPERATIONS

        /// <summary>
        /// Handles the eclipse event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnEclipseEventHandler(object sender, EventManager.EclipseEventArgs args)
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

        /// <summary>
        /// Handles the pre scene changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPreSceneChangedEvent(object sender, EventManager.PreSceneChangeEventArgs args)
        {
            StopEclipse();
        }

        /// <summary>
        /// Activates the eclipse mode.
        /// </summary>
        private void StartEclipse()
        {
            if (IsEclipseActive)
            {
                return;
            }

            IsEclipseActive = true;

            StopAllCoroutines();
            StartCoroutine(ChangeGravityRoutine(true, 1.8f));
        }

        /// <summary>
        /// Ends the eclipse mode.
        /// </summary>
        private void StopEclipse()
        {
            if (!IsEclipseActive)
            {
                return;
            }

            IsEclipseActive = false;
            StopAllCoroutines();
            StartCoroutine(ChangeGravityRoutine(false, 1f));
        }

        /// <summary>
        /// Coroutine that changes the gravity over time.
        /// </summary>
        /// <param name="eclipseOn"></param>
        /// <returns></returns>
        private IEnumerator ChangeGravityRoutine(bool eclipseOn, float delay = 0f)
        {
            if (delay != 0f)
                yield return new WaitForSeconds(delay);

            float gravityTimer = 0;
            Player.AddExternalVelocity(new Vector3(0f, 10f, 0f), false, false);

            if (eclipseOn)
                eclipsePostEffect.enabled = true;

            while (gravityTimer < rotationDuration)
            {
                float t = gravityTimer / rotationDuration;
                if (eclipseOn)
                {
                    Player.ChangeGravityDirection(Vector3.Slerp(regularGravity, eclipseGravity, t));
                    eclipsePostEffect.Intensity = Mathf.Lerp(0, 1, t);
                }
                else
                {
                    Player.ChangeGravityDirection(Vector3.Slerp(eclipseGravity, regularGravity, t));
                    eclipsePostEffect.Intensity = Mathf.Lerp(1, 0, t);
                }
                gravityTimer += Time.deltaTime;
                yield return null;
            }
            if (eclipseOn)
            {
                Player.ChangeGravityDirection(eclipseGravity);
                eclipsePostEffect.Intensity = 1;
            }
            else
            {
                Player.ChangeGravityDirection(regularGravity);
                eclipsePostEffect.enabled = false;
            }
        }
    }
} //end of namespace