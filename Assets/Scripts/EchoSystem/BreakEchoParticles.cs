using UnityEngine;

namespace Game.EchoSystem
{
    public class BreakEchoParticles : MonoBehaviour
    {
        ParticleSystem ps;

        void Start()
        {
            ps = GetComponentInChildren<ParticleSystem>();
            ps.Play();
            Invoke("DestroyWhenOver", ps.main.duration);
        }

        void DestroyWhenOver()
        {
            Destroy(gameObject);
        }

    }
} //end of namespace