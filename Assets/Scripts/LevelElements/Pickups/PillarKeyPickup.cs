using Game.Model;

namespace Game.LevelElements
{
    /// <summary>
    /// Pickup that gives a PillarKey to the player.
    /// </summary>
    public class PillarKeyPickup : Pickup
    {
        //##################################################################

        public override string PickupName { get { return "Pillar Key"; } }
        public override string OnPickedUpMessage { get { return "The Eyes have marked you"; } }
        public override string OnPickedUpDescription { get { return "Destroy the Pillars to free the world"; } }

        //##################################################################

        protected override void OnPickedUp()
        {
            GameController.PlayerModel.ChangePillarKeysCount(1);
        }

        //##################################################################
    }
} // end of namespace