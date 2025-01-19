using FMS.TapperRedone.Managers;

using TMPro;

using UnityEngine;

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
            _nightTitle.text = $"Night {GameManager.StatManager.CurrentNight}";
        }

        private void RunSequence()
        {
            if (_nightTitle == null)
            {
                Debug.LogError("Missing reference to night title!");
                return;
            }

            AdjustSavedData();
            Cleanup();
            AdjustAlpha(_nightTitle, 1, _titleFadeInDuration).setOnComplete(() =>
            {
                AdjustAlpha(_nightTitle, 0, _titleFadeOutDuration).setDelay(_titleStayDuration).setOnComplete(() =>
                {
                    GameManager.Instance.UpdateGameState(GameManager.GameState.MainGame);
                });
            });
        }

        private void AdjustSavedData()
        {
            // Don't reset score over night, instead save it as it could be consumed for "upgrades".
            // GameManager.StatManager.Score = 0;
            GameManager.StatManager.CurrentNight++;

            GameManager.StatManager.savedData.CurrentBeers = 0;
        }
    }
}
