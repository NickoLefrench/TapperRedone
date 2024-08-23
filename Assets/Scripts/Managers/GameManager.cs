using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DentedPixel;
using System;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State { get; private set; }
    public int Score { get; private set; }

    public static event Action<GameState> OnGameStateChanged; //to notify game of the change of state
    public static event Action<int> OnScoreChanged;

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
		// when the game starts, player moves around normally
		UpdateGameState(GameState.BaseMovement );
        Score = 0;
    }

    public enum GameState
    {
        BaseMovement,
        BeerMiniGame, 
        CocktailMiniGame,
        Leaderboards,
        EndofNight,
        ScoreUI,
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

    void Update()
    {
        ProcessPause();
    }

    private void ProcessPause()
    {
        if (Input.GetButtonDown("Pause"))
        {
            Time.timeScale = Time.timeScale > 0f ? 0f : 1f;
        }
    }
}
