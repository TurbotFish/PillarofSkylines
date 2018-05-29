using UnityEngine;

public class PlayerAudio : MonoBehaviour {

	public AudioManager audioManager;
	Animator anim;


	//Footsteps
	//float footsteps
	void Awake()
	{
		anim = GetComponent<Animator> ();
	}
	public void Footstep()
	{
		//commented this part on 28/05/18 after changing PlayRandomFootstep() - Aloïs
		//audioManager.PlayRandomFootstep ();
	}
}
