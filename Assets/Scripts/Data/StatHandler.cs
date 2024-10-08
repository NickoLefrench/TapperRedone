using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace FMS.TapperRedone.Data
{
    public class StatHandler
    {
        public const string UNKNOWN_STAT = "unknownStat";
        private const string PLAYER_PREFS_FILENAME = "persistentStats.json";

        private Dictionary<string, AbstractStat> sessionStats = new();

        [System.Serializable]
        private struct SavedStat
        {
            public string Name;
            public string Value;
        }

        [SerializeField]
        private List<SavedStat> _savedStats = new();

        public void RegisterStat(AbstractStat stat)
        {
            if (sessionStats.ContainsKey(stat.StatName))
            {
                Debug.LogError($"Cannot add stat {stat.StatName} as one with that name already exists");
            }
            else
            {
                sessionStats.Add(stat.StatName, stat);
            }
        }

        public string GetSessionStatAsString(string statName)
        {
            if (sessionStats.TryGetValue(statName, out AbstractStat stat))
            {
                return stat.GetValueAsString();
            }
            return UNKNOWN_STAT;
        }

        public int GetSessionStatAsInt(string statName)
        {
            if (sessionStats.TryGetValue(statName, out AbstractStat stat) && stat is IntStat)
            {
                return ((IntStat) stat).Value;
            }
            return int.MinValue;
        }

        public bool UpdateSessionStat(string statName, object value)
        {
            if (sessionStats.TryGetValue(statName, out AbstractStat stat))
            {
                return stat.UpdateWithValue(value);
            }
            return false;
        }

        public void LoadPersistentStats(ISaveFileHandler saveFileHandler)
        {
            if (!saveFileHandler.FileExists(PLAYER_PREFS_FILENAME))
                return;

            if (!saveFileHandler.LoadFile(PLAYER_PREFS_FILENAME, out string jsonified))
                return;

            StatHandler parsedStats = JsonUtility.FromJson<StatHandler>(jsonified);
            if (parsedStats == null || parsedStats._savedStats.Count == 0)
                return;

            foreach (SavedStat savedStat in parsedStats._savedStats)
            {
                if (sessionStats.ContainsKey(savedStat.Name))
                {
                    sessionStats[savedStat.Name].SetValueFromString(savedStat.Value);
                }
            }
        }

        public void SavePersistentStats(ISaveFileHandler saveFileHandler)
        {
            _savedStats = sessionStats.Values
                .Where(stat => stat.Lifetime)
                .Select(stat => new SavedStat { Name = stat.StatName, Value = stat.GetValueAsString() })
                .ToList();

            saveFileHandler.SaveFile(PLAYER_PREFS_FILENAME, JsonUtility.ToJson(this));
        }
    }
}
