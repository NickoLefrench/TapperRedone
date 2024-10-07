using FMS.TapperRedone.Managers;

using TMPro;

using UnityEngine;

namespace FMS.TapperRedone.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))] //confirms text component present
    public class RemainingTime : MonoBehaviour
    {
        private TextMeshProUGUI TextComponent;
        private const float minuteMultiplier = 1.0f / 60.0f; //convert seconds to minutes

        private void Awake()
        {
            TextComponent = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            float remainingTime = GameManager.Instance.RemainingTime;               //gets remaining time from gamemanager
            int remMinutes = Mathf.FloorToInt(remainingTime * minuteMultiplier);    //converts remaining time to minutes
            int remSeconds = Mathf.FloorToInt(remainingTime) % 60;                  //retrieves remaining seconds and modul  by 60
            TextComponent.text = $"Time left: {remMinutes}:{remSeconds:00}";
        }
    }
}
