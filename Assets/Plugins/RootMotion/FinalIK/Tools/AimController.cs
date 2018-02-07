using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK {

	[RequireComponent(typeof(AimIK))]
	public class AimController : MonoBehaviour {

		[Header("Target Smoothing")]

		[Tooltip("The target to aim at. Do not use the Target transform that is assigned to AimIK. Set to null if you wish to stop aiming.")]
		public Transform target;

		[Tooltip("The time it takes to switch targets.")]
		public float targetSwitchSmoothTime = 0.3f;

		[Tooltip("The time it takes to blend in/out of AimIK weight.")]
		public float weightSmoothTime = 0.3f;

		[Header("Turning Towards The Target")]

		[Tooltip("Enables smooth turning towards the target according to the parameters under this header.")]
		public bool smoothTurnTowardsTarget = true;

		[Tooltip("Speed of turning towards the target using Vector3.RotateTowards.")]
		public float maxRadiansDelta = 3f;

		[Tooltip("Speed of moving towards the target using Vector3.RotateTowards.")]
		public float maxMagnitudeDelta = 3f;

		[Tooltip("Speed of slerping towards the target.")]
		public float slerpSpeed = 3f;

		[Tooltip("The position of the pivot that the aim target is rotated around relative to the root of the character.")] 
		public Vector3 pivotOffsetFromRoot = Vector3.up;

		[Tooltip("Minimum distance of aiming from the first bone. Keeps the solver from failing if the target is too close.")] 
		public float minDistance = 1f;

		[Header("RootRotation")]
		[Tooltip("Character root will be rotate around the Y axis to keep root forward within this angle from the aiming direction.")]
		[Range(0f, 180f)]
		public float maxRootAngle = 45f;

		[Header("Mode")]

		[Tooltip("If true, AimIK will consider whatever the current direction of the weapon to be the forward aiming direction and work additively on top of that. This enables you to use recoil and reloading animations seamlessly with AimIK. Adjust the Vector3 value below if the weapon is not aiming perfectly forward in the aiming animation clip.")]
		public bool useAnimatedAimDirection;

		[Tooltip("The direction of the animated weapon aiming in character space. Tweak this value to adjust the aiming. 'Use Animated Aim Direction' must be enabled for this property to work.")]
		public Vector3 animatedAimDirection = Vector3.forward;

		private AimIK ik;
		private Transform lastTarget;
		private float switchWeight, switchWeightV;
		private float weightV;
		private Vector3 lastPosition;
		private Vector3 dir;
		private bool lastSmoothTowardsTarget;

		void Start() {
			ik = GetComponent<AimIK>();

			if (ik.solver.target == null) {
				Debug.LogError("Please assign a Target transform for AimIK", ik.transform);
				enabled = false;
				return;			
			}

			lastPosition = ik.solver.target.position;
			dir = ik.solver.target.position - pivot;
		}

		void LateUpdate () {
			// If target has changed...
			if (target != lastTarget) {
				if (target == ik.solver.target) {
					Debug.LogWarning("Do not assign the Target transform of AimIK as AimController.target.", transform);
					target = null;
				}

				if (lastTarget == null && target != null) {
					lastPosition = target.position;
					dir = target.position - pivot;
					ik.solver.target.position = target.position;
				} else {
					lastPosition = ik.solver.target.position;
					dir = ik.solver.target.position - pivot;
				}

				switchWeight = 0f;
				lastTarget = target;
			}

			// Smooth weight
			ik.solver.IKPositionWeight = Mathf.SmoothDamp(ik.solver.IKPositionWeight, (target != null? 1f: 0f), ref weightV, weightSmoothTime);
			if (ik.solver.IKPositionWeight >= 0.999f) ik.solver.IKPositionWeight = 1f;
			if (ik.solver.IKPositionWeight <= 0.001f) ik.solver.IKPositionWeight = 0f;

			if (ik.solver.IKPositionWeight <= 0f) return;

			// Smooth target switching
			switchWeight = Mathf.SmoothDamp(switchWeight, 1f, ref switchWeightV, targetSwitchSmoothTime);
			if (switchWeight >= 0.999f) switchWeight = 1f;

			if (target != null) {
				ik.solver.target.position = Vector3.Lerp(lastPosition, target.position, switchWeight);
			}

			// Smooth turn towards target
			if (smoothTurnTowardsTarget != lastSmoothTowardsTarget) {
				dir = ik.solver.target.position - pivot;
				lastSmoothTowardsTarget = smoothTurnTowardsTarget;
			}

			if (smoothTurnTowardsTarget) {
				Vector3 targetDir = ik.solver.target.position - pivot;
				dir = Vector3.Slerp(dir, targetDir, Time.deltaTime * slerpSpeed);
				dir = Vector3.RotateTowards(dir, targetDir, Time.deltaTime * maxRadiansDelta, maxMagnitudeDelta);
				ik.solver.target.position = pivot + dir;
			}

			// Min distance from the pivot
			ApplyMinDistance();

			// Root rotation
			RootRotation();

			// Offset mode
			if (useAnimatedAimDirection) {
				ik.solver.axis = ik.solver.transform.InverseTransformVector(ik.transform.rotation * animatedAimDirection);
			}
		}

		// Pivot of rotating the aiming direction.
		private Vector3 pivot {
			get {
				return ik.transform.position + ik.transform.rotation * pivotOffsetFromRoot;
			}
		}

		// Make sure aiming target is not too close (might make the solver instable when the target is closer to the first bone than the last bone is).
		void ApplyMinDistance() {
			Vector3 aimFrom = pivot;
			Vector3 direction = (ik.solver.target.position - aimFrom);
			direction = direction.normalized * Mathf.Max(direction.magnitude, minDistance);
				
			ik.solver.target.position = aimFrom + direction;
		}

		// Character root will be rotate around the Y axis to keep root forward within this angle from the aiming direction.
		private void RootRotation() {
			float max = Mathf.Lerp(180f, maxRootAngle, ik.solver.IKPositionWeight);

			if (max < 180f) {
				Vector3 faceDirLocal = Quaternion.Inverse(ik.transform.rotation) * (ik.solver.target.position - pivot);
				float angle = Mathf.Atan2(faceDirLocal.x, faceDirLocal.z) * Mathf.Rad2Deg;

				float rotation = 0f;

				if (angle > max) {
					rotation = angle - max;
				}
				if (angle < -max) {
					rotation = angle + max;
				}

				ik.transform.rotation = Quaternion.AngleAxis(rotation, ik.transform.up) * ik.transform.rotation;		
			}
		}
	}
}
