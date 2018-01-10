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
		audioManager.PlayRandomFootstep ();
	}
}
