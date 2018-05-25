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

        private IGameController gameController;
        private Transform target;

        //########################################################################

        public void Initialize(IGameController gameController)
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

        public Transform Transform { get { return transform; } }

        public bool IsInteractable()
        {
            return gameController.PlayerModel.PlayerHasNeedle;
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
            if (!IsInteractable())
            {
                return;
            }

            var showMessageEventArgs = new EventManager.OnShowHudMessageEventArgs(false);
            EventManager.SendShowHudMessageEvent(this, showMessageEventArgs);

            var model = gameController.PlayerModel;

            model.PlayerHasNeedle = false;

            model.SetAbilityState(model.LevelData.GetPillarRewardAbility(gameController.ActivePillarId), Model.AbilityState.active);
            model.SetPillarState(gameController.ActivePillarId, Model.PillarState.Destroyed);

            gameController.SwitchToOpenWorld();
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