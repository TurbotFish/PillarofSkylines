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

	public void FootSound(AnimationEvent _event){

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

	int CheckGroundBelowFoot(int _footThatTouchedGround){
		//0 is left foot
		//1 is right foot
		rayOrigin = _footThatTouchedGround == 0 ? leftFoot.transform.position : rightFoot.transform.position;

		groundFromRaycast = 0;

		if (Physics.Raycast (rayOrigin, -transform.up, out rayHit, .5f, groundMask)) {
			string _objectTag = rayHit.transform.gameObject.tag;

			/*
			if (_objectTag == "Concrete") {
				groundFromRaycast = 1;
			} else if (_objectTag == "Grass") {
				groundFromRaycast = 2;
			} 
			*/

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
}
