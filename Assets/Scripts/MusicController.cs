using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class MusicController : MonoBehaviour
{
    private AudioSource _currentPlayingAudio;

    private const float TRANSITION_TIME = 3f;

    public static MusicController Instance
    {
        get
        {
            if (!_instance)
            {
                Debug.LogWarning("Trying to access MusicController before Awake()");
            }
            return _instance;
        }
    }
    private static MusicController _instance = null;

    private void Awake()
    {
        if (_instance)
        {
            Debug.LogError("Multiple instances of MusicController defined, there should only be one.");
        }
        _instance = this;

        // AudioController.Instance.OnMusicVolumeChanged.AddListener(this.MusicLevelChanged);
    }

    // Find the track, add it to currently playing, and then fade it in
    public void FadeInMusic(AudioClip audioClip, float volume, float fadeTime = TRANSITION_TIME)
    {
        AudioSource audioSource = AudioController.Instance.CreateNewMusicAndPlay(audioClip, 0f, true);
        audioSource.Play();

        audioSource.GetComponent<AudioInfo>().DefaultVolume = volume;

        var actualVolume = AudioController.Instance.GetMusicVolume(volume);

        // Need to set it again in onComplete in case MasterVolume changed during fade.
        // this._currentSequence.Insert(0, audioSource.DOFade(actualVolume, fadeTime).OnComplete(() => audioSource.volume = AudioController.Instance.GetMusicVolume(volume)));
        audioSource.DOFade(actualVolume, fadeTime).OnComplete(() => audioSource.volume = AudioController.Instance.GetMusicVolume(volume));

        this._currentPlayingAudio = audioSource;
    }

    // Find the track, remove it from currently playing, and then fade it out
    public void FadeOutCurrentMusic(float fadeTime = TRANSITION_TIME)
    {
        AudioSource audioSource = this._currentPlayingAudio;
        audioSource.GetComponent<AudioInfo>().DefaultVolume = 0f;
        audioSource.DOFade(0f, fadeTime);

        this._currentPlayingAudio = null;
    }
}
