using TMPro;
using UnityEngine;

using FMS.TapperRedone.Managers;

namespace FMS.TapperRedone.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class RemainingTime : MonoBehaviour
	{
		private TextMeshProUGUI TextComponent;
		private const float minuteMultiplier = 1.0f / 60.0f;

		private void Awake()
		{
			TextComponent = GetComponent<TextMeshProUGUI>();
		}

		private void Update()
		{
			float remainingTime = GameManager.Instance.RemainingTime;
			int remMinutes = Mathf.FloorToInt(remainingTime * minuteMultiplier);
			int remSeconds = Mathf.FloorToInt(remainingTime) % 60;
			TextComponent.text = $"Time left: {remMinutes}:{remSeconds:00}";
		}
	}
}
