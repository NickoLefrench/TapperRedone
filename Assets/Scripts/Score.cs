using UnityEngine.UI;
using UnityEngine;

public class Score : MonoBehaviour
{
    int score = 0;
    public Text ScoreText;

    void Start()
    {
        ScoreText.text = score.ToString() + " Space Shillings";
    }

    // Update is called once per frame
    void Update()
    {





        ScoreText.text = "Score" + score;
    }
}
