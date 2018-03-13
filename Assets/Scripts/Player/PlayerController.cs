using Game.Player.CharacterController;
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
        public Transform PlayerTransform { get; private set; }

        //[System.Obsolete]
        //public Character OldPlayer { get; private set; }
        public CharController CharController { get; private set; }

        public InteractionController InteractionController { get; private set; }

        WrappableObject wrappableObject;

        //public AbilitySystem.TombFinderController TombFinderController { get; private set; }

        /// <summary>
        /// Initializes the Player Prefab with references to outside scripts.
        /// </summary>
        /// <param name="gameController"></param>
        public void InitializePlayerController(GameControl.IGameControllerBase gameController)
        {
            //getting all references
            PlayerTransform = transform;

            //OldPlayer = GetComponent<Character>();
            CharController = GetComponent<CharController>();

            InteractionController = GetComponentInChildren<InteractionController>();
            wrappableObject = GetComponent<WrappableObject>();
            //TombFinderController = GetComponentInChildren<AbilitySystem.TombFinderController>();

            //initializing all the things
            //OldPlayer.InitializePlayer(gameController.PlayerModel);
            CharController.Initialize(gameController);

            InteractionController.Initialize(gameController.PlayerModel, CharController, gameController.EchoManager);

            if (gameController.WorldController != null)
            {
                wrappableObject.InitializeWrappableObject(gameController);
            }

            //TombFinderController.InitializeTombFinderController(gameController);
        }
    }
}