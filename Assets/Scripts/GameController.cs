using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        Player.PlayerModel playerModel;

        public Player.UI.UiController UiController { get; private set; }


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
            
            SceneManager.LoadSceneAsync("UiScene", LoadSceneMode.Additive);
        }

        void OnSceneLoadedEventHandler(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "UiScene")
            {
                var list = new List<Player.UI.UiController>();

                foreach (var gameObject in scene.GetRootGameObjects())
                {
                    list.Add(gameObject.transform.GetComponentInChildren<Player.UI.UiController>());
                }

                this.UiController = list[0];
                this.UiController.InitializeUi(this.playerModel, Player.UI.eUiState.Intro);
            }
        }
    }
} //end of namespace