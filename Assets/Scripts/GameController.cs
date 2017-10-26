using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameController : MonoBehaviour
    {

        [Header("GameController Scripts")]
        [SerializeField]
        Player.PlayerModel playerModel;

        [Header("Scenes")]
        [SerializeField]
        SceneNames sceneNames;

        [Header("")]
        [SerializeField]
        Transform playerTransform;


        Scene uiScene;
        public Player.UI.UiController UiController { get; private set; }

        Scene openWorldScene;
        World.ChunkSystem.WorldController worldController;

        // Use this for initialization
        void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

            StartCoroutine(LoadUiSceneRoutine());
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator LoadUiSceneRoutine()
        {
            yield return null;

            SceneManager.LoadScene(this.sceneNames.UiSceneName, LoadSceneMode.Additive);

            yield return null;

            SceneManager.LoadScene(this.sceneNames.OpenWorldSceneName, LoadSceneMode.Additive);

            yield return null;

            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;
            Utilities.EventManager.SendShowMenuEvent(this, new Utilities.EventManager.OnShowMenuEventArgs(Player.UI.eUiState.Intro));
        }

        void OnSceneLoadedEventHandler(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == this.sceneNames.UiSceneName)
            {
                this.UiController = SearchForScriptInScene<Player.UI.UiController>(scene);
                this.UiController.InitializeUi(this.playerModel);

                this.uiScene = scene;
            }
            else if (scene.name == this.sceneNames.OpenWorldSceneName)
            {
                SceneManager.SetActiveScene(scene);

                this.worldController = SearchForScriptInScene<World.ChunkSystem.WorldController>(scene);
                this.worldController.InitializeWorldController(this.playerTransform);

                this.openWorldScene = scene;
            }
        }

        static T SearchForScriptInScene<T>(Scene scene) where T : class
        {
            T result = null;

            foreach(var gameObject in scene.GetRootGameObjects())
            {
                result = gameObject.GetComponentInChildren<T>();

                if (result != null)
                {
                    break;
                }
            }

            return result;
        }
    }
} //end of namespace