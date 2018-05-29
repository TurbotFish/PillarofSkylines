using Game.GameControl;
using Game.LevelElements;
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
        //########################################################################

        // -- ATTRIBUTES

        public CharController CharController { get; private set; }
        public InteractionController InteractionController { get; private set; }

        private Transform myTransform;
        private WrappableObject wrappableObject;

        //########################################################################

        // -- INITIALIZATION

        public void InitializePlayerController(GameController gameController)
        {
            Debug.Log("PlayerController: Initialize");

            //getting all references
            myTransform = transform;

            CharController = GetComponent<CharController>();
            InteractionController = GetComponentInChildren<InteractionController>();
            wrappableObject = GetComponent<WrappableObject>();

            //initializing all the things
            CharController.Initialize(gameController);
            InteractionController.Initialize(gameController);
            wrappableObject.Initialize(gameController);
        }

        //########################################################################

        // -- INQUIRIES

        public Transform PlayerTransform
        {
            get
            {
                if (myTransform == null) { myTransform = transform; }
                return myTransform;
            }
        }

        //########################################################################

        // -- OPERATIONS

        /// <summary>
        /// Called to handle input.
        /// </summary>
        public void HandleInput()
        {
            //charController.HandleInput();
            //interactionController.HandleInput();
        }
    }
} // end of namespace