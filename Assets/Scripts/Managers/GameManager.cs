using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DentedPixel;
using System;


public class GameManager : MonoBehaviour
{
	public enum GameState
	{
        Inactive,
		BaseMovement,
		BeerMiniGame,
		CocktailMiniGame,
		Leaderboards,
		EndofNight,
		ScoreUI,
	}

	public static GameManager Instance;

    public GameState State { get; private set; } = GameState.Inactive;
    public int Score { get; private set; }

    public static event Action<GameState> OnGameStateChanged; //to notify game of the change of state
    public static event Action<int> OnScoreChanged;

    public static PatronManager GetPatronManager()
    {
        return Instance.GetComponent<PatronManager>();
    }

    public static MenuManager GetMenuManager()
    {
        return Instance.GetComponent<MenuManager>(); 
    }

    private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
			return;
		}

		Instance = this;
	}

    private void Start()
    {
		GetMenuManager().OnUIStateChanged += OnUIStateChanged;
    }

	private void OnUIStateChanged(MenuManager.UIState newState)
	{
		if (newState == MenuManager.UIState.Game)
        {
            OnGameStart();
        }
	}

	public void OnGameStart()
	{
		UpdateGameState(GameState.BaseMovement);
		Score = 0;
	}

    public void UpdateGameState(GameState newState)
    {
		Debug.Log($"Updating state of GameManager from {State} to {newState}");
		State = newState;

        switch (newState)
        {
            case GameState.CocktailMiniGame:
                CocktailMiniGame();
                break;

            case GameState.Leaderboards:
                Leaderboards();
                break;

            case GameState.EndofNight:
                EndofNight();
                break;

            case GameState.ScoreUI: //dont think this should be included here, tbd
                break;

            default:
                break;
        }

        OnGameStateChanged?.Invoke(newState); //avoids a null being thrown
    }

    public void AddScore(int addToScore)
    {
        Score += addToScore;
        OnScoreChanged?.Invoke(Score);
    }

    public void CocktailMiniGame()
    {

    }

    public void Leaderboards()
    {

    }

    public void EndofNight()
    {

    }
}
