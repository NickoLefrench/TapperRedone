using FMS.TapperRedone.Managers;

using TMPro;

using UnityEngine;

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
            ScoreText.text = GameManager.Instance.Score.ToString() + " Space Shillings";
            GameManager.OnScoreChanged += OnScoreChanged;
        }

        private void OnScoreChanged(int newScore)
        {
            ScoreText.text = $"{newScore} Space Shillings";
        }
    }
}
