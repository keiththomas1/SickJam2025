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
    SmashBurger,
    Spatulas2,
    Meatballs2,
    SmashBurger2,
    None
}

// Execute before other scripts so it has a better chance of being initialized for
// start-up music/sfx.
[DefaultExecutionOrder(-100)]
public class ContestController : MonoBehaviour
{
    private MinigameController _currentMinigame;
    private MinigameType _currentMinigameType = MinigameType.None;
    private CountdownText _countdown;
    private FinishText _finishText;
    private FinishedPlayersText _finishedPlayersText;

    private List<MinigameType> _gameQueue;
    private int _round = 1;
    private int _finishedThisRound = 0;
    private MinigameType _optionalStartGame = MinigameType.None;

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
            this._countdown.SetText(this.GetMinigameName(this._currentMinigameType), this.PlayersInRound[this._round]);
            this._countdown.GetComponent<CountdownText>().OnFinished.AddListener(this.StartMinigame);
        }

        var introCutscene = GameObject.FindFirstObjectByType<IntroCutsceneController>();

        if (introCutscene != null)
        {
            introCutscene.OnFinished.AddListener(() => { this.LoadNextMinigame(this._optionalStartGame); });
        }
    }

    public void SetupGames(MinigameType optionalStartGame = MinigameType.None)
    {
        this._round = 0;

        var minigames = new List<MinigameType>()
        {
            MinigameType.Spatulas,
            MinigameType.Meatballs,
            MinigameType.SmashBurger
        };
        this._gameQueue = minigames.OrderBy(i => Guid.NewGuid()).ToList();

        // Make sure the "level 2" order is same as first order
        this._gameQueue.Add(this.GetSecondLevel(this._gameQueue[0]));
        this._gameQueue.Add(this.GetSecondLevel(this._gameQueue[1]));
        this._gameQueue.Add(this.GetSecondLevel(this._gameQueue[2]));

        if (this._finishedPlayersText == null)
        {
            this._finishedPlayersText = GameObject.Instantiate(Resources.Load("FinishedPlayersText") as GameObject).GetComponent<FinishedPlayersText>();
            this._finishedPlayersText.SetText(0, 0);
            this._finishedPlayersText.gameObject.SetActive(false);
        }

        this._optionalStartGame = optionalStartGame;
        SceneManager.LoadScene("IntroCutscene", LoadSceneMode.Single);
    }

    private void CharacterFinished(string name)
    {
        if (name == "Player")
        {
            AudioController.Instance.LoadNewSFXAndPlay("Win", null, 1f);

            this.MinigameOver();
        } else
        {
            AudioController.Instance.LoadNewSFXAndPlay("NPCWin", null, 1f);
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

        MusicController.Instance.FadeOutCurrentMusic(2f);

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

        this._currentMinigameType = (optionalStartGame == MinigameType.None) ? this._gameQueue[0] : optionalStartGame;
        SceneManager.LoadScene(this.GetMinigameSceneName(this._currentMinigameType), LoadSceneMode.Single);

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

    private string GetMinigameSceneName(MinigameType minigameType)
    {
        switch (minigameType)
        {
            case MinigameType.Spatulas:
                return "Spatulas";
            case MinigameType.Meatballs:
                return "Meatballs";
            case MinigameType.SmashBurger:
                return "SmashBurger";
            case MinigameType.Spatulas2:
                return "Spatulas2";
            case MinigameType.Meatballs2:
                return "Meatballs2";
            case MinigameType.SmashBurger2:
                return "SmashBurger2";
            case MinigameType.None:
            default:
                Debug.LogError("Trying to load none or unknown type " + minigameType.ToString());
                return "None";
        }
    }

    private string GetMinigameName(MinigameType minigameType)
    {
        switch (minigameType)
        {
            case MinigameType.Spatulas:
                return "SPINNING SPATS OF PAIN";
            case MinigameType.Meatballs:
                return "MEATBALL HILL";
            case MinigameType.SmashBurger:
                return "SMASH BURGERS";
            case MinigameType.Spatulas2:
                return "SPINNING SPATS OF DEATH";
            case MinigameType.Meatballs2:
                return "MEATBALL MOUNTAIN";
            case MinigameType.SmashBurger2:
                return "SMASH BURGERS 2";
            case MinigameType.None:
            default:
                Debug.LogError("Trying to load none or unknown type " + minigameType.ToString());
                return "None";
        }
    }

    private MinigameType GetSecondLevel(MinigameType minigameType)
    {
        switch (minigameType)
        {
            case MinigameType.Spatulas:
                return MinigameType.Spatulas2;
            case MinigameType.Meatballs:
                return MinigameType.Meatballs2;
            case MinigameType.SmashBurger:
                return MinigameType.SmashBurger2;
            default:
                return MinigameType.None;
        }
    }
}
