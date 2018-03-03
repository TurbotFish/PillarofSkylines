﻿using UnityEngine;
using System.Collections;
using Game.GameControl;

namespace Game.LevelElements
{
    [RequireComponent(typeof(BoxCollider))]
    public class TriggerReset : Trigger
    {
        //###########################################################

        [SerializeField]
        private string tagToActivate = "Player";
        [SerializeField]
        private bool definitiveActivation;
        [SerializeField]
        private float delayBeforeDeactivation;

        [SerializeField]
        private bool changeMaterial;

        [ConditionalHide("changeMaterial"), SerializeField]
        private int materialID = 0;

        [ConditionalHide("changeMaterial"), SerializeField]
        private Material on, off;

        [ConditionalHide("changeMaterial"), SerializeField]
        private new Renderer renderer;

        //###########################################################

        #region monobehaviour methods

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == tagToActivate)
            {
                /*if (Toggle)
                    TriggerState ^= true;
                else*/
                SetTriggerState(false);

                if (changeMaterial)
                {
                    Material[] sharedMaterialsCopy = renderer.sharedMaterials;
                    sharedMaterialsCopy[materialID] = TriggerState ? on : off;
                    renderer.sharedMaterials = sharedMaterialsCopy;
                }
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        #region public methods

        public override void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            base.Initialize(gameController, isCopy);

            GetComponent<BoxCollider>().isTrigger = true;
        }

        #endregion public methods

        //###########################################################
    }
} //end of namespace