//using UnityEngine;

//namespace Game.World.Interaction
//{
//    public class FavourStatue : MonoBehaviour
//    {
//        public string favourID;

//        [SerializeField] Material matWhenActive;

//        Renderer rend;

//        private void Awake()
//        {
//            rend = GetComponent<Renderer>();
//            Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;
//        }

//        void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
//        {
//            if (args.FavourId == favourID)
//            {
//                rend.sharedMaterial = matWhenActive;
//            }
//        }
        
//    }
//}