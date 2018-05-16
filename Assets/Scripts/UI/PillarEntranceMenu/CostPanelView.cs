using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.UI.PillarEntranceMenu
{
    public class CostPanelView : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI valueText;

        [SerializeField]
        TextMeshProUGUI unitSuffixText;

        public void Initialize(int cost)
        {
            valueText.text = cost.ToString();

            if (cost == 1)
                unitSuffixText.text = "";
            else
                unitSuffixText.text = "s";
        }
    }
} //end of namespace