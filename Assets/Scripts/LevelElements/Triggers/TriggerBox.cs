using UnityEngine;
using System.Collections;
using Game.GameControl;

namespace Game.LevelElements
{
    [RequireComponent(typeof(BoxCollider))]
    public class TriggerBox : Trigger, IInteractable
    {
        //###########################################################

        // -- CONSTANTS

        [Header("TriggerBox")]
        [SerializeField] private string tagToActivate = "Player";
        [SerializeField] private bool definitiveActivation;
        [SerializeField] private float delayBeforeDeactivation;
        [SerializeField] private bool directionIndependent;
        [SerializeField] private bool changeMaterial;

        [ConditionalHide("changeMaterial"), SerializeField] private int materialID = 0;
        [ConditionalHide("changeMaterial"), SerializeField] private Material on;
        [ConditionalHide("changeMaterial"), SerializeField] private Material off;
        [ConditionalHide("changeMaterial"), SerializeField] private new Renderer renderer;


        [Header("Sound")]
        public bool playsSoundOnStart;
        public AudioClip onClip;
        public AudioClip offClip;
        public float minDistanceStart = 10f;
        public float maxDistanceStart = 50f;
        [Range(0, 2)] public float volume = 1f;
        public float clipDuration = 0f;
        public bool addRandomisation = false;

        //###########################################################

        // -- ATTRIBUTES

        private Transform MyTransform;
        private bool isInitialized;

        //###########################################################

        // -- INITIALIZATION

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);
            isInitialized = true;
            GetComponent<BoxCollider>().isTrigger = true;
        }

        //###########################################################

        // -- INQUIRIES

        public Transform Transform
        {
            get
            {
                if (MyTransform == null)
                {
                    MyTransform = this.transform;
                }

                return MyTransform;
            }
        }

        public bool IsInteractable()
        {
            return false;
        }

        //###########################################################

        // -- OPERATIONS

        public void OnPlayerEnter()
        {
            if (!isInitialized)
                return;
            
            /*
             * gravity check
             */
            if (Mathf.Abs(Vector3.Dot(GameController.PlayerController.Transform.up, transform.up)) > 0.9f || directionIndependent)
            {
                if (Toggle)
                {
                    SetTriggerState(!TriggerState);
                }
                else
                {
                    SetTriggerState(true);
                }
            }
        }

        public void OnPlayerExit()
        {
            if (!definitiveActivation && !Toggle)
            {
                StartCoroutine(DelayedSetTriggerState(false));
            }
        }

        public void OnHoverBegin()
        {
            throw new System.NotImplementedException();
        }

        public void OnHoverEnd()
        {
            throw new System.NotImplementedException();
        }

        public void OnInteraction()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Called when the state of the trigger changes.
        /// </summary>
        /// <param name="old_state"></param>
        /// <param name="new_state"></param>
        protected override void OnTriggerStateChanged(bool old_state, bool new_state)
        {
            if (TriggerState)
            {
                ActivateTriggerBox();
            }
            else
            {
                DeactivateTriggerBox();
            }
        }

        /// <summary>
        /// This calls Trigger.SetTriggerState with a delay.
        /// </summary>
        /// <param name="new_trigger_state"></param>
        /// <returns></returns>
        private IEnumerator DelayedSetTriggerState(bool new_trigger_state)
        {
            yield return new WaitForSeconds(delayBeforeDeactivation);

            SetTriggerState(new_trigger_state);
        }

        /// <summary>
        /// EDITOR: OnDrawGizmos
        /// </summary>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            BoxCollider box = GetComponent<BoxCollider>();
            box.isTrigger = true;

            if (Targets == null || Targets.Count == 0)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;

            Gizmos.DrawWireCube(box.center, box.size);
        }

        /// <summary>
        /// Activates the TriggerBox.
        /// </summary>
        private void ActivateTriggerBox()
        {
            //Debug.LogErrorFormat("TriggerBox {0}: ActivateTrigger called!", this.name);

            /*
             * Sound
             */
            if (playsSoundOnStart)
            {
                SoundifierOfTheWorld.PlaySoundAtLocation(onClip, transform, maxDistanceStart, volume, minDistanceStart, clipDuration, addRandomisation);
            }

            /*
             * Material
             */
            if (changeMaterial)
            {
                Material[] sharedMaterialsCopy = renderer.sharedMaterials;
                sharedMaterialsCopy[materialID] = on;
                renderer.sharedMaterials = sharedMaterialsCopy;
            }

            /*
             * Glyph
             */
            GlyphFX _fx = GetComponent<GlyphFX>();
            if (_fx != null)
            {
                _fx.GlyphOn();
            }
            else
            {
                Debug.LogError ("whoopsie, " + transform.name + " isn't attached to a glyph, is it ?");
            }
        }

        /// <summary>
        /// Deactivates the TriggerBox.
        /// </summary>
        /// <returns></returns>
        private void DeactivateTriggerBox()
        {
            if (definitiveActivation)
            {
                Debug.LogErrorFormat("TriggerBox {0}: DeactivateTriggerBoxCoroutine: definitiveActivation=true!", this.name);
                return;
            }

            /*
             * Sound
             */
            if (playsSoundOnStart)
            {
                SoundifierOfTheWorld.PlaySoundAtLocation(offClip, transform, maxDistanceStart, volume, minDistanceStart, clipDuration, addRandomisation);
            }

            /*
             * Material
             */
            if (changeMaterial)
            {
                Material[] sharedMaterialsCopy = renderer.sharedMaterials;
                sharedMaterialsCopy[materialID] = off;
                renderer.sharedMaterials = sharedMaterialsCopy;
            }

            /*
             * Glyph
             */
            GlyphFX _fx = GetComponent<GlyphFX>();
            if (_fx != null)
            {
                _fx.GlyphOff();
            }
            else
            {
                Debug.LogWarning("whoopsie, " + transform.name + " isn't attached to a glyph, is it ?");
            }
        }

        ///// <summary>
        ///// Monobehaviour: OnTriggerEnter
        ///// </summary>
        ///// <param name="other"></param>
        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.tag == tagToActivate && (Mathf.Abs(Vector3.Dot(other.transform.up, transform.up)) > 0.9f || directionIndependent))
        //    {

        //        if (definitiveActivation && playsSoundOnStart && !TriggerState)
        //        {
        //            SoundifierOfTheWorld.PlaySoundAtLocation(onClip, transform, maxDistanceStart, volume, minDistanceStart, clipDuration, addRandomisation);
        //        }

        //        if (Toggle)
        //            SetTriggerState(!TriggerState);
        //        else
        //            SetTriggerState(true);


        //        if (changeMaterial)
        //        {
        //            Material[] sharedMaterialsCopy = renderer.sharedMaterials;
        //            sharedMaterialsCopy[materialID] = TriggerState ? on : off;
        //            renderer.sharedMaterials = sharedMaterialsCopy;
        //        }

        //        if (playsSoundOnStart && !definitiveActivation)
        //        {
        //            if (TriggerState)
        //                SoundifierOfTheWorld.PlaySoundAtLocation(onClip, transform, maxDistanceStart, volume, minDistanceStart, clipDuration, addRandomisation);
        //            else
        //                SoundifierOfTheWorld.PlaySoundAtLocation(offClip, transform, maxDistanceStart, volume, minDistanceStart, clipDuration, addRandomisation);

        //        }

        //        GlyphFX _fx = GetComponent<GlyphFX>();
        //        if (_fx != null)
        //        {
        //            if (Toggle)
        //            {

        //                if (TriggerState)
        //                {
        //                    _fx.GlyphOn();
        //                }
        //                else
        //                {
        //                    _fx.GlyphOff();
        //                }


        //            }
        //            else
        //            {
        //                _fx.GlyphOn();
        //            }


        //        }
        //        else
        //        {
        //            //Debug.LogError ("whoopsie, " + transform.name + " isn't attached to a glyph, is it ?");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Monobehaviour: OnTriggerExit
        ///// </summary>
        ///// <param name="other"></param>
        ///// <returns></returns>
        //private IEnumerator OnTriggerExit(Collider other)
        //{
        //    if (definitiveActivation || Toggle)
        //    {
        //        yield break;
        //    }
        //    else if (other.tag == tagToActivate)
        //    {
        //        yield return new WaitForSeconds(delayBeforeDeactivation);
        //        SetTriggerState(false);

        //        if (changeMaterial)
        //        {
        //            Material[] sharedMaterialsCopy = renderer.sharedMaterials;
        //            sharedMaterialsCopy[materialID] = off;
        //            renderer.sharedMaterials = sharedMaterialsCopy;
        //        }


        //        GlyphFX _fx = GetComponent<GlyphFX>();
        //        if (_fx != null)
        //        {
        //            _fx.GlyphOff();
        //        }
        //        else
        //        {
        //            Debug.LogWarning("whoopsie, " + transform.name + " isn't attached to a glyph, is it ?");
        //        }
        //    }
        //}
    }
} //end of namespace