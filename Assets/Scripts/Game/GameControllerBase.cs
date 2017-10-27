using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public abstract class GameControllerBase : MonoBehaviour
    {
        public const string UI_SCENE_NAME = "UiScene";

        [Header("GameController Scripts")]
        [SerializeField]
        protected Player.PlayerModel playerModel;
        public Player.PlayerModel PlayerModel { get { return this.playerModel; } }



        protected Scene uiScene;
        public Player.UI.UiController UiController { get; protected set; }

        protected virtual void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoadedEventHandler;

            StartCoroutine(LoadScenesRoutine());
        }

        protected abstract IEnumerator LoadScenesRoutine();

        protected abstract void OnSceneLoadedEventHandler(Scene scene, LoadSceneMode mode);

        protected static T SearchForScriptInScene<T>(Scene scene) where T : class
        {
            T result = null;

            foreach (var gameObject in scene.GetRootGameObjects())
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
}