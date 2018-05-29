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

        // -- ATTRIBUTES

        // FSM: FaveurManager
        [Header("PillarKey FX")]
        [SerializeField] public Animator eyeAnim;
        [SerializeField] public Light eyeLight;
        [SerializeField] public float timeBeforeChange;
        [SerializeField] public Material crystalOff;
        [SerializeField] public List<MeshRenderer> crystalsTransforming = new List<MeshRenderer>();
        [SerializeField] public List<MeshRenderer> crystalsImmediate = new List<MeshRenderer>();
        [SerializeField] public List<ParticleSystemRenderer> crystalParticles = new List<ParticleSystemRenderer>();

        [Header("Eclipse")]
        [SerializeField] public float timeInBetween;
        [SerializeField] public float intensityMax;
        [SerializeField] public float intensitySpeed;
        [SerializeField] public float colorVariationSpeed;
        [SerializeField] public float colorVariationR;
        [SerializeField] public float colorVariationG;
        [SerializeField] public float colorVariationB;

        private Eclipse _eclipse;

        //##################################################################

        // -- OPERATIONS

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
                    GetMark();
                }
            }

            return true;
        }

        private void GetMark()
        {
            eyeAnim.SetBool("marked", true);
            StartCoroutine(EndAnimation());
            StartCoroutine(HandleEclipse());
        }

        private IEnumerator EndAnimation()
        {
            yield return new WaitForSeconds(timeBeforeChange);
            eyeLight.DOIntensity(0, 3).SetEase(Ease.InSine);
            animationFinishedCallback?.Invoke(); // Informs the Pickup that the Tomb has finished its animation.

            for (int i = 0; i < 200; i++)
            {
                yield return new WaitForSeconds(0.01f);
                foreach (MeshRenderer ms in crystalsTransforming)
                {
                    Material mat = ms.material;
                    mat.SetFloat("_Transition", mat.GetFloat("_Transition") - 0.005f);
                }
            }
            foreach (MeshRenderer ms in crystalsImmediate)
            {
				
                ms.material = crystalOff;
            }
            foreach (ParticleSystemRenderer psr in crystalParticles)
            {
                psr.material = crystalOff;
            }
        }

        private IEnumerator HandleEclipse()
        {
            _eclipse = GameObject.FindObjectOfType<Eclipse>();
            _eclipse.colorChangeR = 0;
            _eclipse.colorChangeG = 0;
            _eclipse.colorChangeB = 0;
            _eclipse.Intensity = 0;
            _eclipse.enabled = true;
            while (_eclipse.Intensity < intensityMax)
            {
                yield return new WaitForSeconds(0.01f);
                _eclipse.Intensity += intensitySpeed;
                _eclipse.colorChangeR += colorVariationSpeed;
            }

            yield return new WaitForSeconds(timeInBetween);

            while (_eclipse.Intensity > 0)
            {
                yield return new WaitForSeconds(0.01f);
                _eclipse.Intensity -= intensitySpeed;
                _eclipse.colorChangeR -= colorVariationSpeed;
            }
            yield return new WaitForSeconds(timeInBetween);
            _eclipse.enabled = false;
        }
    }
} // end of namespace