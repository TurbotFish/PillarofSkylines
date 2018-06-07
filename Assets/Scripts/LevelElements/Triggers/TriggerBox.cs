using UnityEngine;
using System.Collections;
using Game.GameControl;

namespace Game.LevelElements
{
    [RequireComponent(typeof(BoxCollider))]
    public class TriggerBox : Trigger
    {
        //###########################################################

        [SerializeField]
        string tagToActivate = "Player";
        [SerializeField]
        bool definitiveActivation;
        [SerializeField]
        float delayBeforeDeactivation;
        [SerializeField]
        bool directionIndependent;

        [SerializeField]
        bool changeMaterial;

        [ConditionalHide("changeMaterial"), SerializeField]
        int materialID = 0;

        [ConditionalHide("changeMaterial"), SerializeField]
        Material on, off;

        [ConditionalHide("changeMaterial"), SerializeField]
        new Renderer renderer;


		[Header("Sound")]
		public bool playsSoundOnStart;
        public AudioClip onClip;
        public AudioClip offClip;
        public float minDistanceStart = 10f;
		public float maxDistanceStart = 50f;
		[Range(0,2)] public float volume = 1f;
		public float clipDuration = 0f;
		public bool addRandomisation = false;
        //###########################################################

        #region editor methods

#if UNITY_EDITOR

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

#endif

        #endregion editor methods

        //###########################################################

        #region monobehaviour methods

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == tagToActivate && (Mathf.Abs(Vector3.Dot(other.transform.up, transform.up)) > 0.9f || directionIndependent))
            {

                if (definitiveActivation && playsSoundOnStart && !TriggerState)
                {
                    SoundifierOfTheWorld.PlaySoundAtLocation(onClip, transform, maxDistanceStart, volume, minDistanceStart, clipDuration, addRandomisation);
                }

                if (Toggle)
                    SetTriggerState(!TriggerState);
                else
                    SetTriggerState(true);


                if (changeMaterial)
                {
                    Material[] sharedMaterialsCopy = renderer.sharedMaterials;
                    sharedMaterialsCopy[materialID] = TriggerState ? on : off;
                    renderer.sharedMaterials = sharedMaterialsCopy;
                }

                if (playsSoundOnStart && !definitiveActivation)
                {
                    if (TriggerState)
    					SoundifierOfTheWorld.PlaySoundAtLocation (onClip, transform, maxDistanceStart, volume, minDistanceStart, clipDuration, addRandomisation);
                    else
                        SoundifierOfTheWorld.PlaySoundAtLocation (offClip, transform, maxDistanceStart, volume, minDistanceStart, clipDuration, addRandomisation);

                }

                GlyphFX _fx = GetComponent<GlyphFX> ();
				if (_fx != null) {
					if (Toggle) {
						
						if (TriggerState) {
							_fx.GlyphOn ();
						} else {
							_fx.GlyphOff ();
						}


					} else {
						_fx.GlyphOn ();
					}


				} else {
					//Debug.LogError ("whoopsie, " + transform.name + " isn't attached to a glyph, is it ?");
				}
            }
        }

        private IEnumerator OnTriggerExit(Collider other)
        {
            if (definitiveActivation || Toggle) yield break;
            if (other.tag == tagToActivate)
            {
                yield return new WaitForSeconds(delayBeforeDeactivation);
                SetTriggerState(false);

				if (changeMaterial) {
					Material[] sharedMaterialsCopy = renderer.sharedMaterials;
					sharedMaterialsCopy [materialID] = off;
					renderer.sharedMaterials = sharedMaterialsCopy;
				}


				GlyphFX _fx = GetComponent<GlyphFX> ();
				if (_fx != null) {
					_fx.GlyphOff ();
				} else {
					Debug.LogError ("whoopsie, " + transform.name + " isn't attached to a glyph, is it ?");
				}
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        #region public methods

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);
			//Debug.Log ("INITIALISE" + transform.name);
            GetComponent<BoxCollider>().isTrigger = true;
        }

        #endregion public methods

        //###########################################################
    }
} //end of namespace