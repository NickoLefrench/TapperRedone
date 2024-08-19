using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DentedPixel;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;

    public RectTransform tickRectTransform; // Reference to the tick (slider handle)
    public float barEndPosition; // The end position of the tick on the bar
    public float timeToMove = 2f; // Time for the tick to move across the bar

    public float perfectRangeStart; // Start of the perfect timing range
    public float perfectRangeEnd; // End of the perfect timing range

    public static event Action<GameState> OnGameStateChanged; //to notify game of the change of state

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
		UpdateGameState(GameState.BaseMovement);
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

    public void UpdateGameState(GameState newState) //what mode is the player currently in
    {
		Debug.Log($"Updating state of GameManager from {State} to {newState}");
		State = newState;

        switch (newState)
        {
            case GameState.BaseMovement:
                BaseMovement();
                break;

            case GameState.BeerMiniGame:
                BeerMiniGame();
                break;

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
        }

        OnGameStateChanged?.Invoke(newState); //avoids a null being thrown
    }

    public void BaseMovement()
    {

    }

    public  void BeerMiniGame()
    {
        // Start the Beer Mini Game
        tickRectTransform.anchoredPosition.Set(0, 0);
        LeanTween.moveX(tickRectTransform, barEndPosition, timeToMove).setOnComplete(() => {
            // Update the game state to return to BaseMovement when the animation is complete
            UpdateGameState(GameState.BaseMovement);
        });
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

        if (State == GameState.BeerMiniGame) //checking each frame if the player hit the button at the correct time to receive the score bonus
        {
            if (Input.GetButtonDown("BeerPour"))
            {
                float tickPosition = tickRectTransform.anchoredPosition.x;
                Debug.Log("Tick position: " + tickPosition);
                if (tickPosition >= perfectRangeStart && tickPosition <= perfectRangeEnd)
                {
                    OnPerfectTiming();
                }
                else
                {
                    OnMissedTiming();
                }
            }
        }
    }

    private void ProcessPause()
    {
        if (Input.GetButtonDown("Pause"))
        {
            Time.timeScale = Time.timeScale > 0f ? 0f : 1f;
        }
    }

    void OnPerfectTiming()
	{
		Debug.Log($"Perfect beer mini game timing!");
		// Handle successful timing
		UpdateGameState(GameState.BaseMovement); //add score bonu here if mini game success +add item to inventory
    }

    void OnMissedTiming() //no score bonus, just add the item to inventory
	{
		Debug.Log($"Missed beer mini game timing.");
		// Handle missed timing
		UpdateGameState(GameState.BaseMovement);
    }
}
