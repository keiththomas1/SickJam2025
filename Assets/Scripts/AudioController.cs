using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UIElements;

public enum AudioType
{
    Music,
    Ambient,
    SFX,
    None
}

// Execute before other scripts so it has a better chance of being initialized for
// start-up music/sfx.
[DefaultExecutionOrder(-100)]
public class AudioController : MonoBehaviour
{
    [SerializeField]
    private AudioSource DefaultAudioSource;
    [SerializeField]
    private Transform PlayingMusic;
    [SerializeField]
    private Transform PlayingAmbient;
    [SerializeField]
    private Transform PlayingSFX;
    [SerializeField]
    private List<AudioMixerGroup> AudioMixers;

    private List<AudioSource> _musicSources = new List<AudioSource>();
    private List<AudioSource> _ambientSources = new List<AudioSource>();
    private GenericPool _pooledSFXSources;

    private const int SFX_POOL_SIZE = 100;

    private const float DEFAULT_VOLUME_LEVEL = 0.75f;
    private const float DEFAULT_FALLOFF_DISTANCE = 25f;
    private const string AMBIENT_PATH = "Audio/";
    private const string SFX_PATH = "Audio/";

    private AudioSource _currentWeatherAmbience = null;

    public UnityEvent OnMusicVolumeChanged = new UnityEvent();

    public float MasterMusicVolume { get; private set; } = .75f;
    public float MasterAmbientVolume { get; private set; } = .75f;
    public float MasterSFXVolume { get; private set; } = .75f;

    public static AudioController Instance
    {
        get
        {
            if (!_instance)
            {
                Debug.LogWarning("Trying to access AudioController before Awake()");
            }
            return _instance;
        }
    }
    private static AudioController _instance = null;

    private void Awake()
    {
        if (_instance)
        {
            Debug.LogError("Multiple instances of AudioController defined, there should only be one.");
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        this._pooledSFXSources = new GenericPool(this.DefaultAudioSource.gameObject, this.PlayingSFX, SFX_POOL_SIZE);
    }

    // Expects a 0-1 value
    public void SetMusicVolume(float newVolume)
    {
        this.OnMusicVolumeChanged.Invoke();

        this.MasterMusicVolume = newVolume * newVolume;
        this.ChangeAudioSourceVolumes(AudioType.Music);
    }
    // Expects a 0-1 value
    public void SetAmbientVolume(float newVolume)
    {
        this.MasterAmbientVolume = newVolume * newVolume;
        this.ChangeAudioSourceVolumes(AudioType.Ambient);
    }
    // Expects a 0-1 value
    public void SetSFXVolume(float newVolume)
    {
        this.MasterSFXVolume = newVolume * newVolume;
        this.ChangeAudioSourceVolumes(AudioType.SFX);
    }

    public float GetMusicVolume(float volume)
    {
        return volume * this.MasterMusicVolume;
    }
    public float GetAmbientVolume(float volume)
    {
        return volume * this.MasterAmbientVolume;
    }
    public float GetSFXVolume(float volume)
    {
        return volume * this.MasterSFXVolume;
    }

    // Music

    public AudioSource CreateNewMusicAndPlay(
        AudioClip audioClip, float volumeLevel = DEFAULT_VOLUME_LEVEL, bool looping = true)
    {
        if (audioClip == null)
        {
            return null;
        }

        return this.SetupAudioSource(audioClip, this.GetMusicVolume(volumeLevel), looping, AudioType.Music, null);
    }

    // Ambient

    public AudioSource LoadNewWeatherAmbientAndPlay(
        string clipName, float volumeLevel = DEFAULT_VOLUME_LEVEL)
    {
        if (this._currentWeatherAmbience != null)
        {
            Destroy(this._currentWeatherAmbience.gameObject);
        }

        this._currentWeatherAmbience = this.LoadNewAmbientAndPlay(clipName, null, volumeLevel);
        return this._currentWeatherAmbience;
    }

    public AudioSource LoadNewAmbientAndPlay(
        string clipName, Vector3? position, float volumeLevel = DEFAULT_VOLUME_LEVEL)
    {
        if (clipName == string.Empty)
        {
            return null;
        }

        var audioClip = Resources.Load<AudioClip>(Path.Join(AMBIENT_PATH, clipName));
        return this.CreateNewAmbientAndPlay(audioClip, position, volumeLevel);
    }

    public AudioSource CreateNewAmbientAndPlay(
        AudioClip audioClip, Vector3? position, float volumeLevel = DEFAULT_VOLUME_LEVEL, float falloffDistance = DEFAULT_FALLOFF_DISTANCE)
    {
        if (audioClip == null)
        {
            return null;
        }

        var audioSource = this.SetupAudioSource(audioClip, this.GetAmbientVolume(volumeLevel), true, AudioType.Ambient, position);
        audioSource.maxDistance = falloffDistance;
        return audioSource;
    }

    // SFX

    public AudioSource LoadNewSFXAndPlay(
        string clipName, Vector3? position, float volumeLevel = DEFAULT_VOLUME_LEVEL, float pitch = 1.0f, bool looping = false)
    {
        if (clipName == string.Empty)
        {
            return null;
        }

        var audioClip = Resources.Load<AudioClip>(Path.Join(SFX_PATH, clipName));
        return this.CreateNewSFXAndPlay(audioClip, position, volumeLevel, pitch, looping);
    }

    public AudioSource CreateNewSFXAndPlay(
        AudioClip audioClip, Vector3? position, float volumeLevel = DEFAULT_VOLUME_LEVEL, float pitch = 1.0f, bool looping = false)
    {
        if (audioClip == null)
        {
            return null;
        }

        var audioSource = this.SetupAudioSource(audioClip, this.GetSFXVolume(volumeLevel), looping, AudioType.SFX, position);
        audioSource.pitch = UnityEngine.Random.Range(pitch - 0.05f, pitch + 0.05f); // Adds a bit of variance on each sound for sounds that are played often.

        // TODO: Set up a system where this event will change the volume of the audio source but when the audio source is destroyed (maybe a custom script that lives on all
        //      audio sources, then this listener is removed) and same with music/ambient
        // this.OnSFXVolumeChanged.AddListener(() => { })
        return audioSource;
    }

    private AudioSource SetupAudioSource(AudioClip audioClip, float volumeLevel, bool looping, AudioType audioType, Vector3? position)
    {
        AudioSource audioSource = null;
        switch (audioType)
        {
            case AudioType.Music:
                audioSource = Instantiate(this.DefaultAudioSource);
                audioSource.transform.SetParent(this.PlayingMusic);
                this._musicSources.Add(audioSource);
                break;
            case AudioType.Ambient:
                audioSource = Instantiate(this.DefaultAudioSource);
                audioSource.transform.SetParent(this.PlayingAmbient);
                this._ambientSources.Add(audioSource);
                break;
            case AudioType.SFX:
                audioSource = this._pooledSFXSources.GetNewPoolObject().GetComponent<AudioSource>();
                break;
        }

        audioSource.gameObject.name = audioClip.name;

        audioSource.Stop();
        audioSource.clip = audioClip;
        audioSource.volume = volumeLevel;
        audioSource.loop = looping;
        audioSource.Play();

        if (position != null && position.Value != Vector3.zero)
        {
            audioSource.transform.position = position.Value;
            audioSource.spatialBlend = 1f;
        }
        else
        {
            audioSource.spatialBlend = 0f;
        }

        var audioInfo = audioSource.GetComponent<AudioInfo>();
        audioInfo.DefaultVolume = volumeLevel;
        audioInfo.AudioType = audioType;

        // if (!looping)
        // {
        //     var deathByTimer = audioSourceObject.AddComponent<DeathByTimer>();
        //     deathByTimer.deathTimeInSeconds = audioClip.length + 0.5f; // Add a bit of buffer
        // }

        return audioSource;
    }

    private void ChangeAudioSourceVolumes(AudioType audioType)
    {
        switch (audioType)
        {
            case AudioType.Music:
                foreach (var audioSource in this._musicSources)
                {
                    var audioInfo = audioSource.GetComponent<AudioInfo>();
                    if (audioSource.gameObject.activeSelf && audioSource.enabled)
                    {
                        var defaultVolume = audioInfo.DefaultVolume;
                        audioSource.volume = defaultVolume * this.MasterMusicVolume;
                    }
                }
                break;
            case AudioType.Ambient:
                foreach (var audioSource in this._ambientSources)
                {
                    var audioInfo = audioSource.GetComponent<AudioInfo>();
                    if (audioSource.gameObject.activeSelf && audioSource.enabled)
                    {
                        var defaultVolume = audioInfo.DefaultVolume;
                        audioSource.volume = defaultVolume * this.MasterAmbientVolume;
                    }
                }
                break;
            case AudioType.SFX:
                foreach (var audioSource in this._pooledSFXSources.PooledObjects)
                {
                    var audioInfo = audioSource.GetComponent<AudioInfo>();
                    if (audioSource.activeSelf && audioSource.GetComponent<AudioSource>().enabled)
                    {
                        var defaultVolume = audioInfo.DefaultVolume;
                        audioSource.GetComponent<AudioSource>().volume = defaultVolume * this.MasterSFXVolume;
                    }
                }
                break;
        }
    }
}
