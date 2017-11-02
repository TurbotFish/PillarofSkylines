using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Player.UI.PillarEntranceMenu
{
    public class CostPanelView : MonoBehaviour
    {
        [SerializeField]
        Text valueText;

        [SerializeField]
        Text unitSuffixText;

        public void Initialize(int cost)
        {
            this.valueText.text = cost.ToString();

            if (cost == 1)
            {
                this.unitSuffixText.text = "";
            }
            else
            {
                this.unitSuffixText.text = "s";
            }
        }
    }
} //end of namespace