using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObjects/AudioLibrary", fileName = "AudioLibrary")]
public class AudioLibrary : ScriptableObject
{
    [System.Serializable]
    public class AudioAsset
    {
        public string name;
        public AudioClip clip;

        public AudioAsset(string name, AudioClip clip) {
            this.name = name;
            this.clip = clip;
        }

        public static implicit operator AudioAsset(AudioClip audioClip) {
            return new AudioAsset(audioClip.name, audioClip);
        }

        public static implicit operator AudioClip(AudioAsset asset)
        {
            return asset.clip;
        }

    }

    public List<AudioClip> FootSteps_Concrete, FootSteps_Grass;
    
    [SerializeField] AudioAsset[] sfx = new AudioAsset[0];
    public static Dictionary<string, AudioClip> Sfx = new Dictionary<string, AudioClip>();
    
    public void Initialize() {
        Sfx = new Dictionary<string, AudioClip>();

        foreach (AudioAsset asset in sfx)
            Sfx.Add(asset.name, asset.clip);
    }

    void OnValidate() {
        for (int i = 0; i < sfx.Length; i++) {
            AudioAsset asset = sfx[i];

            if (asset.clip && asset.name == "")
                asset.name = asset.clip.name;
        }
        
        Initialize();
        
    }


}
