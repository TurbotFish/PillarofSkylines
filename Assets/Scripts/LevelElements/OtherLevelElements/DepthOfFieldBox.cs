using Game.GameControl;
using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    public class DepthOfFieldBox : MonoBehaviour, IInteractable, IWorldObject
    {
        //########################################################################

        // -- CONSTANTS

        public enum MODE
        {
            ActivateDepthOfField,
            DeactivateDepthOfField
        }

        [SerializeField] private MODE Mode;

        //########################################################################

        // -- ATTRIBUTES

        private GameController GameController;
        private DepthOfField DepthOfFieldComponent;

        //########################################################################

        // -- INITIALIZATION

        public void Initialize(GameController gameController)
        {
            GameController = gameController;
            DepthOfFieldComponent = GameController.CameraController.GetComponent<DepthOfField>();
        }

        //########################################################################

        // -- INQUIRIES

        public Transform Transform { get { return this.transform; } }

        public bool IsInteractable()
        {
            return false;
        }

        //########################################################################

        // -- OPERATIONS

        public void OnPlayerEnter()
        {
            switch (Mode)
            {
                case MODE.ActivateDepthOfField:
                    DepthOfFieldComponent.enabled = true;
                    break;
                case MODE.DeactivateDepthOfField:
                    DepthOfFieldComponent.enabled = false;
                    break;
            }
        }

        public void OnPlayerExit()
        {
            switch (Mode)
            {
                case MODE.ActivateDepthOfField:
                    DepthOfFieldComponent.enabled = false;
                    break;
                case MODE.DeactivateDepthOfField:
                    DepthOfFieldComponent.enabled = true;
                    break;
            }
        }

        public void OnHoverBegin()
        {
            throw new System.NotImplementedException();
        }

        public void OnHoverEnd()
        {
            throw new System.NotImplementedException();
        }

        public void OnInteraction()
        {
            throw new System.NotImplementedException();
        }


    }
} // end of namespace