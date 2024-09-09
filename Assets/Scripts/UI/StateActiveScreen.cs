using System.Collections.Generic;
using UnityEngine;

using FMS.TapperRedone.Managers;

namespace FMS.TapperRedone.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class StateActiveScreen : MonoBehaviour
	{
		public List<MenuManager.UIState> activeStates;
		public bool handleInteraction;

		private CanvasGroup _controlComponent;

		public void Start()
		{
			_controlComponent = GetComponent<CanvasGroup>();
			OnUIStateChanged(MenuManager.Instance.State, MenuManager.Instance.State);
			MenuManager.Instance.OnUIStateChanged += OnUIStateChanged;
		}

		private void OnDestroy()
		{
			if (MenuManager.Instance != null)
			{
				MenuManager.Instance.OnUIStateChanged -= OnUIStateChanged;
			}
		}

		private void OnUIStateChanged(MenuManager.UIState oldState, MenuManager.UIState newState)
		{
			bool isActive = activeStates.Contains(newState);
			_controlComponent.alpha = isActive ? 1f : 0f;
			if (handleInteraction)
			{
				_controlComponent.interactable = isActive;
				_controlComponent.blocksRaycasts = isActive;
			}
		}
	}
}
