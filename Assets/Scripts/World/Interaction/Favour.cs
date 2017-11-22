using System.Collections;
using System.Collections.Generic;
using Game.World.ChunkSystem;
using UnityEngine;

namespace Game.World.Interaction
{
    public class Favour : MonoBehaviour, IWorldObject
    {
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

        void IWorldObject.InitializeWorldObject(WorldController worldController)
        {
            if (!this.instanceIdSet)
            {
                this.instanceId = this.GetInstanceID();
                this.instanceIdSet = true;
            }

            this.MyTransform = this.transform;
            this.myCollider = GetComponent<BoxCollider>();

            Utilities.EventManager.FavourPickedUpEvent += OnFavourPickedUpEventHandler;

            worldController.RegisterFavour(this);
        }

        void OnFavourPickedUpEventHandler(object sender, Utilities.EventManager.FavourPickedUpEventArgs args)
        {
            if (args.FavourId == this.instanceId)
            {
                if (!this.FavourPickedUp)
                {
                    this.FavourPickedUp = true;

                    if (this.myCollider != null || !this.myCollider.Equals(null)) //check because all colliders are removed in the duplicated worlds
                    {
                        this.myCollider.enabled = false;
                    }
                }
            }
        }
    }
} //end of namespace