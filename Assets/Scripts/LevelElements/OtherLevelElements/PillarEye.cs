using Game.GameControl;
using Game.Utilities;
using Game.World;
using UnityEngine;
using System.Collections;

namespace Game.LevelElements
{
    public class PillarEye : MonoBehaviour, IInteractable, IWorldObject
    {
        //########################################################################

        [SerializeField] new Renderer renderer;

        [SerializeField] Material regularMat;

        [SerializeField] Vector3 eyeGravity = new Vector3(0, 0, -1);

        [SerializeField] float changeGravityTime = 0.7f, destroyTime = 2.5f;
        [SerializeField] float delayBeforeFadeOut = 1.2f, fadeOutTime = 1.2f;

        [Space]

        [SerializeField] GameObject defaultEye;
        [SerializeField] GameObject eclipseEye;

        [SerializeField] float lookAtDamp = 0.5f;

        private GameController gameController;
        private Transform target;

        //########################################################################

        public void Initialize(GameController gameController)
        {
            this.gameController = gameController;
            target = gameController.PlayerController.CharController.MyTransform;
        }

        private void OnEnable()
        {
            EventManager.EclipseEvent += OnEclipseEventHandler;
        }

        private void OnDisable()
        {
            EventManager.EclipseEvent -= OnEclipseEventHandler;
        }

        //########################################################################

        public Transform Transform { get { return transform; } }

        public bool IsInteractable()
        {
            return gameController.PlayerModel.PlayerHasNeedle;
        }

        //########################################################################

        private void Update()
        {
            if (target != null) //smooth look at
            {
                var rotation = Quaternion.LookRotation(target.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * lookAtDamp);
            }
        }

        public void OnPlayerEnter()
        {
        }

        public void OnPlayerExit()
        {
        }

        public void OnHoverBegin()
        {
            gameController.UiController.Hud.ShowHelpMessage("[X]: Kill the Eye", "PillarEye");
        }

        public void OnHoverEnd()
        {
            gameController.UiController.Hud.HideHelpMessage("PillarEye");
        }

        public void OnInteraction()
        {
            if (!IsInteractable())
            {
                return;
            }

            gameController.UiController.Hud.HideHelpMessage("PillarEye");

            var model = gameController.PlayerModel;

            model.PlayerHasNeedle = false;

            model.SetAbilityState(model.LevelData.GetPillarRewardAbility(gameController.ActivePillarId), Model.AbilityState.active);
            model.SetPillarState(gameController.ActivePillarId, Model.PillarState.Destroyed);

            // ok change here.

            StartCoroutine(_DestructionSequence());
        }

        void OnEclipseEventHandler(object sender, EventManager.EclipseEventArgs args)
        {
            /*if (args.EclipseOn) //eclipse on
            {
                defaultEye.SetActive(false);
                eclipseEye.SetActive(true);
            }
            else //eclipse off
            {
                defaultEye.SetActive(true);
                eclipseEye.SetActive(false);
            }*/
        }


        IEnumerator _DestructionSequence()
        {
            Debug.Log("starting destruction sequence");
            Player.CharacterController.CharController player = gameController.PlayerController.CharController;
            Vector3 eclipseGravity = gameController.EclipseManager.EclipseGravity;

            //gameController.SwitchGameState(GameState.Pause, UI.MenuType.NONE);

            yield return null;

            player.SetHandlingInput(false);

            Debug.Log("starting gravity change");
            for (float elapsed = 0; elapsed < changeGravityTime; elapsed+=Time.deltaTime) {
                player.ChangeGravityDirection(Vector3.Slerp(eclipseGravity, eyeGravity, elapsed / changeGravityTime));
                yield return null;
            }
            Debug.Log("ending gravity change, playing animation");

            // PLAY ANIMATION
            gameController.PlayerController.InteractionController.PutNeedleInHand();
            player.KillPillarEye();

            yield return new WaitForSeconds(1.5f);
            
            StartCoroutine(_WhiteFlash());

            Debug.Log("starting fadeout");
            StartCoroutine(_FadeOut());


            Debug.Log("starting destruction ???");
            for (float elapsed = 0; elapsed < destroyTime; elapsed += Time.deltaTime) {
                renderer.material.SetFloat("_Destruction", Mathf.Pow(elapsed / destroyTime, 2));
                yield return null;
            }

            Debug.Log("ending destruction sequence");

            player.SetHandlingInput(true);
            yield return null;

            // BACK TO NORMAL

            //gameController.SwitchGameState(GameState.Play, UI.MenuType.NONE);
        }

        IEnumerator _FadeOut()
        {
            yield return new WaitForSeconds(delayBeforeFadeOut);

            Eclipse eclipsePostFX = FindObjectOfType<Eclipse>();
            Vector3 luminosityInfluence = eclipsePostFX.LuminosityInfluence;
            Vector3 defaultValue = luminosityInfluence;

            ColorOverlay whiteScreen = eclipsePostFX.gameObject.GetComponent<ColorOverlay>();
            whiteScreen.color = Color.white;
            whiteScreen.intensity = 0;
            whiteScreen.blend = ColorOverlay.BlendMode.Normal;

            for (float elapsed = 0; elapsed < fadeOutTime; elapsed += Time.deltaTime) {
                luminosityInfluence.x += Time.deltaTime * 100;
                eclipsePostFX.LuminosityInfluence = luminosityInfluence;

                whiteScreen.intensity = Mathf.Pow(elapsed / fadeOutTime, 2);

                yield return null;
            }

            eclipsePostFX.LuminosityInfluence = defaultValue;


            Debug.Log("ending fadeout");
            gameController.SwitchToOpenWorld();
            whiteScreen.intensity = 0;
        }

        IEnumerator _WhiteFlash()
        {

            ColorOverlay whiteScreen = FindObjectOfType<ColorOverlay>();
            whiteScreen.color = Color.white;
            whiteScreen.intensity = 0;
            whiteScreen.blend = ColorOverlay.BlendMode.Normal;

            float flashHalfTime = 0.1f;

            for (float elapsed = 0; elapsed < flashHalfTime; elapsed += Time.deltaTime)
            {
                whiteScreen.intensity = Mathf.Pow(elapsed / flashHalfTime, 2);

                yield return null;
            }

            gameController.PlayerController.InteractionController.SetNeedleActive(false);
            for (float elapsed = 0; elapsed < flashHalfTime; elapsed += Time.deltaTime)
            {
                whiteScreen.intensity = 1- Mathf.Pow(elapsed / flashHalfTime, 2);

                yield return null;
            }

        }

        //########################################################################
    }
} // end of namespace