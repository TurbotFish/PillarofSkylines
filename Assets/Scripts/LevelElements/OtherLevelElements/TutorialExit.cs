using Game.GameControl;
using Game.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    public class TutorialExit : PersistentLevelElement<TutorialExitPersistentData>, IInteractable
    {
        //########################################################################

        // -- ATTRIBUTES

        private Transform MyTransform;

        //########################################################################

        // -- INITIALIZATION

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);

            if (!PersistentData.IsOpen)
            {
                this.gameObject.SetActive(false);
            }
        }

        //########################################################################

        // -- INQUIRIES

        public Transform Transform
        {
            get
            {
                if (MyTransform == null)
                {
                    MyTransform = this.transform;
                }

                return MyTransform;
            }
        }

        public bool IsInteractable()
        {
            return false;
        }

        //########################################################################

        // -- OPERATIONS

        public void OnPlayerEnter()
        {
        }

        public void OnPlayerExit()
        {
            if (!PersistentData.IsOpen)
            {
                Debug.LogError("TutorialExit: OnPlayerExit: Exit was already closed!");
            }

            PersistentData.IsOpen = false;
            this.gameObject.SetActive(false);
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
            return new TutorialExitPersistentData(UniqueId)
            {
                IsOpen = true
            };
        }
    }
} // end of namespace