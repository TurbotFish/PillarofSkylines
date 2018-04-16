using Game.Model;
using UnityEngine;

namespace Game.LevelElements
{
    /// <summary>
    /// Pickup that activates a player ability.
    /// </summary>
    public class FavourPickup : Pickup
    {
        //##################################################################

        [Header("FavourPickup")]
        [SerializeField] private eAbilityType ability;

        //##################################################################

        public override string PickupName { get { return ability.ToString() + " Ability"; } }
		public override string OnPickedUpMessage { get { return "You've been granted the "+ability.ToString() + " Ability"; } }
        public override string OnPickedUpDescription { get { return "Press Back to see how to use your abilities"; } }

        //##################################################################

        protected override void OnPickedUp()
        {
            GameController.PlayerModel.ActivateAbility(ability);
        }

        //##################################################################
    }
} // end of namespace