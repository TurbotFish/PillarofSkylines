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
        public CharacterController.Character OldPlayer { get; private set; }
        public CharacterController.CharController CharController { get; private set; }

        public InteractionController InteractionController { get; private set; }

        WrappableObject wrappableObject;

        public AbilitySystem.TombFinderController TombFinderController { get; private set; }

        /// <summary>
        /// Initializes the Player Prefab with references to outside scripts.
        /// </summary>
        /// <param name="gameController"></param>
        public void InitializePlayerController(GameControl.IGameControllerBase gameController)
        {
            //getting all references
            OldPlayer = GetComponent<CharacterController.Character>();
            InteractionController = GetComponentInChildren<InteractionController>();
            wrappableObject = GetComponent<WrappableObject>();
            TombFinderController = GetComponentInChildren<AbilitySystem.TombFinderController>();

            //initializing all the things
            OldPlayer.InitializePlayer(gameController.PlayerModel);
            InteractionController.InitializeFavourController(gameController.PlayerModel, OldPlayer, gameController.EchoManager);

            if (gameController.WorldController != null)
            {
                wrappableObject.InitializeWrappableObject(gameController.WorldController);
            }

            TombFinderController.InitializeTombFinderController(gameController);
        }
    }
} //end of namespace