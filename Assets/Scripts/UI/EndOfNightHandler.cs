using FMS.TapperRedone.Data;
using FMS.TapperRedone.Managers;

using TMPro;

using UnityEngine;

namespace FMS.TapperRedone.UI
{
    public class EndOfNightHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform _coinsRow;
        [SerializeField] private RectTransform _beersRow;

        private const string CURRENT_VALUE = "CurrentValue";
        private const string RUN_VALUE = "RunValue";
        private const string LIFETIME_VALUE = "LifetimeValue";

        private void Start()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameManager.GameState newState)
        {
            switch (newState)
            {
            case GameManager.GameState.EndofNight:
                RunSequence();
                break;
            default:
                Cleanup();
                break;
            }
        }

        private void Cleanup()
        {
            // Do anything that needs cleanup after show, or right before show
        }

        private void UpdateSavedData()
        {
            SavedData savedData = GameManager.StatManager.savedData;

            savedData.RunTotalScore += savedData.CurrentScore;
            savedData.LifetimeTotalScore += savedData.CurrentScore;

            UpdateRow(_coinsRow, savedData.CurrentScore, savedData.RunTotalScore, savedData.LifetimeTotalScore);

            savedData.RunTotalBeers += savedData.CurrentBeers;
            savedData.LifetimeTotalBeers += savedData.CurrentBeers;

            UpdateRow(_beersRow, savedData.CurrentBeers, savedData.RunTotalBeers, savedData.LifetimeTotalBeers);

            savedData.LifetimeBestNight = Mathf.Max(savedData.LifetimeBestNight, savedData.RunNight);
        }

        private void TryUpdateRowField(RectTransform field, string expectedName, int valueIfMatchesName)
        {
            if (field.name == expectedName)
            {
                if (field.TryGetComponent<TextMeshProUGUI>(out var textComponent))
                {
                    textComponent.text = valueIfMatchesName.ToString();
                }
            }
        }

        private void UpdateRow(RectTransform rowContainer, int currentValue, int runValue, int lifetimeValue)
        {
            foreach (RectTransform textInRow in rowContainer)
            {
                TryUpdateRowField(textInRow, CURRENT_VALUE, currentValue);
                TryUpdateRowField(textInRow, RUN_VALUE, runValue);
                TryUpdateRowField(textInRow, LIFETIME_VALUE, lifetimeValue);
            }
        }

        private void RunSequence()
        {
            // Do anything when this screen is asked to appear
            Cleanup();

            UpdateSavedData();
            GameManager.StatManager.RequestStatSave();
        }
    }
}
