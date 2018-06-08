using UnityEngine;
using UnityEngine.Audio;

public class SoundifierOfTheWorld : MonoBehaviour {

	public static GameObject PlaySoundAtLocation(AudioClip _clip, Transform _transform, float _maxDistance, float _volume, float _minDistance, float _duration, bool _random, bool _onLoop = false, float _dopplerLevel = 1f){
		GameObject _obj = new GameObject ();
		_obj.transform.SetParent (_transform);
		_obj.name = "SFX " + _clip.name;
		_obj.transform.position = _transform.position;
		AudioSource _source = _obj.AddComponent<AudioSource> ();

		_source.clip = _clip;
		_source.spatialBlend = 1.0f;
		_source.maxDistance = _maxDistance;
		_source.minDistance = _minDistance;
		_source.volume = _volume;
		_source.outputAudioMixerGroup = AudioManager.atmoMixerGroup;
        _source.loop = _onLoop;
        _source.dopplerLevel = _dopplerLevel;

		float clipDuration = _duration == 0f ? _clip.length : _duration;

		if (_random) {
			_source.pitch *= Random.Range (.8f, 1.2f);
			_source.volume *= Random.Range (.8f, 1.2f);
		}


		_source.Play ();

        if (!_onLoop)
    		Destroy (_obj, clipDuration);

        return _obj;
	}
}
