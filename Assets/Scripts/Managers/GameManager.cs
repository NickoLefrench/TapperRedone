using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DentedPixel;
using System;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;

    // Unsure if this block of text is still necessary if used in beertap
    //assing the following in the director 
    public RectTransform tickRectTransform; // Reference to the tick (slider handle)
    public float barEndPosition; // The end position of the tick on the bar, Set this in the Inspector
    public float timeToMove = 2f; // Time for the tick to move across the bar, Set this in the Inspector

    public float perfectRangeStart; // Start of the perfect timing range
    public float perfectRangeEnd; // End of the perfect timing range
                                  //
                                  // Unsure if this block of text is still necessary if used in beertap


    //private BeerTap BeerMiniGame; //initialize the beer minigame

    // Reference to the BeerTap object
    public BeerTap beerTap;


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
		UpdateGameState(GameState.BaseMovement );

        // Ensure beerTap is assigned either via Inspector or Find
        if (beerTap == null)
        {
            beerTap = FindObjectOfType<BeerTap>(); // Try to find BeerTap in the scene
        }
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

    public void UpdateGameState(GameState newState, PlayerInteraction player = null) //what mode is the player currently in
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
        //Base Movement Logic, is regular player movement by default game boot
    }

    public  void BeerMiniGame()
    {

        if (beerTap != null)
        {
            // Passsing the polayer reference
            PlayerInteraction player = FindObjectOfType<PlayerInteraction>(); 
            if (player != null)
            {
                beerTap.StartBeerMiniGame(player);
            }
            else
            {
                Debug.LogError("PlayerInteraction not found!");
            }
        }
        else
        {
            Debug.LogError("BeerTap not assigned or found!");
        }
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

        //update Score Here!
        //activate UI bonus animation
    }

    void OnMissedTiming() //no score bonus, just add the item to inventory
	{
		Debug.Log($"Missed beer mini game timing.");
		// Handle missed timing
		UpdateGameState(GameState.BaseMovement);
    }
}
