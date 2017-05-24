using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour {

	AudioSource _audio;
	VideoPlayer _movie;
	//int _vsyncBase;
	// Use this for initialization
	void Start () {
		_movie = GetComponent<VideoPlayer>();
		_audio = GetComponent<AudioSource>();
		//_vsyncBase = QualitySettings.vSyncCount;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (!_movie.isPlaying)
			{
				_movie.Play();
				//_audio.Play();
			}
			else
			{
				_movie.Pause();
				//_audio.Pause();
			}
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			_movie.Stop();
			//_audio.Stop();
			_movie.Play();
			//_audio.Play();
		}
		_audio.volume += Input.GetAxis("Mouse ScrollWheel")/5;
/*
		if (_movie.isPlaying)
		{
			QualitySettings.vSyncCount = 0;
		}
		else
		{
			QualitySettings.vSyncCount = _vsyncBase;
		}*/
	}
}
