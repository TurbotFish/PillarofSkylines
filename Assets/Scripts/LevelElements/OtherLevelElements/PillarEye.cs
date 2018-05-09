using Game.GameControl;
using Game.Utilities;
using Game.World;
using UnityEngine;

namespace Game.LevelElements
{
    public class PillarEye : MonoBehaviour, IInteractable, IWorldObject
    {
        //########################################################################

        [SerializeField] GameObject defaultEye;
        [SerializeField] GameObject eclipseEye;
        
        [SerializeField] float lookAtDamp = 0.5f;

        private IGameControllerBase gameController;
        private Transform target;

        //########################################################################

        public void Initialize(IGameControllerBase gameController)
        {
            this.gameController = gameController;
            target = gameController.PlayerController.CharController.MyTransform;
        }

        private void OnEnable()
        {
            EventManager.EclipseEvent += OnEclipseEventHandler;
        }

        private void OnDisable()
        {
            EventManager.EclipseEvent -= OnEclipseEventHandler;
        }

        //########################################################################

        public Vector3 Position { get { return transform.position; } }

        public bool IsInteractable()
        {
            return gameController.PlayerModel.hasNeedle;
        }

        //########################################################################

        private void Update()
        {
            if (target != null) //smooth look at
            {
                var rotation = Quaternion.LookRotation(target.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * lookAtDamp);
            }
        }

        public void OnPlayerEnter()
        {
        }

        public void OnPlayerExit()
        {
        }

        public void OnHoverBegin()
        {
            var eventArgs = new EventManager.OnShowHudMessageEventArgs(true, "[X]: Plant Needle");
            EventManager.SendShowHudMessageEvent(this, eventArgs);
        }

        public void OnHoverEnd()
        {
            var eventArgs = new EventManager.OnShowHudMessageEventArgs(false);
            EventManager.SendShowHudMessageEvent(this, eventArgs);
        }

        public void OnInteraction()
        {
            gameController.PlayerModel.hasNeedle = false;

            EventManager.SendLeavePillarEvent(this, new EventManager.LeavePillarEventArgs(true));

            var showMessageEventArgs = new EventManager.OnShowHudMessageEventArgs(false);
            EventManager.SendShowHudMessageEvent(this, showMessageEventArgs);
        }

        void OnEclipseEventHandler(object sender, EventManager.EclipseEventArgs args)
        {
            if (args.EclipseOn) //eclipse on
            {
                defaultEye.SetActive(false);
                eclipseEye.SetActive(true);
            }
            else //eclipse off
            {
                defaultEye.SetActive(true);
                eclipseEye.SetActive(false);
            }
        }

        //########################################################################
    }
} // end of namespace