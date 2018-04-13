﻿using UnityEngine;
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

        Game.Player.CharacterController.CharController player;
		[SerializeField]
        Eclipse eclipsePostEffect;

        bool isEclipseActive;

        //###############################################################

        #region initialization

        public void InitializeEclipseManager(GameControl.IGameControllerBase gameController)
        {
            this.player = gameController.PlayerController.CharController;
            this.eclipsePostEffect = gameController.CameraController.EclipseEffect;

            Utilities.EventManager.EclipseEvent += OnEclipseEventHandler;
            Utilities.EventManager.SceneChangedEvent += OnSceneChangedEventHandler;
        }

        #endregion initialization

        //###############################################################

        #region event handlers

        void OnEclipseEventHandler(object sender, Game.Utilities.EventManager.EclipseEventArgs args)
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

        void OnSceneChangedEventHandler(object sender, Utilities.EventManager.SceneChangedEventArgs args)
        {
            StopEclipse();
        }

        #endregion event handlers

        //###############################################################

        void StartEclipse()
        {
            if (this.isEclipseActive)
            {
                return;
            }

            this.isEclipseActive = true;

			eclipsePostEffect.Intensity = 1;
			eclipsePostEffect.colorChangeR = 0;
			eclipsePostEffect.colorChangeB = 1;
			eclipsePostEffect.colorChangeG = 1;

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
            //player.SetVelocity(new Vector3(0f, 10f, 0f), false, false);
            player.AddExternalVelocity(new Vector3(0f, 10f, 0f), false, false);

            if (eclipseOn)
                eclipsePostEffect.enabled = true;

            while (gravityTimer < rotationDuration)
            {
                float t = gravityTimer / rotationDuration;
                if (eclipseOn)
                {
                    player.ChangeGravityDirection(Vector3.Lerp(regularGravity, eclipseGravity, t));
                    eclipsePostEffect.Intensity = Mathf.Lerp(0, 1, t);
                }
                else
                {
                    player.ChangeGravityDirection(Vector3.Lerp(eclipseGravity, regularGravity, t));
                    eclipsePostEffect.Intensity = Mathf.Lerp(1, 0, t);
                }
                gravityTimer += Time.deltaTime;
                yield return null;
            }
            if (eclipseOn)
            {
                player.ChangeGravityDirection(eclipseGravity);
                eclipsePostEffect.Intensity = 1;
            }
            else
            {
                player.ChangeGravityDirection(regularGravity);
                eclipsePostEffect.enabled = false;
            }
        }

        //###############################################################
    }
} //end of namespace