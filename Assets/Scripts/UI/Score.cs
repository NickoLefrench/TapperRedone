using TMPro;
using UnityEngine;

using FMS.TapperRedone.Managers;

namespace FMS.TapperRedone.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class Score : MonoBehaviour
	{
		private TextMeshProUGUI ScoreText;

		private void Awake()
		{
			ScoreText = GetComponent<TextMeshProUGUI>();
		}

		private void Start()
		{
			ScoreText.text = GameManager.StatManager.Score.ToString() + " Space Shillings";
			StatManager.OnScoreChanged += OnScoreChanged;
		}

		private void OnScoreChanged(int newScore)
		{
			ScoreText.text = $"{newScore} Space Shillings";
		}
	}
}
