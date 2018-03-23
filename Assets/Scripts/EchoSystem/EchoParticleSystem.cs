using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.EchoSystem
{
    public class EchoParticleSystem : MonoBehaviour
    {
        
        public EchoParticle particlePrefab;
        public float particleSpeed = 10f;
        [HideInInspector]
        public int numEchoes = 3;
        

        List<EchoParticle> activeEchoParticles = new List<EchoParticle>();
        List<Vector3> targets = new List<Vector3>();
        
        List<EchoParticle> disabledEchoParticles = new List<EchoParticle>();

        void Start()
        {
            for (int i = 0; i < numEchoes; i++)
            {
                disabledEchoParticles.Add(Instantiate(particlePrefab, transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity));
                disabledEchoParticles[i].target = transform.position;
                disabledEchoParticles[i].speed = particleSpeed;
                disabledEchoParticles[i].gameObject.SetActive(false);
            }
        }

        void Update()
        {
            int i = 0;
            foreach (EchoParticle ep in activeEchoParticles)
            {
                Quaternion lookEcho = Quaternion.identity;
                if (targets[i] - transform.position != Vector3.zero)
                    lookEcho = Quaternion.LookRotation(targets[i] - transform.position);
                
                ep.target = transform.position + new Vector3(0f, 1f, 0f) + lookEcho * Vector3.forward;
                ep.transform.rotation = lookEcho;
                i++;
            }
        }

        public void AddEcho(Vector3 echoPosition)
        {
            activeEchoParticles.Add(disabledEchoParticles[0]);
            disabledEchoParticles.RemoveAt(0);
            activeEchoParticles[activeEchoParticles.Count - 1].gameObject.SetActive(true);
            activeEchoParticles[activeEchoParticles.Count - 1].transform.position = transform.position + new Vector3(0f, 1f, 0f);
            targets.Add(echoPosition);
        }
        
        public void RemoveAllEcho()
        {
            foreach (EchoParticle ep in activeEchoParticles)
            {
                disabledEchoParticles.Add(ep);
                ep.gameObject.SetActive(false);
            }
            activeEchoParticles.Clear();
            targets.Clear();
        }

        public void RemoveEcho(Vector3 echoPosition)
        {
            disabledEchoParticles.Add(activeEchoParticles[0]);
            activeEchoParticles.RemoveAt(0);
            disabledEchoParticles[disabledEchoParticles.Count - 1].gameObject.SetActive(false);
            targets.Remove(echoPosition);
        }

        /*
        int EchoIndexFromPosition(Vector3 position)
        {
            for (int i = 0; i < numActiveEchoes; i++)
            {
                if (targets[i] == position)
                    return i;
            }
            return 1000;
        }*/
    }
} //end of namespace