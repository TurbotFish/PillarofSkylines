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
        [SerializeField] public float intensityChangeDuration = 1f;
        [SerializeField] public float crystalDissolveDuration = 3f;
        [SerializeField] public float colorVariationR;
        [SerializeField] public float colorVariationG;
        [SerializeField] public float colorVariationB;


        [Header("Sound")]
        [SerializeField] private AudioClip getClip;
        [SerializeField, Range(0, 2)] private float volumeGet = 1f;
        [SerializeField] private bool addRandomisationGet = false;
        [SerializeField] private float minDistance = 10f;
        [SerializeField] private float maxDistance = 50f;
        [SerializeField] private float clipDuration = 0f;

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
            SoundifierOfTheWorld.PlaySoundAtLocation(getClip, transform, maxDistance, volumeGet, minDistance, 0f, addRandomisationGet, false, .2f);
            eyeAnim.SetBool("marked", true);
            StartCoroutine(EndAnimation());
            StartCoroutine(HandleEclipse());
        }

        private IEnumerator EndAnimation()
        {
            yield return new WaitForSeconds(timeBeforeChange);
            eyeLight.DOIntensity(0, 3).SetEase(Ease.InSine);
            animationFinishedCallback?.Invoke(); // Informs the Pickup that the Tomb has finished its animation.

            float transitionValue = 1;
            for(float elapsed = 0; elapsed < crystalDissolveDuration; elapsed+=Time.deltaTime)
            {
                transitionValue = Mathf.Lerp(1, 0, elapsed / crystalDissolveDuration);
                foreach (MeshRenderer ms in crystalsTransforming) {
                    Material mat = ms.material;
                    mat.SetFloat("_Transition", transitionValue);
                }
                yield return null;
            }

            foreach (MeshRenderer ms in crystalsTransforming)
                ms.material = crystalOff;
            foreach (MeshRenderer ms in crystalsImmediate)
                ms.material = crystalOff;
            foreach (ParticleSystemRenderer psr in crystalParticles)
                psr.material = crystalOff;
        }

        private IEnumerator HandleEclipse()
        {
            _eclipse = FindObjectOfType<Eclipse>();
            _eclipse.colorChangeR = 0;
            _eclipse.colorChangeG = 0;
            _eclipse.colorChangeB = 0;
            _eclipse.Intensity = 0;
            _eclipse.enabled = true;

            for (float elapsed = 0; elapsed < intensityChangeDuration; elapsed+=Time.deltaTime) {
                _eclipse.Intensity = Mathf.Lerp(0, intensityMax, elapsed / intensityChangeDuration);
                _eclipse.colorChangeR = Mathf.Lerp(0, 1, elapsed / intensityChangeDuration);
                yield return null;
            }
            yield return new WaitForSeconds(timeInBetween);

            for (float elapsed = 0; elapsed < intensityChangeDuration; elapsed += Time.deltaTime) {
                _eclipse.Intensity = Mathf.Lerp(intensityMax, 0, elapsed / intensityChangeDuration);
                _eclipse.colorChangeR = Mathf.Lerp(1, 0, elapsed / intensityChangeDuration);
                yield return null;
            }

            _eclipse.ResetColourChange();
            _eclipse.enabled = false;
        }
    }
} // end of namespace