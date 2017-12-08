using System.Collections;
using System.Collections.Generic;
using Game.World.ChunkSystem;
using UnityEngine;

namespace Game.World.Interaction
{
    public class Favour : MonoBehaviour, IWorldObjectInitialization
    {
        //##################################################################

        [SerializeField]
        [HideInInspector]
        int instanceId = -1;
        public int InstanceId { get { return this.instanceId; } }

        [SerializeField]
        [HideInInspector]
        bool instanceIdSet = false;

        public bool FavourPickedUp { get; private set; }
        public Transform MyTransform { get; private set; }
        BoxCollider myCollider;

        //##################################################################

        void IWorldObjectInitialization.Initialize(WorldController worldController, bool isCopy)
        {
            if (!instanceIdSet)
            {
                instanceId = GetInstanceID();
                instanceIdSet = true;
            }

            MyTransform = transform;
            myCollider = GetComponent<BoxCollider>();
            
            Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;

            worldController.RegisterFavour(this);
        }

        //##################################################################

        void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
        {
            if (args.FavourId == instanceId)
            {
                if (!FavourPickedUp)
                {
                    FavourPickedUp = true;

                    if (myCollider != null || !myCollider.Equals(null)) //check because all colliders are removed in the duplicated worlds
                    {
                        myCollider.enabled = false;
                    }
                }
            }
        }

        //##################################################################
    }
} //end of namespace