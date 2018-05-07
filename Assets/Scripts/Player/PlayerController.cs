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

        // ATTRIBUTES

        private Transform myTransform;

        private CharController charController;
        private InteractionController interactionController;
        private WrappableObject wrappableObject;

        private AirParticle airParticle;

        //########################################################################

        // INITIALIZATION

        public void InitializePlayerController(IGameControllerBase gameController)
        {
            Debug.Log("PlayerController: Initialize");

            //getting all references
            myTransform = transform;

            charController = GetComponent<CharController>();
            interactionController = GetComponentInChildren<InteractionController>();
            wrappableObject = GetComponent<WrappableObject>();

            //initializing all the things
            CharController.Initialize(gameController);
            InteractionController.Initialize(gameController);
            wrappableObject.InitializeWrappableObject(gameController);
        }

        //########################################################################

        // INQUIRIES

        public Transform PlayerTransform
        {
            get
            {
                if (myTransform == null) { myTransform = transform; }
                return myTransform;
            }
        }

        public CharController CharController { get { return charController; } }
        public InteractionController InteractionController { get { return interactionController; } }

        public bool HasAirParticle { get { return airParticle != null; } }
        public AirParticle AirParticle { get { return airParticle; } }

        //########################################################################

        // OPERATIONS

        public void SetAirParticle(AirParticle airParticle)
        {
            this.airParticle = airParticle;
        }

        //########################################################################
    }
} // end of namespace