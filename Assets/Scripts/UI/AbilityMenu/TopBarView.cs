using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.AbilityMenu
{
    public class TopBarView : MonoBehaviour
    {
        [SerializeField]
        Text favourAmountText;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Initialize(int favourAmount)
        {
            this.favourAmountText.text = favourAmount.ToString();

            Utilities.EventManager.FavourAmountChangedEvent += OnFavourAmountChangedEventHandler;
        }

        void OnFavourAmountChangedEventHandler(object sender, Utilities.EventManager.FavourAmountChangedEventArgs args)
        {
            this.favourAmountText.text = args.FavourAmount.ToString();
        }
    }
} //end of namespace