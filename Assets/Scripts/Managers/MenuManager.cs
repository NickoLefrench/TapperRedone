using System;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
	public GameObject PauseScreen;
	public GameObject MainMenuScreen;

	public enum UIState
    {
        MainMenu,
        Credits,
        Settings,
        Game,
        Paused
    }

	public event Action<UIState> OnUIStateChanged;

	public UIState State { get; private set; } = UIState.MainMenu;

    public void UpdateUIState(UIState newState)
    {
		Debug.Log($"Updating state of MenuManager from {State} to {newState}");
		State = newState;

        OnUIStateChanged?.Invoke(State);
	}

	void Update()
	{
		ProcessPause();
	}

	public void OnStartGame()
	{
		if (State != UIState.MainMenu)
		{
			Debug.LogError($"Trying to click Main Menu Play game but currently in state {State}");
			return;
		}

		Debug.Log("Starting game");
		MainMenuScreen.SetActive(false);
		UpdateUIState(UIState.Game);
	}

	private void ProcessPause()
	{
		// Can only swap between Pause and Game states.
		if (Input.GetButtonDown("Pause"))
		{
			switch (State)
			{
			case UIState.Game:
				Time.timeScale = 0f;
				PauseScreen.SetActive(true);
				UpdateUIState(UIState.Paused);
				break;
			case UIState.Paused:
				Time.timeScale = 1f;
				PauseScreen.SetActive(false);
				UpdateUIState(UIState.Game);
				break;
			default:
				break;
			}
		}
	}
}
