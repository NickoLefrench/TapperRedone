using System.Collections.Generic;
using UnityEngine;

using static FMS.TapperRedone.Managers.MenuManager;

namespace FMS.TapperRedone.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class StateActiveScreen : MonoBehaviour
	{
		public List<UIState> activeStates;
		public bool handleInteraction;

		private CanvasGroup _controlComponent;

		public void Start()
		{
			Instance.OnUIStateChanged += OnUIStateChanged;

			_controlComponent = GetComponent<CanvasGroup>();
		}

		private void OnDestroy()
		{
			if (Instance != null)
			{
				Instance.OnUIStateChanged -= OnUIStateChanged;
			}
		}

		private void OnUIStateChanged(UIState oldState, UIState newState)
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
