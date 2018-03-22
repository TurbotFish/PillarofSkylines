using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.EchoSystem
{
    public class EchoParticleSystem : MonoBehaviour
    {
        ParticleSystem system;
        ParticleSystem.Particle[] particles;

        int numParticlesAlive;

        public int numEchoes = 3;

        void Update()
        {
            InitializeIfNeeded();

            system = GetComponent<ParticleSystem>();
            numParticlesAlive = system.GetParticles(particles);

            //		print("there is currently " + numParticlesAlive + " particles, and we have " + numEchoes + " echoes available.");

            if (numParticlesAlive > numEchoes)
            {
                particles[numParticlesAlive].remainingLifetime = 0f;
                //			print("destroyed one particle");
                system.SetParticles(particles, numParticlesAlive - 1);
            }
            else if (numParticlesAlive < numEchoes)
            {
                system.Emit(numEchoes - numParticlesAlive);
                //			print("added " + (numEchoes - numParticlesAlive) + " particle");
            }
        }

        public void SetEchoNumber(int echoesAvailable)
        {
            numEchoes = echoesAvailable;
        }

        void InitializeIfNeeded()
        {
            if (system == null)
                system = GetComponent<ParticleSystem>();

            if (particles == null || particles.Length < system.maxParticles)
                particles = new ParticleSystem.Particle[system.maxParticles];
        }
    }
} //end of namespace