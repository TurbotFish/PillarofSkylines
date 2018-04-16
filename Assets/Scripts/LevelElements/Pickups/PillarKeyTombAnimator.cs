using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.LevelElements
{
    /// <summary>
    /// Class handling the animation of a Tomb containing a PillarKey.
    /// </summary>
    public class PillarKeyTombAnimator : TombAnimator
    {
        //##################################################################

        // FSM: FaveurManager
        [Header("PillarKey FX")]
		public Animator eyeAnim;
		public Light eyeLight;
		public float timeBeforeChange;
		public Material crystalOff;
		public List<MeshRenderer> crystalsTransforming = new List<MeshRenderer>();
		public List<MeshRenderer> crystalsImmediate = new List<MeshRenderer>();
		public List<ParticleSystemRenderer> crystalParticles = new List<ParticleSystemRenderer>();

		[Header("Eclipse")]
		Eclipse _eclipse;
		public float timeInBetween;
		public float intensityMax;
		public float intensitySpeed;
		public float colorVariationSpeed;
		public float colorVariationR;
		public float colorVariationG;
		public float colorVariationB;
        //##################################################################

        public override bool SetTombState(bool isActivated, bool interactWithPlayer, bool doImmediateTransition, TombAnimationFinishedCallback callback = null)
        {
            if (!base.SetTombState(isActivated, interactWithPlayer, doImmediateTransition, callback))
            {
                return false;
            }

            if (isActivated)
            {
                if (interactWithPlayer)
                {
					GetMark ();
                }
            }
            
            return true;
        }



		public void GetMark()
		{
			eyeAnim.SetBool ("marked", true);
			StartCoroutine (EndAnimation ());
			StartCoroutine (HandleEclipse ());

		}
			

		IEnumerator EndAnimation()
		{
			yield return new WaitForSeconds (timeBeforeChange);
			eyeLight.DOIntensity (0, 3).SetEase (Ease.InSine);
			animationFinishedCallback?.Invoke(); // Informs the Pickup that the Tomb has finished its animation.

			for (int i = 0; i < 200; i++) {
				yield return new WaitForSeconds (0.01f);
				foreach (MeshRenderer ms in crystalsTransforming) {
					Material mat = ms.material;
					mat.SetFloat ("_Transition", mat.GetFloat ("_Transition") - 0.005f);
				}
			}
			foreach (MeshRenderer ms in crystalsImmediate) {
				ms.material = crystalOff;
			}
			foreach (ParticleSystemRenderer psr in crystalParticles) {
				psr.material = crystalOff;
			}

		}

		IEnumerator HandleEclipse()
		{

			_eclipse = GameObject.FindObjectOfType<Eclipse> ();
			_eclipse.colorChangeR = 0;
			_eclipse.colorChangeG = 0;
			_eclipse.colorChangeB = 0;
			_eclipse.Intensity = 0;
			_eclipse.enabled = true;
			while (_eclipse.Intensity < intensityMax) {
				yield return new WaitForSeconds (0.01f);
				_eclipse.Intensity += intensitySpeed;
				_eclipse.colorChangeR += colorVariationSpeed;
			}

			yield return new WaitForSeconds (timeInBetween);

			while (_eclipse.Intensity > 0) {
				yield return new WaitForSeconds (0.01f);
				_eclipse.Intensity -= intensitySpeed;
				_eclipse.colorChangeR -= colorVariationSpeed;
			}
			yield return new WaitForSeconds (timeInBetween);
			_eclipse.enabled = false;

		}
     


        //##################################################################
    }
} // end of namespace