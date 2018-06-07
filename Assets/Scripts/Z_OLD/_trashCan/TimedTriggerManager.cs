//using UnityEngine;
//using System.Collections;

//namespace Game.LevelElements
//{
//    public class TimedTriggerManager : MonoBehaviour
//    {
//        public Mechanism[] mechanisms;

//        [Space, TestButton("Create Activator", "CreateActivator")]

//        public float timer = 10;
//        public float elapsed = 0;

//        bool state;

//        public void Activate()
//        {
//            elapsed = 0;
//            StopAllCoroutines();
//            if (state) return;

//            for (int i = 0; i < mechanisms.Length; i++)
//                mechanisms[i].Trigger(true);
//            state = true;
//        }

//        public void StartTimer()
//        {
//            StartCoroutine(TickTock());
//        }

//        IEnumerator TickTock()
//        {
//            while (elapsed < timer)
//            {
//                elapsed += Time.deltaTime;
//                yield return null;
//            }
//            for (int i = 0; i < mechanisms.Length; i++)
//                mechanisms[i].Trigger(false);
//            state = false;
//        }

//        public void CreateActivator()
//        {
//            GameObject obj = new GameObject();
//            obj.transform.parent = transform;
//            obj.transform.localPosition = Vector3.zero;
//            obj.name = "Activator";

//            TimedActivator a = obj.AddComponent<TimedActivator>();
//            a.manager = this;

//            BoxCollider col = obj.AddComponent<BoxCollider>();
//            col.isTrigger = true;

//            obj.layer = 12; // Layer PickUps
//            obj.tag = "TriggerActivator";
//        }
//    }
//} //end of namespace