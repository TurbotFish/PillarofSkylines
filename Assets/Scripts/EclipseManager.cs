using UnityEngine;
using System.Collections;

namespace Game
{
    public class EclipseManager : MonoBehaviour
    {

        [TestButton("Start Eclipse", "StartEclipse", isActiveInEditor = false)]
        [TestButton("Stop Eclipse", "StopEclipse", isActiveInEditor = false)]
        public bool isEclipseActive;

        public float rotationDuration = 1;
        public Vector3 regularGravity;
        public Vector3 eclipseGravity;

        [SerializeField]
        Transform pillar;

        global::Player player;

        #region Singleton
        public static EclipseManager instance;
        void Awake()
        {
            if (!instance)
            {
                instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
                Destroy(gameObject);
        }
        #endregion

        void Start()
        {
            player = FindObjectOfType<global::Player>(); //to fix

            Utilities.EventManager.OnEclipseEvent += HandleEventEclipse;
        }

        void StartEclipse()
        {
            isEclipseActive = true;
        }

        void StopEclipse()
        {
            isEclipseActive = false;
        }

        void HandleEventEclipse(object sender, Game.Utilities.EventManager.OnEclipseEventArgs args)
        {
            if (args.EclipseOn)
            {
                StartEclipse();
            }
            else
            {
                StopEclipse();
            }
            StartCoroutine("ChangeGravity", args.EclipseOn);
        }

        IEnumerator ChangeGravity(bool eclipseOn)
        {
            float gravityTimer = 0;
            player.SetVelocity(new Vector3(0f, 10f, 0f), false, false);
            while (gravityTimer < rotationDuration)
            {
                if (eclipseOn)
                {
                    player.ChangeGravityDirection(Vector3.Lerp(regularGravity, eclipseGravity, gravityTimer / rotationDuration));
                }
                else
                {
                    player.ChangeGravityDirection(Vector3.Lerp(eclipseGravity, regularGravity, gravityTimer / rotationDuration));
                }
                gravityTimer += Time.deltaTime;
                yield return null;
            }
            if (eclipseOn)
            {
                player.ChangeGravityDirection(eclipseGravity);
            }
            else
            {
                player.ChangeGravityDirection(regularGravity);
            }
        }
    }
} //end of namespace