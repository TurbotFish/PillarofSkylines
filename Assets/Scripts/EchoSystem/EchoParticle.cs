using System;
using Game.Utilities;
using UnityEngine;


namespace Game.EchoSystem
{
    public class EchoParticle : MonoBehaviour {

        public Vector3 target;
        public float speed;
        public int rank;

        ParticleSystem system;
        float baseLifetime;

        void Awake() {
            system = GetComponentInChildren<ParticleSystem>();
            baseLifetime = system.main.startLifetime.Evaluate(0);
            EventManager.TeleportPlayerEvent += OnTeleportPlayerEventHandler;
        }

        private void OnDestroy() {
            EventManager.TeleportPlayerEvent -= OnTeleportPlayerEventHandler;
        }

        public void ChangeRank(int newRank)
        {
            rank = newRank;
            ParticleSystem.MainModule main = system.main;
            main.startLifetime = baseLifetime * (1f - (rank / 3f));
            //main.startLifetimeMultiplier = 1f - (rank/3f);
        }

        private void OnTeleportPlayerEventHandler(object sender, EventManager.TeleportPlayerEventArgs args)
        {
            transform.position += (args.Position - args.FromPosition);
            target += (args.Position - args.FromPosition);
        }

        void Update () {

            transform.position = target;

            //           v old v
            //transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
	    }
    }
}
