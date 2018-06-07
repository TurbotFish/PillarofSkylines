using Game.GameControl;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Cutscene
{
    public class CutsceneManager
    {
        //###############################################################

        // -- ATTRIBUTES

        public bool IsRunningCutscene { get; private set; }
        public CutsceneType CurrentCutsceneType { get; private set; }

        private GameController GameController;
        private Dictionary<CutsceneType, Cutscene> CutsceneDictionary = new Dictionary<CutsceneType, Cutscene>();

        //###############################################################

        // -- INITIALIZATION

        public CutsceneManager(GameController game_controller)
        {
            GameController = game_controller;
        }

        //###############################################################

        // -- OPERATIONS

        /// <summary>
        /// Registers a Cutscene with the GameController.
        /// </summary>
        /// <param name="cutscene_type"></param>
        /// <param name="cutscene"></param>
        public void RegisterCutscene(CutsceneType cutscene_type, Cutscene cutscene)
        {
            if (CutsceneDictionary.ContainsKey(cutscene_type))
            {
                CutsceneDictionary[cutscene_type] = cutscene;
            }
            else
            {
                CutsceneDictionary.Add(cutscene_type, cutscene);
            }
        }

        /// <summary>
        /// Starts a Cutscene.
        /// </summary>
        /// <param name="cutscene_type"></param>
        public void PlayCutscene(CutsceneType cutscene_type, bool hide_ui)
        {
            if (IsRunningCutscene)
            {
                Debug.LogError("GameController: PlayCutscene: a Cutscene is already running");
            }
            else if (!CutsceneDictionary.ContainsKey(cutscene_type))
            {
                Debug.LogErrorFormat("GameController: PlayCutscene: no Cutscene of type {0}", cutscene_type);
            }
            else
            {
                GameController.SwitchGameState(GameState.Pause, hide_ui ? MenuType.NONE : MenuType.HUD);
                IsRunningCutscene = true;
                CutsceneDictionary[cutscene_type].StartCutscene();
            }
        }

        /// <summary>
        /// Called when a Cutscene is over.
        /// </summary>
        public void OnCutsceneEnded()
        {
            if (IsRunningCutscene)
            {
                IsRunningCutscene = false;
                GameController.SwitchGameState(GameState.Play, MenuType.HUD);
            }
        }
    }
}