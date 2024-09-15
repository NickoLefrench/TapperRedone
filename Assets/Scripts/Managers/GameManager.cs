using System;
using UnityEngine;

namespace FMS.TapperRedone.Managers
{
	[RequireComponent(typeof(MenuManager))]
	[RequireComponent(typeof(PatronManager))]
    public class GameManager : MonoBehaviour
    {
        public enum GameState
        {
            Inactive,           // Currently not in a game night
            StartOfNight,       // Starting - load into level, UI for night
            MainGame,           // Normal game, allows player movement. Includes both having time, and time out but patrons still there
            BeerMiniGame,       // Inside beer mini game
            CocktailMiniGame,   // Inside cocktail mini game
            EndofNight,         // Night complete, score and UI
        }

        public static GameManager Instance;

        public GameState State { get; private set; } = GameState.Inactive;
        public int Score { get; private set; }

        public static event Action<GameState> OnGameStateChanged; //to notify game of the change of state
        public static event Action<int> OnScoreChanged;

        public static PatronManager PatronManager => Instance ? Instance.GetComponent<PatronManager>() : null;
        public static MenuManager MenuManager => Instance ? Instance.GetComponent<MenuManager>() : null;

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

        private void OnUIStateChanged(MenuManager.UIState oldState, MenuManager.UIState newState)
        {
            switch (newState)
            {
            case MenuManager.UIState.Game:
                if (oldState == MenuManager.UIState.MainMenu)
                {
                    OnGameStart();
                }
                break;
            case MenuManager.UIState.Paused:
                // Nothing
                break;
            default:
                // Anything but game and paused, set state to Inactive.
                UpdateGameState(GameState.Inactive);
                break;
            }
        }

        public void OnGameStart()
        {
            UpdateGameState(GameState.StartOfNight);
            SetScore(0);
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

            OnGameStateChanged?.Invoke(newState); //avoids a null being thrown
        }

        public void AddScore(int addToScore)
        {
            SetScore(Score + addToScore);
        }

        public void SetScore(int newScore)
        {
            Score = newScore;
            OnScoreChanged?.Invoke(Score);
        }
    }
}
