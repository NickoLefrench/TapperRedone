using System;
using UnityEditor;
using UnityEngine;

namespace FMS.TapperRedone.Managers
{
	public class MenuManager : MonoBehaviour
	{
		public static MenuManager Instance => GameManager.MenuManager;

		public enum UIState
		{
			MainMenu,
			Credits,
			Settings,
			Game,
			EndOfNightScoreboard,
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

			// Should this be in GameManager? 
			Time.timeScale = RequiresTimeScale ? 1.0f : 0.0f;

			OnUIStateChanged?.Invoke(oldState, newState);
		}

		public void OnStartGame()
		{
			Debug.Log("Starting game");
			SafeUpdateUIState(UIState.MainMenu, UIState.Game);
		}

		public void OnMainMenu()
		{
			SafeUpdateUIState(UIState.Paused, UIState.MainMenu);
		}

		public void OnQuitGame()
		{
#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif
		}

		public void OnSettingsMenu()
		{
			SafeUpdateUIState(UIState.MainMenu, UIState.Settings);
		}

		public void OnExitSettings()
		{
			SafeUpdateUIState(UIState.Settings, UIState.MainMenu);
		}

		public void OnNextNight() //update scroeboard in btween nights?
		{
			SafeUpdateUIState(UIState.EndOfNightScoreboard, UIState.Game);
		}

		private void SafeUpdateUIState(UIState requiredState, UIState toState)
		{
			if (State != requiredState)
			{
				Debug.LogError($"Trying to go to state {toState}, from expected state {requiredState}, but currently in state {State}");
				return;
			}

			UpdateUIState(toState);
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
