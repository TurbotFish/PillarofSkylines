﻿using System.Collections;
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
        //[SerializeField]
        global::Player player;
        public global::Player Player { get { return this.player; } }

        //[SerializeField]
        InteractionController favourController;
        public InteractionController FavourController { get { return this.favourController; } }      

        /// <summary>
        /// Initializes the Player Prefab with references to outside scripts.
        /// </summary>
        /// <param name="gameController"></param>
        public void InitializePlayerController(GameControl.IGameControllerBase gameController)
        {
			this.player = transform.GetComponent<global::Player> ();
			this.favourController = GetComponentInChildren<InteractionController> ();

            this.player.InitializePlayer(gameController.PlayerModel);
            this.favourController.InitializeFavourController(gameController.PlayerModel, player);
        }
    }
}