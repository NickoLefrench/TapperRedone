using System.Collections.Generic;
using UnityEngine;

using FMS.TapperRedone.Managers;
using TMPro;

namespace FMS.TapperRedone.UI
{
	public class StartOfNightHandler : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _nightTitle;
		[SerializeField] private float _titleFadeInDuration = 2.0f;
		[SerializeField] private float _titleStayDuration = 2.0f;
		[SerializeField] private float _titleFadeOutDuration = 2.0f;

		public void Start()
		{
			GameManager.OnGameStateChanged += OnGameStateChanged;
		}

		private void OnGameStateChanged(GameManager.GameState newState)
		{
			switch (newState)
			{
			case GameManager.GameState.StartOfNight:
				RunSequence();
				break;
			case GameManager.GameState.MainGame:
				Cleanup();
				break;
			}
		}

		private LTDescr AdjustAlpha(TextMeshProUGUI text, float to, float time)
		{
			Color color = text.color;
			Color fadeoutColor = new Color(color.r, color.g, color.b, to);
			return LeanTween.value(text.gameObject, color, fadeoutColor, time).setOnUpdateColor(clr => text.color = clr);
		}

		private void Cleanup()
		{
			Color color = _nightTitle.color;
			color.a = 0;
			_nightTitle.color = color;
			_nightTitle.text = $"Night {GameManager.Instance.CurrentNight}";
		}

		private void RunSequence()
		{
			if (_nightTitle == null)
			{
				Debug.LogError("Missing reference to night title!");
				return;
			}

			Cleanup();
			AdjustAlpha(_nightTitle, 1, _titleFadeInDuration).setOnComplete(() =>
			{
				AdjustAlpha(_nightTitle, 0, _titleFadeOutDuration).setDelay(_titleStayDuration).setOnComplete(() =>
				{
					GameManager.Instance.UpdateGameState(GameManager.GameState.MainGame);
				});
			});
		}
	}
}
