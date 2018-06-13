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
        [SerializeField] private AbilityType ability;

        private Ability AbilityData { get { return GameController.PlayerModel.AbilityData.GetAbility(ability); } }

        //##################################################################

        public override string PickupName { get { return AbilityData.Name; } }
        public override string OnPickedUpMessage { get { return "You've been granted the " + AbilityData.Name; } }
        public override string OnPickedUpDescription { get { return AbilityData.Description; } }
        public override Sprite OnPickedUpIcon { get { return GameController.PlayerModel.AbilityData.GetAbility(ability).Icon; } }

        //##################################################################

        protected override void OnPickedUp()
        {
            GameController.PlayerModel.SetAbilityState(ability, AbilityState.active);
        }

        //##################################################################
    }
} // end of namespace