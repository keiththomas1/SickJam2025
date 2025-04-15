using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.AdaptivePerformance;
public enum MinigameType
{
    Spatulas,
    Meatballs,
    GreaseFryers,
    None
}

// Execute before other scripts so it has a better chance of being initialized for
// start-up music/sfx.
[DefaultExecutionOrder(-100)]
public class ContestController : MonoBehaviour
{
    private MinigameController _currentMinigame;
    private CountdownText _countdown;
    private FinishText _finishText;
    private FinishedPlayersText _finishedPlayersText;

    private List<MinigameType> _gameQueue;
    private int _round = 1;
    private int _finishedThisRound = 0;

    private Dictionary<int, int> PlayersInRound = new Dictionary<int, int>()
    {
        { 1, 20 },
        { 2, 16 },
        { 3, 12 },
        { 4, 8 },
        { 5, 5 },
        { 6, 3 },
        { 7, 1 }, // There is no 7th round, this is used to decide final players in 6th round
    };

    public static ContestController Instance
    {
        get
        {
            if (!_instance)
            {
                Debug.LogWarning("Trying to access ContestController before Awake()");
            }
            return _instance;
        }
    }
    private static ContestController _instance = null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // <-- This removes the duplicate
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += this.SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        this._currentMinigame = GameObject.FindFirstObjectByType<MinigameController>();

        if (this._currentMinigame != null)
        {
            var npcs = this.PlayersInRound[this._round] - 1;
            var contestantsNextRound = this.PlayersInRound[this._round + 1];
            this._currentMinigame.PrepareGame(npcs, contestantsNextRound);
            this._currentMinigame.OnCharacterFinished.AddListener(this.CharacterFinished);
            this._currentMinigame.OnAllFinished.AddListener(this.MinigameOver);

            this._countdown = Instantiate(Resources.Load("CountdownText") as GameObject).GetComponent<CountdownText>();
            this._countdown.SetText(this._currentMinigame.Title, this.PlayersInRound[this._round]);
            this._countdown.GetComponent<CountdownText>().OnFinished.AddListener(this.StartMinigame);
        }
    }

    public void SetupGames(MinigameType optionalStartGame = MinigameType.None)
    {
        this._round = 0;

        var minigames = new List<MinigameType>()
        {
            MinigameType.Spatulas,
            MinigameType.Meatballs,
            MinigameType.GreaseFryers
        };
        this._gameQueue = minigames.OrderBy(i => Guid.NewGuid()).ToList();

        if (this._finishedPlayersText == null)
        {
            this._finishedPlayersText = GameObject.Instantiate(Resources.Load("FinishedPlayersText") as GameObject).GetComponent<FinishedPlayersText>();
            this._finishedPlayersText.SetText(0, 0);
            this._finishedPlayersText.gameObject.SetActive(false);
        }

        this.LoadNextMinigame(optionalStartGame);
    }

    private void CharacterFinished(string name)
    {
        if (name == "Player")
        {
            AudioController.Instance.LoadNewSFXAndPlay("Win", null, 1f);

            this.MinigameOver();
        } else
        {
            AudioController.Instance.LoadNewSFXAndPlay("Win", null, 0.3f, 0.7f);
        }

        this._finishedThisRound++;
        var contestantsNextRound = this.PlayersInRound[this._round + 1];
        this._finishedPlayersText.SetText(this._finishedThisRound, contestantsNextRound);
    }

    private void MinigameOver()
    {
        this._finishText = GameObject.Instantiate(Resources.Load("FinishText") as GameObject).GetComponent<FinishText>();

        this._finishedPlayersText.gameObject.SetActive(false);

        if (this._currentMinigame.PlayerFinished)
        {
            this._finishText.GetComponent<FinishText>().SetText("FINISH!");
        } else
        {
            AudioController.Instance.LoadNewSFXAndPlay("Lose", null, 1f);
            this._finishText.GetComponent<FinishText>().SetText("FAIL!");
        }

        this.StartCoroutine(this.ContinueAfterDelay(this._currentMinigame.PlayerFinished, 2f));
    }

    private IEnumerator ContinueAfterDelay(bool playerFinished, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        MusicController.Instance.FadeOutCurrentMusic(1f);

        if (playerFinished)
        {
            this.LoadNextMinigame();
        } else
        {
            this.ResetGame();
        }
    }

    private void LoadNextMinigame(MinigameType optionalStartGame = MinigameType.None)
    {
        this._currentMinigame = null;
        this._round++;
        this._finishedThisRound = 0;

        if (this._gameQueue.Count  == 0)
        {
            // TODO: Show some sort of win sequence
            SceneManager.LoadScene("Credits", LoadSceneMode.Single);
            return;
        }

        var gameToLoad = (optionalStartGame == MinigameType.None) ? this._gameQueue[0] : optionalStartGame;
        SceneManager.LoadScene(this.GetMinigame(gameToLoad), LoadSceneMode.Single);

        this._gameQueue.RemoveAt(0);
    }

    private void StartMinigame()
    {
        if (this._currentMinigame == null)
        {
            Debug.LogError("Trying to StartGame() with no minigame");
            return;
        }

        var contestantsNextRound = this.PlayersInRound[this._round + 1];
        this._finishedPlayersText.SetText(0, contestantsNextRound);
        this._finishedPlayersText.gameObject.SetActive(true);

        this._currentMinigame.StartGame();

        this._countdown.GetComponent<CountdownText>().OnFinished.RemoveListener(this.StartMinigame);
    }

    private void ResetGame()
    {
        this._finishedPlayersText.gameObject.SetActive(false);
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    private string GetMinigame(MinigameType minigameType)
    {
        switch (minigameType)
        {
            case MinigameType.Spatulas:
                return "Spatulas";
            case MinigameType.Meatballs:
                return "Meatballs";
            case MinigameType.GreaseFryers:
                return "GreaseFryers";
            case MinigameType.None:
            default:
                Debug.LogError("Trying to load none or unknown type " + minigameType.ToString());
                return "None";
        }
    }
}
