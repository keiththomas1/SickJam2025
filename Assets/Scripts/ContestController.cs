using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System;
public enum MinigameType
{
    Spatulas,
    Meatballs,
    GreaseFryers,
    SodaMountain,
    BurgerSmash,
    None
}

public class ContestController : MonoBehaviour
{
    public static ContestController Instance;

    private MinigameController _currentMinigame;
    private GameObject _countdown;

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
            this._currentMinigame.PrepareGame();

            this._countdown = Instantiate(Resources.Load("CountdownText") as GameObject);
            this._countdown.GetComponent<CountdownText>().OnFinished.AddListener(this.StartGame);
        }
    }

    void Update()
    {
    }

    public void SetupGames(MinigameType optionalStartGame = MinigameType.None)
    {
        var minigames = new List<MinigameType>()
        {
            MinigameType.Spatulas,
            MinigameType.Meatballs,
            MinigameType.GreaseFryers,
            MinigameType.SodaMountain,
            MinigameType.BurgerSmash
        };
        var finalGames = minigames.OrderBy(i => Guid.NewGuid()).ToList();
        foreach (var minigame in finalGames)
        {
            Debug.Log(minigame);
        }

        var gameToLoad = (optionalStartGame == MinigameType.None) ? finalGames[0] : optionalStartGame;
        SceneManager.LoadScene(this.GetMinigame(gameToLoad), LoadSceneMode.Single);

        finalGames.RemoveAt(0);
    }

    private void StartGame()
    {
        this._currentMinigame.StartGame();
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
            case MinigameType.SodaMountain:
                return "SodaMountain";
            case MinigameType.BurgerSmash:
                return "BurgerSmash";
            case MinigameType.None:
            default:
                Debug.LogError("Trying to load none or unknown type " + minigameType.ToString());
                return "None";
        }
    }
}
