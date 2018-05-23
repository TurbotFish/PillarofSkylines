using UnityEngine;
using System.Collections.Generic;

namespace Game.EchoSystem
{
    public class BreakEchoParticles : MonoBehaviour
    {
		public List<ParticleSystem> particles = new List<ParticleSystem>();
		public float duration;

        void Start()
        {
			foreach (ParticleSystem ps in particles) {
				ps.Play ();
			}
            Invoke("DestroyWhenOver", duration);
        }

        void DestroyWhenOver()
        {
            Destroy(gameObject);
        }

    }
} //end of namespace