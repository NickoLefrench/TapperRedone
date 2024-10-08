using System;

using FMS.TapperRedone.Data;

using UnityEngine;

namespace FMS.TapperRedone
{
    public enum StatName
    {
        Score,
        LifetimeScore,
    }
}

namespace FMS.TapperRedone.Managers
{
    public class StatManager : MonoBehaviour
    {
        public static event Action<int> OnScoreChanged;
        public static event Action<StatName, string> OnStatUpdated;

        private readonly StatHandler _statHandler = new();
        private ISaveFileHandler _saveFileHandler;

        private void Start()
        {
            // Set up file handler
            _saveFileHandler = new PersistentSaveFileHandler();

            // Register all stats
            _statHandler.RegisterStat(new IntStat(StatName.Score.ToString(), false, IntStat.OverrideFunc));
            _statHandler.RegisterStat(new IntStat(StatName.LifetimeScore.ToString(), true, IntStat.AddFunc));

            // Reload any stat values from deep storage
            _statHandler.LoadPersistentStats(_saveFileHandler);
        }

        public void RequestStatSave()
        {
            _statHandler.SavePersistentStats(_saveFileHandler);
        }

        public int GetIntStat(StatName name)
        {
            return _statHandler.GetSessionStatAsInt(name.ToString());
        }

        public void UpdateIntStat(StatName name, int updateValue)
        {
            bool success = _statHandler.UpdateSessionStat(name.ToString(), updateValue);
            if (success)
            {
                OnStatUpdated?.Invoke(name, _statHandler.GetSessionStatAsString(name.ToString()));
            }
        }

        public int Score
        {
            get
            {
                return GetIntStat(StatName.Score);
            }
            set
            {
                UpdateIntStat(StatName.Score, value);
                OnScoreChanged?.Invoke(Score);
            }
        }

        public void AddScore(int score)
        {
            Score += score;
        }
    }
}
