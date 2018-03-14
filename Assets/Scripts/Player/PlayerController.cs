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

        public CharController CharController { get; private set; }

        public InteractionController InteractionController { get; private set; }

        WrappableObject wrappableObject;

        /// <summary>
        /// Initializes the Player Prefab with references to outside scripts.
        /// </summary>
        /// <param name="gameController"></param>
        public void InitializePlayerController(GameControl.IGameControllerBase gameController)
        {
            Debug.Log("PlayerController: Initialize");

            //getting all references
            PlayerTransform = transform;

            CharController = GetComponent<CharController>();

            InteractionController = GetComponentInChildren<InteractionController>();
            wrappableObject = GetComponent<WrappableObject>();

            //initializing all the things
            CharController.Initialize(gameController);
            InteractionController.Initialize(gameController);
            wrappableObject.InitializeWrappableObject(gameController);
        }
    }
}