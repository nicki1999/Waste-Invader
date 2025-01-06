using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioMixerGroup MusicMixerGroup;
    [SerializeField] private AudioMixerGroup SFXMixerGroup;
    [SerializeField] private Sound[] sounds;

    public static float MusicVolume { get; private set; }
    public static float SFXVolume { get; private set; }

    public Slider MusicSlider;
    public Slider SFXSlider;

    private bool starting = true;

    void Awake()
    {
        foreach (Sound item in sounds)
        {
            item.source = gameObject.AddComponent<AudioSource>();
            item.source.clip = item.audioClip;
            item.source.loop = item.Looping;
            item.source.volume = item.volume;

            switch (item.audioType)
            {
                case Sound.AudioTypes.SoundEffect:
                    item.source.outputAudioMixerGroup = SFXMixerGroup;
                    break;
                case Sound.AudioTypes.Music:
                    item.source.outputAudioMixerGroup = MusicMixerGroup;
                    break;
            }

            if (item.playOnAwake)
                item.source.Play();
        }

        PlayerPrefs.SetFloat("Music Volume", 1);
        PlayerPrefs.SetFloat("SFX Volume", 1);

        MusicVolume = PlayerPrefs.GetFloat("Music Volume");
        SFXVolume = PlayerPrefs.GetFloat("SFX Volume");

        MusicSlider.value = MusicVolume;
        SFXSlider.value = SFXVolume;

        starting = false;
    }

    private void Start()
    {
        MusicMixerGroup.audioMixer.SetFloat("Music Volume", Mathf.Log10(MusicVolume) * 20);
        SFXMixerGroup.audioMixer.SetFloat("SFX Volume", Mathf.Log10(SFXVolume) * 20);
    }

    public void Play(string clipname)
    {
        Sound sound = Array.Find(sounds, dummySound => dummySound.clipName == clipname);
        if (sound != null)
            sound.source.Play();
    }

    public void Stop(string clipname)
    {
        Sound sound = Array.Find(sounds, dummySound => dummySound.clipName == clipname);
        if (sound != null)
            sound.source.Stop();
    }

    public bool IsPlaying(string clipname)
    {
        Sound sound = Array.Find(sounds, dummySound => dummySound.clipName == clipname);
        if (sound != null)
        {
            if (sound.source.isPlaying)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public void OnMusicSliderValueChange(float value)
    {
        if (!starting)
        {
            MusicVolume = value;
            MusicMixerGroup.audioMixer.SetFloat("Music Volume", Mathf.Log10(MusicVolume) * 20);
            PlayerPrefs.SetFloat("Music Volume", MusicVolume);
        }
    }

    public void OnSFXSliderValueChange(float value)
    {
        if (!starting)
        {
            SFXVolume = value;
            SFXMixerGroup.audioMixer.SetFloat("SFX Volume", Mathf.Log10(SFXVolume) * 20);
            PlayerPrefs.SetFloat("SFX Volume", SFXVolume);
            Play("Shoot1");
        }
    }

    public void ToggleMusic(bool MusicMuted)
    {
        if (MusicMuted)
        {
            MusicSlider.value = 0;
        }
        else
        {
            MusicSlider.value = 1;
        }
    }
}
