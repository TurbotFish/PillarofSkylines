using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    [CreateAssetMenu(menuName = "ScriptableObjects/AbilityData", fileName = "AbilityData")]
    public class AbilityData : ScriptableObject
    {
        [SerializeField] private List<Ability> ab = new List<Ability>();

        //###########################################################

        #region methods

        public Ability GetAbility(eAbilityType ability)
        {
            var result = ab.FirstOrDefault(item => item.Type == ability);
            return result;
        }

        public List<Ability> GetAllAbilities()
        {
            return new List<Ability>(ab);
        }

        #endregion methods

        //###########################################################

        void OnValidate()
        {
            List<eAbilityType> wantedValues = Enum.GetValues(typeof(eAbilityType)).Cast<eAbilityType>().ToList();
            List<eAbilityType> presentValues = new List<eAbilityType>();

            List<Ability> elementsToRemove = new List<Ability>();
            bool changedList = false;

            foreach (var element in ab)
            {
                if (presentValues.Contains(element.Type))
                {
                    elementsToRemove.Add(element);
                }
                else
                {
                    presentValues.Add(element.Type);
                }
            }

            foreach(var element in elementsToRemove) //removing unnecessary objects
            {
                ab.Remove(element);
                changedList = true;
            }

            foreach(var type in wantedValues) //adding missing objects
            {
                if (!presentValues.Contains(type))
                {
                    ab.Add(new Ability(type));
                    changedList = true;
                }
            }

            if (changedList)
            {
                ab.Sort((x, y) => ((int)x.Type).CompareTo((int)y.Type));
            }

            foreach (var ability in ab)
            {
                ability.OnValidate();
            }
        }
    }
} //end of namespace