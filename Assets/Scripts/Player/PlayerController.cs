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
        global::Player player;
        public global::Player Player { get { return this.player; } }

        InteractionController interactionController;
        public InteractionController InteractionController { get { return this.interactionController; } }

        WrappableObject wrappableObject;

        public AbilitySystem.TombFinderController TombFinderController { get; private set; }

        /// <summary>
        /// Initializes the Player Prefab with references to outside scripts.
        /// </summary>
        /// <param name="gameController"></param>
        public void InitializePlayerController(GameControl.IGameControllerBase gameController)
        {
            //getting all references
			this.player = GetComponent<global::Player> ();
			this.interactionController = GetComponentInChildren<InteractionController> ();
            this.wrappableObject = GetComponent<WrappableObject>();
            this.TombFinderController = GetComponentInChildren<AbilitySystem.TombFinderController>();

            //initializing all the things
            this.player.InitializePlayer(gameController.PlayerModel);
            this.interactionController.InitializeFavourController(gameController.PlayerModel, player);

            if (gameController.WorldController != null)
            {
                this.wrappableObject.InitializeWrappableObject(gameController.WorldController);
            }

            this.TombFinderController.InitializeTombFinderController(gameController);
        }
    }
}