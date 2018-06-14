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

        // -- CONSTANTS



        //########################################################################

        // -- ATTRIBUTES

        public CharController CharController { get; private set; }
        public InteractionController InteractionController { get; private set; }
        public AudioManager AudioManager { get; private set; }

        private Transform myTransform;
        private WrappableObject wrappableObject;

        //########################################################################

        // -- INITIALIZATION

        public void InitializePlayerController(GameController gameController)
        {
            /*
             * getting all references
             */
            myTransform = transform;

            CharController = GetComponent<CharController>();
            InteractionController = GetComponentInChildren<InteractionController>();
            AudioManager = GetComponentInChildren<AudioManager>();

            wrappableObject = GetComponent<WrappableObject>();

            /*
             * initializing all the things
             */
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
    }
} // end of namespace