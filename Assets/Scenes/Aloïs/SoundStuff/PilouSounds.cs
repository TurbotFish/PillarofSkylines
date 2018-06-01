using UnityEngine;

public class PilouSounds : MonoBehaviour {

	AudioManager audioManager;
	GameObject leftFoot;
	GameObject rightFoot;

	RaycastHit rayHit;
	Vector3 rayOrigin;
	public LayerMask groundMask;
	int groundType;
	int groundFromRaycast;


	float currentFootLeft;
	float lastFootLeft;
	float currentFootRight;
	float lastFootRight;

	Animator anim;

	void Update(){

		if (!anim)
			anim = this.GetComponent<Animator> ();

		if (!audioManager)
			audioManager = FindObjectOfType<AudioManager> ();

		if (!leftFoot) {
			leftFoot = transform.Find ("bone_root_Ctrl").Find ("bone_hub1_leg1_foot_FootIKctrl").gameObject;
			if (!leftFoot)
				Debug.LogError ("Couldn't find left foot object, it's likely the hierarchy has been changed.");
		}

		if (!rightFoot) {
			rightFoot = transform.Find ("bone_root_Ctrl").Find ("bone_hub1_leg2_foot_FootIKctrl").gameObject;
			if (!leftFoot)
				Debug.LogError ("Couldn't find right foot object, it's likely the hierarchy has been changed.");
		}

		currentFootLeft = anim.GetFloat ("LeftFoot");
		if (currentFootLeft < 0f && lastFootLeft >= 0f) {
			if(CheckGround(leftFoot.transform.position + transform.up * .3f))
				audioManager.PlayRandomFootstep (groundFromRaycast);
				
		}
		lastFootLeft = currentFootLeft;

		currentFootRight = anim.GetFloat ("RightFoot");
		if (currentFootRight >= 0f && lastFootRight < 0f) {
			if(CheckGround(rightFoot.transform.position + transform.up * .3f))
				audioManager.PlayRandomFootstep (groundFromRaycast);
		}
		lastFootRight = currentFootRight;
		
	}

	bool CheckGround(Vector3 _rayOrigin){
		groundFromRaycast = 0;

		Debug.DrawRay (_rayOrigin, -transform.up * .8f, Color.cyan);

		bool _hitSomething = Physics.Raycast (_rayOrigin, -transform.up, out rayHit, .8f, groundMask);

		if (_hitSomething) {
			
			string _objectTag = rayHit.transform.gameObject.tag;

			if (_objectTag == "Grass") {
				groundFromRaycast = 2;
			} else {
				groundFromRaycast = 1;
			}
		}

		return _hitSomething;
	}

	/*
	public void FootSound(AnimationEvent _event){

		

		soundDebug04 = transform.position;
		//print (_event.animatorClipInfo.weight + "    " + Time.frameCount);

		if (_event.animatorClipInfo.weight > .5f) {
			//0 is left foot
			//1 is right foot
			groundType = CheckGroundBelowFoot (_event.intParameter);

			if (groundType == 0)
				return;

			//1 is concrete
			//2 is grass
			audioManager.PlayRandomFootstep (groundType);
		}


	}

	Vector3 soundDebug01;
	Vector3 soundDebug02;
	Vector3 soundDebug03;
	Vector3 soundDebug04;

	int CheckGroundBelowFoot(int _footThatTouchedGround){
		//0 is left foot
		//1 is right foot
		rayOrigin = _footThatTouchedGround == 0 ? leftFoot.transform.position : rightFoot.transform.position;
		rayOrigin += transform.up * .3f;

		groundFromRaycast = 0;

		soundDebug02 = rayOrigin;
		soundDebug03 = rayOrigin;

		Debug.DrawRay (rayOrigin, -transform.up * .8f, Color.cyan);
		if (Physics.Raycast (rayOrigin, -transform.up, out rayHit, .8f, groundMask)) {
			string _objectTag = rayHit.transform.gameObject.tag;

			
//			if (_objectTag == "Concrete") {
//				groundFromRaycast = 1;
//			} else if (_objectTag == "Grass") {
//				groundFromRaycast = 2;
//			} 

			soundDebug01 = rayHit.point;

			if (_objectTag == "Grass") {
				groundFromRaycast = 2;
			} else {
				groundFromRaycast = 1;
			}

		}
		return groundFromRaycast;
	}

	public void LandSound(){
		print ("LANDLAND");
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere (soundDebug01, .1f);

		Gizmos.DrawLine (soundDebug02, soundDebug01);

		Gizmos.color = Color.red;
		Gizmos.DrawCube (soundDebug03, Vector3.one * .1f);

		Gizmos.color = Color.yellow;
		Gizmos.DrawCube (soundDebug04, Vector3.one * .2f);
	}
*/
}
