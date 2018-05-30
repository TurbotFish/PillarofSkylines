using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameControl
{
    public class Cutscene : MonoBehaviour, IWorldObject
    {
        //########################################################################

        // -- CONSTANTS

        [SerializeField] private CutsceneType CutsceneType;
        [SerializeField] private PlayMakerFSM PlayMakerFSM;

        //########################################################################

        // -- ATTRIBUTES

        private GameController GameController;

        //########################################################################

        // -- INITIALIZATION

        public void Initialize(GameController game_controller)
        {
            GameController = game_controller;

            GameController.RegisterCutscene(CutsceneType, this);
        }

        //########################################################################

        // -- OPERATIONS

        public void StartCutscene()
        {
            if(PlayMakerFSM != null)
            {
                PlayMakerFSM.enabled = true;
            }
        }

        public void OnCutsceneOver()
        {
            PlayMakerFSM.enabled = false;

            GameController.OnCutsceneEnded();
        }
    }
}