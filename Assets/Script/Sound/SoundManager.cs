using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager sInstance;
    public AudioMixerGroup mMainMixer;
    [SerializeField]
    private SoundAsset[] mSoundAssetArray;

    
    public Dictionary<string, SoundAsset> mSoundsDict = new Dictionary<string, SoundAsset>();

    private void Awake()
    {
        if (sInstance == null)
        {
            sInstance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(this);
        }

        InitialiseSounds();
    }

    private void InitialiseSounds()
    {
        foreach (SoundAsset sound in mSoundAssetArray)
        {
            mSoundsDict.Add(sound.name, sound);


            sound.audioSource = gameObject.AddComponent<AudioSource>();

            sound.audioSource.loop = sound.loop;
            sound.audioSource.spatialBlend = sound.spatialBlend;
            sound.audioSource.outputAudioMixerGroup = mMainMixer;

            if (sound.playOnAwake)
            {
                sound.audioSource.playOnAwake = true;
                sound.PlaySound();
            }
        }
    }

    public void PlaySound(string soundName)
    {
        if (!mSoundsDict.ContainsKey(soundName))
        {
            Debug.LogError("The sound called \""+ soundName +"\" do not exist.");
            return;
        }

        SoundAsset sound = mSoundsDict[soundName];

        sound.PlaySound();
    }
}
