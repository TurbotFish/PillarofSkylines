//using UnityEngine;

//namespace Game
//{
//    public class PillarDestroyedFeedback : MonoBehaviour
//    {
//        Renderer rend;

//        [SerializeField] World.ePillarId pillarID;
//        [SerializeField] Material matWhenDestroyed;

//        private void OnEnable()
//        {
//            rend = GetComponent<Renderer>();
//            Utilities.EventManager.PillarDestroyedEvent += OnPillarDestroyed;
//        }

//        private void OnDisable()
//        {
//            Utilities.EventManager.PillarDestroyedEvent -= OnPillarDestroyed;
//        }

//        void OnPillarDestroyed(object sender, Utilities.EventManager.PillarDestroyedEventArgs args)
//        {
//            if (args.PillarId == pillarID)
//            {
//                rend.sharedMaterial = matWhenDestroyed;
//            }
//        }

//    }
//}

