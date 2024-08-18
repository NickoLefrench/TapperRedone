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
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.BaseMovement);// when the game starts, player moves around normally


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

    public void BeerMiniGame()
    {
        // Start the Beer Mini Game
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
        if (State == GameState.BeerMiniGame) //checking each frame if the player hit the button at the correct time to receive the score bonus
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                float tickPosition = tickRectTransform.anchoredPosition.x;
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

    void OnPerfectTiming()
    {
        // Handle successful timing
        UpdateGameState(GameState.BaseMovement); //add score bonu here if mini game success +add item to inventory
    }

    void OnMissedTiming() //no score bonus, just add the item to inventory
    {
        // Handle missed timing
        UpdateGameState(GameState.BaseMovement);
    }
}
