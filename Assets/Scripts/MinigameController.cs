using KinematicCharacterController;
using KinematicCharacterController.Examples;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MinigameController : MonoBehaviour
{
    [SerializeField]
    private AudioClip MinigameMusic;
    [SerializeField]
    private List<SickCharacterController> Characters;

    [SerializeField]
    private Triggerable FinishLine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.FinishLine.OnTriggered.AddListener(this.CharacterFinished);
    }

    void Update()
    {
    }

    public void PrepareGame()
    {
        foreach (var character in Characters)
        {
            character.CanMove = false;
        }
    }

    public void StartGame()
    {
        foreach (var character in Characters)
        {
            character.CanMove = true;
        }

        MusicController.Instance.FadeInMusic(this.MinigameMusic, 1f, 3f);
    }

    private void CharacterFinished(Collider collider)
    {
        Debug.Log(collider.gameObject.name + " finished!");
    }
}
