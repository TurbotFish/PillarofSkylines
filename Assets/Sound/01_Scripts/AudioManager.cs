using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : MonoBehaviour {

	public enum SurfaceType
	{
		Concrete,
		Grass
	};

	[Header("Player")]
	public Animator anim;
	public Game.Player.CharacterController.CharController player;

	[Header("Footsteps")]
	public SurfaceType surface;
	public AudioSource[] FSConcrete;
	public AudioSource[] FSGrass;
	public float pitchVariance;
	public float volumeVariance;
	int previousFS;
	float currentFootsteps;
	float previousFootsteps;

	[Header("Wind")]
	public AudioSource wind;
	float playerSpeed;
	public MinMax windVolume;
	public float playerVelocityMax;

	[Header ("Tracks")]
	public float transitionTime;
	public AudioSource[] tracks;
	AudioSource currenTrack;

	[Header ("Other")]
	public AudioSource[] startingAudios;

	[Header ("AudioMixers")]
	public AudioMixer masterMixer;



	void Start () {
		foreach (AudioSource source in startingAudios) {
			source.Play ();
		}
	}
	

	void Update()
	{
		HandleFootsteps ();
		HandleWind ();
	}


	void HandleWind()
	{
		playerSpeed = player.MovementInfo.velocity.magnitude;
		float t = playerSpeed / playerVelocityMax;
		wind.volume = Mathf.Lerp (windVolume.min, windVolume.max, t);

	}

	void HandleFootsteps()
	{
		currentFootsteps = anim.GetFloat ("Footsteps");	
		if (currentFootsteps > 0 && previousFootsteps < 0) {
			PlayRandomFootstep ();
		}
		previousFootsteps = currentFootsteps;
	}

	public void PlayRandomFootstep()
	{
		AudioSource footstep = FSGrass[0];
		//default footstep
		if (surface == SurfaceType.Concrete) {
			int index = Random.Range (0, FSConcrete.Length);
			for (int i = 0; i < 10; i++) {
				if (index == previousFS)
					index = Random.Range (0, FSConcrete.Length);
				else
					break;
			}
			footstep = FSConcrete [index];
		}
		else if (surface == SurfaceType.Grass) {
			int index = Random.Range (0, FSGrass.Length);
			for (int i = 0; i < 10; i++) {
				if (index == previousFS)
					index = Random.Range (0, FSGrass.Length);
				else
					break;
			}
			footstep = FSGrass [index];
		}

		footstep.volume = 1 + Random.Range (-volumeVariance, 0);
		footstep.pitch = 1 + Random.Range (-pitchVariance, pitchVariance);
		footstep.Play ();
	}


	public void StartTrack(AudioSource track)
	{
		currenTrack.DOFade (0, transitionTime).SetEase (Ease.InOutSine).OnComplete (OnCompleteStop);
		track.Play ();
		track.DOFade (1, transitionTime+0.1f).SetEase (Ease.InOutSine).OnComplete (()=>OnCompleteSwitch(track));
	}

	public void StopTrack()
	{
		currenTrack.DOFade (0, transitionTime).SetEase (Ease.InOutSine).OnComplete (OnCompleteStop);
	}

	void OnCompleteStop()
	{
		currenTrack.Stop ();
	}
	void OnCompleteSwitch(AudioSource track)
	{
		currenTrack = track;
	}
}
