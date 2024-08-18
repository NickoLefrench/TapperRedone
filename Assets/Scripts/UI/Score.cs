using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class Score : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI ScoreText;

    void Start()
    {
        if (ScoreText != null)
        {
            ScoreText.text = score.ToString() + " Space Shillings";
        }
        else
        {
            Debug.LogError("ScoreText is not assigned in the inspector!");
        }
    }

    // If you want to update the score dynamically:
    public void UpdateScore(int newScore)
    {
        score = newScore;
        if (ScoreText != null)
        {
            ScoreText.text = score.ToString() + " Space Shillings";
        }
    }
}
