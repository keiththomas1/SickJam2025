using DG.Tweening;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class MinigameController : MonoBehaviour
{
    [SerializeField]
    private string MinigameTitle;
    [SerializeField]
    private AudioClip MinigameMusic;
    [SerializeField]
    private SickCharacterController Player;
    [SerializeField]
    private List<SickCharacterController> NPCs;

    [SerializeField]
    private Triggerable FinishLine;
    [SerializeField]
    private List<Transform> Destinations;
    [SerializeField]
    private GameObject RespawnPosition;

    private bool _playerFinished = false;
    private int _finishCount;

    public class FinishEvent : UnityEvent<string> { }
    public FinishEvent OnCharacterFinished = new FinishEvent();
    public UnityEvent OnAllFinished = new UnityEvent();

    public string Title
    {
        get { return this.MinigameTitle; }
    }
    public bool PlayerFinished
    {
        get { return this._playerFinished; }
    }

    void Start()
    {
        this.FinishLine.OnTriggered.AddListener(this.CharacterFinished);


        foreach (var npc in this.NPCs)
        {
            var rand = Random.Range(0, this.Destinations.Count);
            npc.GetComponent<NPCNavigation>().Destination = this.Destinations[rand];
        }

        foreach (var destination in this.Destinations)
        {
            destination.gameObject.SetActive(false);
        }
        this.RespawnPosition.SetActive(false);
    }

    void Update()
    {
        this.RespawnIfOutOfBounds(this.Player);
        foreach (var npc in this.NPCs)
        {
            this.RespawnIfOutOfBounds(npc);
        }
    }

    private void RespawnIfOutOfBounds(SickCharacterController sickCharacterController)
    {
        if (sickCharacterController.transform.position.y < -7f)
        {
            sickCharacterController.GetComponent<KinematicCharacterMotor>().SetPosition(this.RespawnPosition.transform.position);
        }   
    }

    public void PrepareGame(int contestants, int finishCount)
    {
        this.Player.CanMove = false;
        foreach (var character in NPCs)
        {
            character.gameObject.SetActive(false);
            character.CanMove = false;
        }

        for (int i = 0; i < contestants; i++)
        {
            this.NPCs[i].gameObject.SetActive(true);
        }

        this._playerFinished = false;
        this._finishCount = finishCount;
    }

    public void StartGame()
    {
        this.Player.CanMove = true;
        foreach (var character in NPCs)
        {
            character.CanMove = true;
        }

        if (MusicController.Instance != null)
        {
            MusicController.Instance.PlayMusic(this.MinigameMusic, 0.7f);
        }
    }

    private void CharacterFinished(Collider collider)
    {
        if (collider.tag != "Player")
        {
            return;
        }

        if (collider.GetComponent<SickCharacterController>() == this.Player)
        {
            if (this._playerFinished) // Player left zone and returned which shouldn't trigger a finish
            {
                return;
            }

            this._playerFinished = true;
        }

        this._finishCount--;
        this.OnCharacterFinished.Invoke(collider.gameObject.name);

        this.FinishLine.GetComponent<FinishLine>().Flicker();

        if (this._finishCount == 0)
        {
            this.OnAllFinished.Invoke();

            this.Player.CanMove = false;
            foreach (var character in NPCs)
            {
                character.gameObject.SetActive(false);
                character.CanMove = false;
            }
        }
    }
}
