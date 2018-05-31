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

        [SerializeField] Material regularMat, destroyedMat;

        [SerializeField] Vector3 eyeGravity = new Vector3(0, 0, 1);

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
            var eventArgs = new EventManager.OnShowHudMessageEventArgs(true, "[X]: Plant Needle");
            EventManager.SendShowHudMessageEvent(this, eventArgs);
        }

        public void OnHoverEnd()
        {
            var eventArgs = new EventManager.OnShowHudMessageEventArgs(false);
            EventManager.SendShowHudMessageEvent(this, eventArgs);
        }

        public void OnInteraction()
        {
            if (!IsInteractable())
            {
                return;
            }

            var showMessageEventArgs = new EventManager.OnShowHudMessageEventArgs(false);
            EventManager.SendShowHudMessageEvent(this, showMessageEventArgs);

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
            Player.CharacterController.CharController player = gameController.PlayerController.CharController;
            Vector3 eclipseGravity = gameController.EclipseManager.eclipseGravity;

            gameController.SwitchGameState(GameState.Pause, UI.MenuType.NONE);

            yield return null;

            for (float elapsed = 0; elapsed < changeGravityTime; elapsed+=Time.deltaTime) {
                player.ChangeGravityDirection(Vector3.Lerp(eclipseGravity, eyeGravity, elapsed / changeGravityTime));
                yield return null;
            }

            // PLAY ANIMATION

            StartCoroutine(_FadeOut());

            for (float elapsed = 0; elapsed < changeGravityTime; elapsed += Time.deltaTime) {
                renderer.sharedMaterial.Lerp(regularMat, destroyedMat, elapsed / changeGravityTime);
                yield return null;
            }
            
            yield return null;
            
            // BACK TO NORMAL

            gameController.SwitchGameState(GameState.Play, UI.MenuType.NONE);
            gameController.SwitchToOpenWorld();
        }

        IEnumerator _FadeOut()
        {
            yield return new WaitForSeconds(delayBeforeFadeOut);

            Eclipse eclipsePostFX = FindObjectOfType<Eclipse>();
            Vector3 luminosityInfluence = eclipsePostFX.LuminosityInfluence;
            Vector3 defaultValue = luminosityInfluence;

            ColorOverlay whiteScreen = eclipsePostFX.gameObject.AddComponent<ColorOverlay>();
            whiteScreen.color = Color.white;
            whiteScreen.intensity = 0;
            
            for (float elapsed = 0; elapsed < changeGravityTime; elapsed += Time.deltaTime) {
                luminosityInfluence.x += Time.deltaTime * 10;
                eclipsePostFX.LuminosityInfluence = luminosityInfluence;

                whiteScreen.intensity = Mathf.Pow(elapsed / changeGravityTime, 2);

                yield return null;
            }

            eclipsePostFX.LuminosityInfluence = defaultValue;
            Destroy(whiteScreen);
        }



        //########################################################################
    }
} // end of namespace