using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioInfo : MonoBehaviour
{
    public AudioType AudioType
    {
        get; set;
    } = AudioType.None;
    public float DefaultVolume
    {
        get; set;
    } = 1f;

    private void Awake()
    {
        // DontDestroyOnLoad(this);
    }
}
