using UnityEngine;

public class SoundifierOfTheWorld : MonoBehaviour {

	public static void PlaySoundAtLocation(AudioClip _clip, Transform _transform, float _maxDistance, float _volume, float _minDistance, float _duration, bool _random){
		GameObject _obj = new GameObject ();
		_obj.name = "SFX " + _clip.name;
		_obj.transform.position = _transform.position;
		AudioSource _source = _obj.AddComponent<AudioSource> ();
		//_source.transform.position = _transform.position;
		_source.clip = _clip;
		_source.spatialBlend = 1.0f;
		_source.maxDistance = _maxDistance;
		_source.minDistance = _minDistance;
		_source.volume = _volume;

		float clipDuration = _duration == 0f ? _clip.length : _duration;


		if (_random) {
			_source.pitch *= Random.Range (.8f, 1.2f);
			_source.volume *= Random.Range (.8f, 1.2f);
		}

		_source.Play ();
		Destroy (_obj, clipDuration);

	}
}
