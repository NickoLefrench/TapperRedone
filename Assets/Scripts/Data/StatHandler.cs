using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FMS.TapperRedone.Data
{
	public class StatHandler
	{
		public const string UNKNOWN_STAT = "unknownStat"; 
		private const string PLAYER_PREFS_KEY = "persistentStats";

		private Dictionary<string, AbstractStat> sessionStats = new();

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
				return ((IntStat)stat).Value;
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

		public void LoadPersistentStats()
		{
			if (!PlayerPrefs.HasKey(PLAYER_PREFS_KEY))
				return;

			string jsonified = PlayerPrefs.GetString(PLAYER_PREFS_KEY);
			if (jsonified == "")
				return;

			Dictionary<string, string> parsedStats = JsonUtility.FromJson<Dictionary<string, string>>(jsonified);
			if (parsedStats == null || parsedStats.Count == 0)
				return;

			foreach (var keyValuePair in parsedStats)
			{
				if (sessionStats.ContainsKey(keyValuePair.Key))
				{
					sessionStats[keyValuePair.Key].SetValueFromString(keyValuePair.Value);
				}
			}
		}

		public void SavePersistentStats()
		{
			Dictionary<string, string> savedStats = sessionStats.Values
				.Where(stat => stat.Lifetime)
				.ToDictionary(stat => stat.StatName, stat => stat.GetValueAsString());
			PlayerPrefs.SetString(PLAYER_PREFS_KEY, JsonUtility.ToJson(savedStats));
			PlayerPrefs.Save();
		}
	}
}