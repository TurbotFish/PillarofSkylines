//using UnityEngine;

//namespace Game.EchoSystem
//{
//    [RequireComponent(typeof(Collider))]
//    public class EchoBreaker : MonoBehaviour
//    {
//        EchoManager echoes;
//        Collider col;
//        private void Start()
//        {
//            col = GetComponent<Collider>();
//            col.isTrigger = true;
//            echoes = EchoManager.instance;
//        }

//        private void OnTriggerEnter(Collider other)
//        {
//            if (other.tag == "Player")
//            {
//                echoes.BreakAll();
//            }
//        }

//    }
//} //end of namespace