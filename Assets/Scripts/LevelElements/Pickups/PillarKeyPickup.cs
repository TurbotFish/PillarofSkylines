using Game.Model;

namespace Game.LevelElements
{
    public class PillarKeyPickup : Pickup<PickupPersistentData>
    {
        //##################################################################

        public override string PickupName { get { return "Pillar Key"; } }
        public override string OnPickedUpMessage { get { return "The Eyes have marked you"; } }
        public override string OnPickedUpDescription { get { return "Destroy the Pillars to free the world"; } }

        //##################################################################

        protected override bool OnPickedUp()
        {
            gameController.PlayerModel.ChangePillarKeysCount(1);
            return true;
        }

        //##################################################################
    }
} // end of namespace