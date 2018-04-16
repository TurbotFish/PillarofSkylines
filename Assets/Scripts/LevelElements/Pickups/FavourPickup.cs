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
        private Ability abilityData;

        //##################################################################

        public override string PickupName { get { return abilityData.Name + " Ability"; } }
		public override string OnPickedUpMessage { get { return "You've been granted the " + abilityData.Name + " Ability"; } }
        public override string OnPickedUpDescription { get { return abilityData.Description; } }

        //##################################################################

        protected override void OnPickedUp()
        {
            abilityData = GameController.PlayerModel.AbilityData.GetAbility(ability);
            GameController.PlayerModel.ActivateAbility(ability);
        }

        //##################################################################
    }
} // end of namespace