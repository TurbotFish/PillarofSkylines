using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.GameControl;

namespace Game.EchoSystem
{
    public class EchoParticleSystem : MonoBehaviour
    {
        
        public EchoParticle particlePrefab;
        public float particleSpeed = 10f;
        [HideInInspector]
        public int numEchoes = 3;

        IGameController gameController;
        float worldSizeX, worldSizeY, worldSizeZ;
        bool isInitialized = false;
        
        List<EchoParticle> activeEchoParticles = new List<EchoParticle>();
        List<Vector3[]> targets = new List<Vector3[]>();
        
        List<EchoParticle> disabledEchoParticles = new List<EchoParticle>();
        

        public void InitializeEchoParticleSystem(IGameController gameController)
        {
            this.gameController = gameController;

            Utilities.EventManager.SceneChangedEvent += OnSceneLoaded;

            for (int i = 0; i < numEchoes; i++)
            {
                disabledEchoParticles.Add(Instantiate(particlePrefab, transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity));
                disabledEchoParticles[i].target = transform.position;
                disabledEchoParticles[i].speed = particleSpeed;
                disabledEchoParticles[i].gameObject.SetActive(false);
            }
            if (gameController.IsOpenWorldLoaded)
            {
                worldSizeX = gameController.WorldController.WorldSize.x;
                worldSizeY = gameController.WorldController.WorldSize.y;
                worldSizeZ = gameController.WorldController.WorldSize.z;
            }

            isInitialized = true;
        }

        void OnSceneLoaded(object sender, Utilities.EventManager.SceneChangedEventArgs args)
        {
            if (gameController.IsOpenWorldLoaded)
            {
                worldSizeX = gameController.WorldController.WorldSize.x;
                worldSizeY = gameController.WorldController.WorldSize.y;
                worldSizeZ = gameController.WorldController.WorldSize.z;
            }
        }

        void Update()
        {
            if (!isInitialized)
            {
                return;
            }
            int i = 0;
            foreach (EchoParticle ep in activeEchoParticles)
            {
                Quaternion lookEcho = Quaternion.identity;
                Vector3 target = Vector3.positiveInfinity;
                if (targets.Count > 0)
                {
                    foreach (Vector3 echoPos in targets[i])
                    {
                        if ((target - transform.position).sqrMagnitude > (echoPos - transform.position).sqrMagnitude)
                        {
                            target = echoPos;
                        }
                    }
                }
                if (target - transform.position != Vector3.zero)
                    lookEcho = Quaternion.LookRotation(target - transform.position);
                
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
            if (gameController.IsOpenWorldLoaded)
            {
                targets.Add(new Vector3[] {echoPosition, echoPosition + new Vector3(0, worldSizeY, worldSizeZ), echoPosition + new Vector3(0, worldSizeY, 0), echoPosition + new Vector3(0, worldSizeY, -worldSizeZ)
                    , echoPosition + new Vector3(0, 0, worldSizeZ), echoPosition + new Vector3(0, 0, -worldSizeZ)
                    , echoPosition + new Vector3(0, -worldSizeY, worldSizeZ), echoPosition + new Vector3(0, -worldSizeY, 0), echoPosition + new Vector3(0, -worldSizeY, -worldSizeZ)});
            }
            else
            {
                targets.Add(new Vector3[] { echoPosition });
            }
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

        public void RemoveEcho(int index)
        {
            disabledEchoParticles.Add(activeEchoParticles[0]);
            activeEchoParticles.RemoveAt(0);
            disabledEchoParticles[disabledEchoParticles.Count - 1].gameObject.SetActive(false);
            if (gameController.IsOpenWorldLoaded)
            {
                targets.RemoveAt(index);
            }
            else
            {
                targets.RemoveAt(index);
            }
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