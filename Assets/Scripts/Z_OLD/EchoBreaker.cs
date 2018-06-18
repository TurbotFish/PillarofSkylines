using UnityEngine;
using Game.GameControl;
using Game.LevelElements;
using Game.World;

namespace Game.EchoSystem
{
    [RequireComponent(typeof(Collider))]
    public class EchoBreaker : MonoBehaviour, IInteractable, IWorldObject
    {
        GameController gameControl;

        public Transform Transform
        {
            get
            {
                return transform;
            }
        }

        public void Initialize(GameController gameController)
        {
            gameControl = gameController;
        }

        public bool IsInteractable()
        {
            return false;
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

        public void OnPlayerEnter()
        {
            gameControl.EchoManager.BreakAll();
        }

        public void OnPlayerExit()
        {
        }
        
    }
} //end of namespace