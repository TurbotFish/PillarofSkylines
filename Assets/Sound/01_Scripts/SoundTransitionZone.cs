using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

[RequireComponent(typeof(CapsuleCollider))]
public class SoundTransitionZone : MonoBehaviour {

	[HideInInspector]
	public enum TransitionType {Start, Stop, None};

	[Header ("Tracks")]
	public TransitionType transition;
	public int trackToStart;
	AudioManager audioManager;

	[Header ("Effects")]
	public float transitionTime;
	public bool neutral;
	public bool echo;


	void Awake () {
	}
	


	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag ("Player")) {
			if (audioManager == null)
				audioManager = GameObject.FindObjectOfType<AudioManager> ().GetComponent<AudioManager>();

			if (transition == TransitionType.Start) {
				audioManager.StartTrack (audioManager.tracks [trackToStart]);
			} else if (transition == TransitionType.Stop) {
				audioManager.StopTrack ();
			} 


			if (neutral) {
				audioManager.masterMixer.FindSnapshot ("Neutral").TransitionTo (transitionTime);
			} else if (echo) {
				audioManager.masterMixer.FindSnapshot ("Reverb").TransitionTo (transitionTime);
			}

		}
	}
}
