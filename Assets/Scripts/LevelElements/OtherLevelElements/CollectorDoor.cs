using Game.GameControl;
using Game.Model;
using Game.World;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    public class CollectorDoor : PersistentLevelElement<FireflyDoorPersistentData>, IInteractable
    {
        //##################################################################

        // -- CONSTANTS

        //[TestButton("Update Collectibles Amount", "UpdateCollectiblesAmount", isActiveInEditor = true)]

        //[SerializeField] int totalCollectibles;

        [SerializeField] GameObject FireflyPrefab;
        [SerializeField] Transform[] luluDestinations;

        [SerializeField] Animator doorAnimator;
        [SerializeField] string animBoolToOpen;

        //##################################################################

        // -- ATTRIBUTES

        int collected;

        private Transform MyTransform;

        private List<Transform> FilledLuluDestinations = new List<Transform>();
        private List<Transform> EmptyLuluDestinations = new List<Transform>();

        //##################################################################

        // -- INITIALIZATION

        //private void Start()
        //{
        //    if (luluDestinations.Length < totalCollectibles)
        //        Debug.LogError("There are less lulu destinations than the given amount of lulu in the world. Either update collectibles amount or add more destinations.");
        //}

        public override void Initialize(GameController game_controller)
        {
            base.Initialize(game_controller);

            for (int destination_index = 0; destination_index < PersistentData.CollectedFirefliesCount; destination_index++)
            {
                var firefly = Instantiate(FireflyPrefab, Transform.position, Transform.rotation).GetComponent<Firefly>();
                var destination = luluDestinations[destination_index];

                firefly.SetParent(destination, false, Firefly.FireflyState.Static);
                FilledLuluDestinations.Add(destination);
            }

            for (int destination_index = PersistentData.CollectedFirefliesCount; destination_index < TotalNeededFireflies; destination_index++)
            {
                EmptyLuluDestinations.Add(luluDestinations[destination_index]);
            }

            if (PersistentData.DoorOpened)
            {
                doorAnimator.SetBool(animBoolToOpen, true);
            }
        }

        //##################################################################

        // -- INQUIRIES

        public int TotalNeededFireflies { get { return luluDestinations.Length; } }

        public int CurrentNeededFireflies { get { return TotalNeededFireflies - FilledLuluDestinations.Count; } }

        public Transform Transform { get { if (MyTransform == null) { MyTransform = this.transform; } return MyTransform; } }

        public bool IsInteractable()
        {
            return false;
        }

        //##################################################################

        // -- OPERATIONS

        public void OnPlayerEnter()
        {
            if (PersistentData.DoorOpened || GameController.PlayerModel.GetFireflyCount() == 0)
            {
                return;
            }

            int new_fireflies = Mathf.Min(CurrentNeededFireflies, GameController.PlayerModel.GetFireflyCount());

            for (int firefly_count = 0; firefly_count < new_fireflies; firefly_count++)
            {
                var destination = luluDestinations[PersistentData.CollectedFirefliesCount];
                var firefly = GameController.PlayerModel.PopFirefly();

                firefly.SetParent(destination, false, Firefly.FireflyState.Static);
                FilledLuluDestinations.Add(destination);
                PersistentData.CollectedFirefliesCount++;
            }

            if (CurrentNeededFireflies == 0)
            {
                doorAnimator.SetBool(animBoolToOpen, true);
            }
        }

        public void OnPlayerExit()
        {
        }

        public void OnHoverBegin()
        {
            throw new System.NotImplementedException();
        }

        public void OnHoverEnd()
        {
            throw new System.NotImplementedException();
        }

        public void OnInteraction()
        {
            throw new System.NotImplementedException();
        }

        protected override PersistentData CreatePersistentDataObject()
        {
            return new FireflyDoorPersistentData(UniqueId);
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    FireflyPickup lulu = other.GetComponent<FireflyPickup>();

        //    if (lulu && !lulu.collected)
        //    {
        //        lulu.GoToCollector(luluDestinations[collected]);
        //        collected++;

        //        if (collected >= totalCollectibles)
        //        {
        //            // la porte se dissolve et ouvre le passage
        //            //Destroy(transform.GetChild(0).gameObject);
        //            doorAnimator.SetBool(animBoolToOpen, true);
        //        }
        //    }
        //}

        //private void UpdateCollectiblesAmount()
        //{
        //    totalCollectibles = FindObjectsOfType<FireflyPickup>().Length;
        //}

    }
} // end of namespace