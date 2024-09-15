using TMPro;
using UnityEngine;

using FMS.TapperRedone.Managers;

namespace FMS.TapperRedone.UI
{
	public class Score : MonoBehaviour
	{
		public TextMeshProUGUI ScoreText;

		void Start()
		{
			if (ScoreText == null)
			{
				Debug.LogError("ScoreText is not assigned in the inspector!");
				return;
			}

			ScoreText.text = GameManager.Instance.Score.ToString() + " Space Shillings";
			GameManager.OnScoreChanged += OnScoreChanged;
		}

		private void OnScoreChanged(int newScore)
		{
			ScoreText.text = $"{newScore} Space Shillings";
		}
	}
}
