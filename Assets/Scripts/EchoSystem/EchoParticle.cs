using System;
using Game.Utilities;
using UnityEngine;


namespace Game.EchoSystem
{
    public class EchoParticle : MonoBehaviour {

        public Vector3 target;
        public float speed;

        void Start()
        {
            EventManager.TeleportPlayerEvent += OnTeleportPlayerEventHandler;
        }

        private void OnDestroy()
        {
            EventManager.TeleportPlayerEvent -= OnTeleportPlayerEventHandler;
        }


        private void OnTeleportPlayerEventHandler(object sender, EventManager.TeleportPlayerEventArgs args)
        {
            transform.position += (args.Position - args.FromPosition);
            target += (args.Position - args.FromPosition);
        }

        void Update () {
            transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
	    }
    }
}
