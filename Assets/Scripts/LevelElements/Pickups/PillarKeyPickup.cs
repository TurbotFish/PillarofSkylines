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

        //##################################################################

        public override string PickupName { get { return "Pillar Key"; } }
        public override string OnPickedUpMessage { get { return "The Eyes have marked you"; } }
        public override string OnPickedUpDescription { get { return "Break the Pillars to free the world"; } }

        //##################################################################

        protected override void OnPickedUp()
        {
            GameController.PlayerModel.SetPillarMarkState(PillarMarkId, PillarMarkState.active);
        }

        //##################################################################
    }
} // end of namespace