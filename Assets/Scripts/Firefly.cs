using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class Firefly : MonoBehaviour
    {
        public enum FireflyState
        {
            Idle,
            Dancing,
            Following,
            Static
        }

        //########################################################################

        // -- CONSTANTS

        [SerializeField] private ParticleSystem Feedback;
        [SerializeField] private Light Light;
        [SerializeField] private TrailRenderer TrailRenderer;

        [Header("")]
        [SerializeField] private float Speed = 4f;
        [SerializeField] private float MaxRadius = 0.3f;
        [SerializeField] private float MinRadius = 0.7f;
        [SerializeField] private float DanceDuration = 0.8f;
        [SerializeField] private float DanceAngle = 1080; // 360 * 3
        [SerializeField] private Vector3 IdleScales = new Vector3(1, 0.3f, 0.3f);
        [SerializeField] private float MinIntensity = 0.5f;
        [SerializeField] private float MaxIntensity = 2f;

        [Header("Sound")]
        [SerializeField] private AudioClip Clip;
        [SerializeField, Range(0, 2)] private float Volume = 1f;
        [SerializeField] private bool AddRandomisation = false;
        [SerializeField] private float MinDistance = 10f;
        [SerializeField] private float MaxDistance = 50f;
        [SerializeField] private float ClipDuration = 0f;

        //########################################################################

        // -- ATTRIBUTES

        private Transform MyTransform;
        private Transform ParentTransform;

        private FireflyState CurrentState = FireflyState.Idle;

        private Vector3 FollowOffset;
        private List<ParticleSystem> ParticleSystemList;

        //########################################################################

        // -- INITIALIZATION

        void Start()
        {
            Utilities.EventManager.TeleportPlayerEvent += OnTeleportPlayerEvent;

            ParticleSystemList = GetComponentsInChildren<ParticleSystem>().ToList();
        }

        private void OnDestroy()
        {
            Utilities.EventManager.TeleportPlayerEvent -= OnTeleportPlayerEvent;
        }

        //########################################################################

        // -- INQUIRIES

        public Transform Transform { get { if (MyTransform == null) { MyTransform = this.transform; } return MyTransform; } }

        //########################################################################

        // -- OPERATIONS

        /// <summary>
        /// Sets the parent and mode of the Firefly.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="do_dance"></param>
        /// <param name="follow_parent"></param>
        public void SetParent(Transform parent, bool do_dance, FireflyState new_state)
        {
            ParentTransform = parent;

            Transform.SetParent(parent, true);
            SetFollowOffset();

            /*
             * State dependent changes
             */
            if (new_state == FireflyState.Following)
            {
                Instantiate(Feedback, Transform.position, Transform.rotation).Play();
                SoundifierOfTheWorld.PlaySoundAtLocation(Clip, Transform, MaxDistance, Volume, MinDistance, ClipDuration, AddRandomisation, false, .2f);
                Light.intensity = MinIntensity;
            }
            else
            {
                Light.intensity = MaxIntensity;
            }

            /*
             * Dance
             */
            if (do_dance)
            {
                StartCoroutine(DanceCoroutine(new_state));
            }
            else
            {
                CurrentState = new_state;
            }
        }

        /// <summary>
        /// Monobehaviour Update method.
        /// </summary>
        private void Update()
        {
            if (ParentTransform == null)
            {
                return;
            }

            switch (CurrentState)
            {
                case FireflyState.Idle:
                    {
                        Transform.position = ParentTransform.position
                                            + (Vector3.right * Mathf.Sin(Time.time / 2 * Speed) * IdleScales.x
                                            - Vector3.up * Mathf.Sin(Time.time * Speed) * IdleScales.y
                                            - Vector3.forward * Mathf.Sin(Time.time * Speed) * IdleScales.z);
                        break;
                    }
                case FireflyState.Following:
                    {
                        if (Transform.position == ParentTransform.position + FollowOffset)
                        {
                            SetFollowOffset();
                        }

                        Transform.position = Vector3.Lerp(Transform.position, ParentTransform.position + FollowOffset, Speed * Time.deltaTime);
                        break;
                    }
                case FireflyState.Static:
                    {
                        Transform.position = Vector3.Lerp(Transform.position, ParentTransform.position, Speed * Time.deltaTime);
                        break;
                    }
                default:
                    break;
            }
        }

        
        private IEnumerator DanceCoroutine(FireflyState exit_state)
        {
            CurrentState = FireflyState.Dancing;

            yield return new WaitForSeconds(0.4f);

            Vector3 dance_pivot = ParentTransform.position;
            dance_pivot.y += FollowOffset.y;

            Vector3 target_point = ParentTransform.position + FollowOffset;
            float target_radius = (dance_pivot - target_point).magnitude;

            for (float elapsed = 0; elapsed < DanceDuration || Vector3.SqrMagnitude(Transform.position - target_point) > 0.5f; elapsed += Time.deltaTime)
            {
                float t = elapsed / DanceDuration;

                float radius = Mathf.Lerp(MinRadius, target_radius, t);

                dance_pivot = ParentTransform.position;
                dance_pivot.y += FollowOffset.y;

                Transform.RotateAround(dance_pivot, ParentTransform.up, DanceAngle * Time.deltaTime / (elapsed * elapsed + DanceDuration));

                Vector3 pos = (Transform.position - dance_pivot).normalized * radius + dance_pivot;
                pos.y = dance_pivot.y;

                Transform.position = Vector3.MoveTowards(Transform.position, pos, Time.deltaTime * Speed);

                target_point = ParentTransform.position + FollowOffset;

                yield return null;
            }

            CurrentState = exit_state;
        }

        private void SetFollowOffset()
        {
            FollowOffset.x = Random.value > 0.5f ? Random.Range(-MaxRadius, -MinRadius) : Random.Range(MinRadius, MaxRadius);
            FollowOffset.y = Random.Range(0.5f, 1.5f);
            FollowOffset.z = Random.value > 0.5f ? Random.Range(-MaxRadius, -MinRadius) : Random.Range(MinRadius, MaxRadius);
        }

        private void OnTeleportPlayerEvent(object sender, Utilities.EventManager.TeleportPlayerEventArgs args)
        {
            if (this.gameObject.activeInHierarchy)
            {
                StartCoroutine(TeleportTrailrendererCoroutine());
            }
        }

        private IEnumerator TeleportTrailrendererCoroutine()
        {
            float trailTime = TrailRenderer.time;
            TrailRenderer.time = 0;

            foreach (var particle_system in ParticleSystemList)
            {
                particle_system.Pause();
            }

            yield return null;

            TrailRenderer.time = trailTime;

            foreach (var particle_system in ParticleSystemList)
            {
                particle_system.Play();
            }
        }
    }
} // end of namespace