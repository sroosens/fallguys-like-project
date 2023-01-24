using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewSoundAsset", menuName = "ScriptableObject/SoundAsset")]
public class SoundAsset : ScriptableObject
{
    [System.Serializable]
    public class Sound
    {
        public AudioClip audioClip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(.1f, 3f)]
        public float pitch = 1f;
    }

    new public string name;

    public Sound[] sounds;

    [Range(0f, 1f)]
    public float spatialBlend;
    public bool loop = false;
    public bool playOnAwake = false;

    [HideInInspector]
    public AudioSource audioSource;

    public void PlaySound()
    {
        Sound soundToPlay;

        if(sounds.Length <= 0)
        {
            Debug.Log("SoundAsset " + name + "do not contain sounds");
            return;
        }
        else if (sounds.Length == 1)
        {
            soundToPlay = sounds[0];
        }
        else
        {
            soundToPlay = sounds[Random.Range(0, sounds.Length)];
        }


        audioSource.clip = soundToPlay.audioClip;
        audioSource.volume = soundToPlay.volume;
        audioSource.pitch = soundToPlay.pitch;

        audioSource.Play();
    }
}
