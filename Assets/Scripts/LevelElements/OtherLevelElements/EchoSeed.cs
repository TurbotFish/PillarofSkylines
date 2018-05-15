using Game.GameControl;
using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    public class EchoSeed : MonoBehaviour, IInteractable, IWorldObject
    {
        //########################################################################

        private IGameController gameController;

        //########################################################################

        public void Initialize(IGameController gameController)
        {
            this.gameController = gameController;
        }

        //########################################################################

        public Vector3 Position { get { return transform.position; } }

        public bool IsInteractable()
        {
            return false;
        }

        //########################################################################

        public void OnPlayerEnter()
        {
            gameController.EchoManager.CreateEcho(false);
            Destroy(gameObject);
        }

        public void OnPlayerExit()
        {
        }

        public void OnHoverBegin()
        {
        }

        public void OnHoverEnd()
        {
        }

        public void OnInteraction()
        {
        }

        //########################################################################
    }
} // end of namespace