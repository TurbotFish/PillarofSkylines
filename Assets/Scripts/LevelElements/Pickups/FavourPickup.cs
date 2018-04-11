using Game.Model;
using UnityEngine;

namespace Game.LevelElements
{
    public class FavourPickup : Pickup
    {
        //##################################################################

        [Header("FavourPickup")]
        [SerializeField] private eAbilityType ability;

        //##################################################################

        public override string PickupName { get { return ability.ToString() + " Ability"; } }
        public override string OnPickedUpMessage { get { return "You have been granted a Favour"; } }
        public override string OnPickedUpDescription { get { return "Press Start to open the ability menu"; } }

        //##################################################################

        protected override bool OnPickedUp()
        {
            return gameController.PlayerModel.ActivateAbility(ability);
        }

        //##################################################################
    }
} // end of namespace