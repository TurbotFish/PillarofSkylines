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
            SceneManager.LoadSceneAsync("UiScene", LoadSceneMode.Additive);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnSceneLoadedEventHandler(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoadedEventHandler;

            if (scene.name == "UiScene")
            {
                var list = new List<Player.UI.UiController>();

                foreach (var gameObject in scene.GetRootGameObjects())
                {
                    list.Add(gameObject.transform.GetComponentInChildren<Player.UI.UiController>());
                }

                this.UiController = list[0];

                this.UiController.InitializeUi(this.playerModel);
            }
        }
    }
} //end of namespace