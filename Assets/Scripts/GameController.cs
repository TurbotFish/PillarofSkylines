using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        Camera mainCamera;
        

        // Use this for initialization
        void Start()
        {
            SceneManager.LoadSceneAsync("UiScene", LoadSceneMode.Additive);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
} //end of namespace