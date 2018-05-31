using Game.GameControl;
using Game.Model;
using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    public class AirParticleOrigin : PersistentLevelElement<AirParticlePersistentData>, IInteractable
    {
        //########################################################################

        [SerializeField] private GameObject airParticlePrefab;

        private AirParticle airParticle;

        //########################################################################

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);

            if(airParticle == null && !PersistentData.IsPicked && !PersistentData.HasBeenReceived)
            {
                airParticle = Instantiate(airParticlePrefab, transform).GetComponent<AirParticle>();
            }
        }

        //########################################################################

        public Transform Transform { get { return transform; } }

        public bool IsInteractable()
        {
            return false;
        }

        //########################################################################

        public void OnPlayerEnter()
        {
        }

        public void OnPlayerExit()
        {
        }

        public void OnHoverBegin()
        {
        }

        public void OnHoverEnd()
        {
        }

        public void OnInteraction()
        {
        }

        protected override PersistentData CreatePersistentDataObject()
        {
            return new AirParticlePersistentData(UniqueId);
        }

        private void Update()
        {
            
        }

        //########################################################################
    }
} // end of namespace