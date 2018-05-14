using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	public enum SurfaceType
	{
		Concrete,
		Grass
	};

    public AudioLibrary lib;

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
    public AudioSource[] sfx;

    Dictionary<string, AudioSource> SfxIndex = new Dictionary<string, AudioSource>();

	[Header ("AudioMixers")]
	public AudioMixer masterMixer;



	void Start () {
		foreach (AudioSource source in startingAudios) {
			source.Play ();
		}

        lib.Initialize();

        foreach (AudioSource source in sfx)
            SfxIndex.Add(source.name, source);
	}
	

	void Update()
	{
		//HandleFootsteps ();
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

    public void PlaySfx(string name) {
        if (SfxIndex.ContainsKey(name))
            SfxIndex[name].Play();
        else
            Debug.LogError("Le SFX demandé n'existe pas. Le nom est sans doute mal tappé, ou il n'a pas été ajouté à l'AudioManager.");
    }

    public void StopSfx(string name) {
        if (SfxIndex.ContainsKey(name))
            SfxIndex[name].Stop();
        else
            Debug.LogError("Le SFX demandé n'existe pas. Le nom est sans doute mal tappé, ou il n'a pas été ajouté à l'AudioManager.");
    }
    
    public void StopAllSfx() {
        foreach (AudioSource source in sfx)
            source.Stop();
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
