using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Collections;
public enum MinigameType
{
    Spatulas,
    Meatballs,
    GreaseFryers,
    None
}

public class ContestController : MonoBehaviour
{
    public static ContestController Instance;

    private MinigameController _currentMinigame;
    private GameObject _countdown;
    private GameObject _finishText;

    private const int STARTING_NPC_COUNT = 15;

    private int _npcsRemaining;
    private List<MinigameType> _gameQueue;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // <-- This removes the duplicate
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += this.SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        this._currentMinigame = GameObject.FindFirstObjectByType<MinigameController>();

        if (this._currentMinigame != null)
        {
            this._currentMinigame.PrepareGame(this._npcsRemaining, this._npcsRemaining / 2);
            this._currentMinigame.OnAllFinished.AddListener(this.ShowFinishedScreen);

            this._countdown = Instantiate(Resources.Load("CountdownText") as GameObject);
            this._countdown.GetComponent<CountdownText>().OnFinished.AddListener(this.StartGame);

            this._npcsRemaining = this._npcsRemaining / 2;
        }
    }

    public void SetupGames(MinigameType optionalStartGame = MinigameType.None)
    {
        this._npcsRemaining = STARTING_NPC_COUNT;

        var minigames = new List<MinigameType>()
        {
            MinigameType.Spatulas,
            MinigameType.Meatballs,
            MinigameType.GreaseFryers
        };
        this._gameQueue = minigames.OrderBy(i => Guid.NewGuid()).ToList();

        this.LoadNextMinigame(optionalStartGame);
    }

    private void ShowFinishedScreen()
    {
        this._finishText = GameObject.Instantiate(Resources.Load("FinishText") as GameObject);

        if (this._currentMinigame.PlayerFinished)
        {
            this._finishText.GetComponent<FinishText>().SetText("Finish!");
        } else
        {
            this._finishText.GetComponent<FinishText>().SetText("Fail!");
        }

        this.StartCoroutine(this.ContinueAfterDelay(this._currentMinigame.PlayerFinished, 2f));
    }

    private IEnumerator ContinueAfterDelay(bool playerFinished, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (playerFinished)
        {
            this.LoadNextMinigame();
        } else
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
    }

    private void LoadNextMinigame(MinigameType optionalStartGame = MinigameType.None)
    {
        this._currentMinigame = null;

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

    private void StartGame()
    {
        if (this._currentMinigame == null)
        {
            Debug.LogError("Trying to StartGame() with no minigame");
            return;
        }

        this._currentMinigame.StartGame();

        this._countdown.GetComponent<CountdownText>().OnFinished.RemoveListener(this.StartGame);
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
