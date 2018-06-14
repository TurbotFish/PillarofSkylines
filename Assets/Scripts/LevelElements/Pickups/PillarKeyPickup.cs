using Game.Model;
using UnityEngine;

namespace Game.LevelElements
{
    /// <summary>
    /// Pickup that gives a PillarKey to the player.
    /// </summary>
    public class PillarKeyPickup : Pickup
    {
        //##################################################################

        [Header("FavourPickup")]
        [SerializeField] private PillarMarkId PillarMarkId;
        [SerializeField] private Sprite tempIcon;

        //##################################################################

        public override string PickupName { get { return "the Mark"; } }
        public override string OnPickedUpMessage { get { return "The Eyes have marked you"; } }
        public override string OnPickedUpDescription { get { return "Break the Pillars to free the world"; } }
        public override Sprite OnPickedUpIcon { get { return tempIcon; } }

        //##################################################################

        protected override void OnPickedUp()
        {
            GameController.PlayerModel.SetPillarMarkState(PillarMarkId, PillarMarkState.active);
        }

        //##################################################################
    }
} // end of namespace