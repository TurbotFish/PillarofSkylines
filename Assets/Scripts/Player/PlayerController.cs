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

        /// <summary>
        /// Initializes the Player Prefab with references to outside scripts.
        /// </summary>
        /// <param name="gameController"></param>
        public void InitializePlayerController(GameControl.IGameControllerBase gameController)
        {
			this.player = this.transform.GetComponent<global::Player> ();
			this.interactionController = GetComponentInChildren<InteractionController> ();
            this.wrappableObject = this.transform.GetComponent<WrappableObject>();

            this.player.InitializePlayer(gameController.PlayerModel);
            this.interactionController.InitializeFavourController(gameController.PlayerModel, player);
            this.wrappableObject.InitializeWrappableObject(gameController.WorldController);
        }
    }
}