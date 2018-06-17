using Game.GameControl;
using Game.LevelElements;
using Game.Player.CharacterController;
using Game.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// This class handles the Initialization of the Player Prefab.
    /// This class MUST be at the root of the Player Prefab!
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        //########################################################################

        // -- CONSTANTS



        //########################################################################

        // -- ATTRIBUTES

        public CharController CharController { get; private set; }
        public InteractionController InteractionController { get; private set; }
        public AudioManager AudioManager { get; private set; }

        private GameController GameController;
        private WrappableObject WrappableObject;

        private Transform MyTransform;

        //########################################################################

        // -- INITIALIZATION

        public void InitializePlayerController(GameController game_controller)
        {
            GameController = game_controller;

            /*
             * getting all references
             */
            CharController = GetComponent<CharController>();
            InteractionController = GetComponentInChildren<InteractionController>();
            AudioManager = GetComponentInChildren<AudioManager>();

            WrappableObject = GetComponent<WrappableObject>();

            /*
             * initializing all the things
             */
            CharController.Initialize(GameController);
            InteractionController.Initialize(GameController);
            WrappableObject.Initialize(GameController);

            EventManager.PreSceneChangeEvent += OnPreSceneChangeEvent;
            EventManager.SceneChangedEvent += OnSceneChangedEvent;
        }

        private void OnDestroy()
        {
            EventManager.PreSceneChangeEvent -= OnPreSceneChangeEvent;
            EventManager.SceneChangedEvent -= OnSceneChangedEvent;
        }

        //########################################################################

        // -- INQUIRIES

        /// <summary>
        /// Returns a reference to the Transform component of the player.
        /// </summary>
        public Transform Transform
        {
            get
            {
                if (MyTransform == null) { MyTransform = this.transform; }
                return MyTransform;
            }
        }

        //########################################################################

        // -- OPERATIONS

        /// <summary>
        /// Handles the PreSceneChange event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPreSceneChangeEvent(object sender, EventManager.PreSceneChangeEventArgs args)
        {
            foreach (var firefly in GameController.PlayerModel.GetFireflyList())
            {
                firefly.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Handles the SceneChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnSceneChangedEvent(object sender, EventManager.SceneChangedEventArgs args)
        {
            if (!args.HasChangedToPillar)
            {
                foreach (var firefly in GameController.PlayerModel.GetFireflyList())
                {
                    firefly.gameObject.SetActive(true);
                }
            }
        }
    }
} // end of namespace