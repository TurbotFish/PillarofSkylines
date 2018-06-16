using UnityEngine;

public class AudioSourceStartOffset : MonoBehaviour {

	AudioSource source;

	void Start(){
		source = GetComponent<AudioSource> ();
		if (!source)
			return;

		source.time = Random.Range (0f, source.clip.length);
	}
}
