using Game.GameControl;
using Game.Model;
using System;
using UnityEngine;

namespace Game.LevelElements
{
    public delegate void PickingUpFinishedCallback(bool showMessage, string message = "", string description = "");

    public abstract class Pickup : PersistentLevelElement<PickupPersistentData>
    {
        //##################################################################

        [Header("Pickup")]
        [SerializeField] private TombAnimator tombAnimator;
        [SerializeField] private new Collider collider;

        private PickingUpFinishedCallback callback;

        //##################################################################

        public override void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            base.Initialize(gameController, isCopy);

            tombAnimator.SetTombState(PersistentData.IsPickedUp, false, true);
        }

        //##################################################################

        public bool IsPickedUp { get { return PersistentData.IsPickedUp; } }

        public abstract string PickupName { get; }
        public abstract string OnPickedUpMessage { get; }
        public abstract string OnPickedUpDescription { get; }

        //##################################################################

        protected abstract bool OnPickedUp();

        public void PickupObject(PickingUpFinishedCallback callback)
        {
            if (PersistentData.IsPickedUp)
            {
                return;
            }

            if (OnPickedUp())
            {
                this.callback = callback;

                if (!tombAnimator.SetTombState(true, true, false, OnTombAnimatingDone))
                {
                    callback?.Invoke(false);
                }

                PersistentData.IsPickedUp = true;
            }
        }

        protected override void OnPersistentDataChange(object sender, EventArgs args)
        {
            if (PersistentData.IsPickedUp)
            {
                collider.enabled = false;

                if (!tombAnimator.IsTombActivated)
                {
                    tombAnimator.SetTombState(true, IsCopy, true);
                }
            }
        }

        protected override PersistentData CreatePersistentDataObject()
        {
            return new PickupPersistentData(UniqueId);
        }

        private void OnTombAnimatingDone()
        {
            callback?.Invoke(true, OnPickedUpMessage, OnPickedUpDescription);
        }

        //##################################################################
    }
} // end of namespace