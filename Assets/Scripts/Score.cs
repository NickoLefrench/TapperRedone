using UnityEngine.UI;
using UnityEngine;

using TMPro;

public class Score : MonoBehaviour
{
    int score = 0;
    public TextMeshPro ScoreText;

    void Start()
    {
        // ScoreText.text = score.ToString() + " Space Shillings";  w regular text ui
    }

    private void Awake()
    {
        // Get a reference to the text component.
        // Since we are using the base class type <TMP_Text> this component could be either a <TextMeshPro> or <TextMeshProUGUI> component.
        ScoreText = GetComponent<TextMeshPro>();

        ScoreText.text = score.ToString() + " Space Shillings";
    }

    // Update is called once per frame
    void Update()
    {





        //ScoreText.text = "Score" + score; w regular text ui
    }
}
