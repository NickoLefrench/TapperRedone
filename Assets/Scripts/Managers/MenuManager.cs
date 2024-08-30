using System;
using UnityEngine;

namespace FMS.TapperRedone.Managers
{
	public class MenuManager : MonoBehaviour
	{
		public GameObject PauseScreen;
		public GameObject MainMenuScreen;

		public static MenuManager Instance => GameManager.MenuManager;

		public enum UIState
		{
			MainMenu,
			Credits,
			Settings,
			Game,
			Paused
		}

		public event Action<UIState, UIState> OnUIStateChanged;

		public UIState State { get; private set; }
		private bool RequiresTimeScale => State == UIState.Game;

		public void UpdateUIState(UIState newState)
		{
			UpdateUIState(newState, false);
		}

		private void UpdateUIState(UIState newState, bool forced)
		{
			if (newState == State && !forced)
			{
				return;
			}

			Debug.Log($"Updating state of MenuManager from {State} to {newState}");
			UIState oldState = State;
			State = newState;

			PauseScreen.SetActive(newState == UIState.Paused);
			MainMenuScreen.SetActive(newState == UIState.MainMenu);
			// Should this be in GameManager? 
			Time.timeScale = RequiresTimeScale ? 1.0f : 0.0f;

			OnUIStateChanged?.Invoke(oldState, newState);
		}

		public void OnStartGame()
		{
			if (State != UIState.MainMenu)
			{
				Debug.LogError($"Trying to click Main Menu Play game but currently in forbidden UI state {State}");
				return;
			}

			Debug.Log("Starting game");
			UpdateUIState(UIState.Game);
		}

		public void OnMainMenu()
		{
			if (State != UIState.Paused)
			{
				Debug.LogError($"Trying to go to Main Menu but currently in forbidden UI state {State}");
				return;
			}

			UpdateUIState(UIState.MainMenu);
		}

		private void Start()
		{
			UpdateUIState(UIState.MainMenu, true);
		}

		private void Update()
		{
			ProcessPause();
		}

		private void ProcessPause()
		{
			// Can only swap between Pause and Game states.
			if (Input.GetButtonDown("Pause"))
			{
				switch (State)
				{
				case UIState.Game:
					UpdateUIState(UIState.Paused);
					break;
				case UIState.Paused:
					UpdateUIState(UIState.Game);
					break;
				default:
					break;
				}
			}
		}
	}
}
