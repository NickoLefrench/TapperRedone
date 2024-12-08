using System;

using FMS.TapperRedone.Data;

using UnityEngine;

namespace FMS.TapperRedone.Managers
{
    [RequireComponent(typeof(MenuManager))]
    [RequireComponent(typeof(PatronManager))]
    [RequireComponent(typeof(StatManager))]
    public class GameManager : MonoBehaviour
    {
        public enum GameState
        {
            Inactive,           // Currently not in a game night
            StartOfNight,       // Starting - load into level, UI for night
            MainGame,           // Normal game, allows player movement. Includes both having time, and time out but patrons still there
            BeerMiniGame,       // Inside beer mini game
            CocktailMiniGame,   // Inside cocktail mini game
            EndofNight,         // Night complete, score and UI. Is this needed?
        }

        public static GameManager Instance;

        public GameState State { get; private set; } = GameState.Inactive;

        private float nightEndTime;

        public static event Action<GameState> OnGameStateChanged; //to notify game of the change of state

        public static PatronManager PatronManager => Instance ? Instance.GetComponent<PatronManager>() : null;
        public static MenuManager MenuManager => Instance ? Instance.GetComponent<MenuManager>() : null;
        public static StatManager StatManager => Instance ? Instance.GetComponent<StatManager>() : null;

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
            MenuManager.OnUIStateChanged += OnUIStateChanged;
            UpdateGameState(GameState.Inactive, true);
        }

        private void Update()
        {
            if (State == GameState.MainGame && RemainingTime == 0.0f && PatronManager.CurrentPatrons == 0) //Conditions for the official EON (End of night) to occur
            {
                UpdateGameState(GameState.EndofNight);
                MenuManager.UpdateUIState(MenuManager.UIState.EndOfNightScoreboard);
            }
        }

        private void OnUIStateChanged(MenuManager.UIState oldState, MenuManager.UIState newState) //what state is the UI in and what night
        {
            switch (newState)
            {
            case MenuManager.UIState.Game:
                if (oldState == MenuManager.UIState.MainMenu)
                {
                    UpdateSavedDataOnNewRun();
                    OnGameStart();
                }
                else if (oldState == MenuManager.UIState.EndOfNightScoreboard)
                {
                    OnGameStart();
                }
                break;
            case MenuManager.UIState.Paused:
            case MenuManager.UIState.EndOfNightScoreboard:
                // Nothing
                break;
            default:
                // Anything but game and paused, set state to Inactive.
                UpdateGameState(GameState.Inactive);
                break;
            }
        }

        public float RemainingTime => Mathf.Max(0.0f, nightEndTime - Time.time);

        private void UpdateSavedDataOnNewRun()
        {
            SavedData savedData = StatManager.savedData;
            savedData.RunNight = 0;
            savedData.RunTotalScore = 0;
            savedData.RunTotalBeers = 0;
        }

        //Start conditions of every new night
        public void OnGameStart()
        {
            UpdateGameState(GameState.StartOfNight);
            nightEndTime = Time.time + TunableHandler.GetTunableFloat("NIGHT.DURATION");
        }

        public void UpdateGameState(GameState newState)
        {
            UpdateGameState(newState, false);
        }

        private void UpdateGameState(GameState newState, bool forced)
        {
            if (newState == State && !forced)
            {
                return;
            }

            Debug.Log($"Updating state of GameManager from {State} to {newState}");
            State = newState;

            switch (newState)
            {
            case GameState.CocktailMiniGame:
                StartCocktailMiniGame();//initiate cocktail minigame
                break;

            case GameState.EndofNight:
                EndofNight();
                break;

            default:
                break;
            }

            OnGameStateChanged?.Invoke(newState); //avoids a null being thrown
        }

        public void EndofNight()
        {

        }

        public void StartCocktailMiniGame()
        {
            Debug.Log("The Cocktail Minigame has begon");
            UpdateGameState(GameState.CocktailMiniGame);
        }

        public void EndCocktailMiniGame()
        {
            Debug.Log("The Cocktail Minigame has ended, returning to regular movement");
            UpdateGameState(GameState.MainGame); // Return to BaseMovement or any other appropriate state
        }
    }
}
