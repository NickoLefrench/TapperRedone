using System;
using UnityEngine;

using FMS.TapperRedone.Data;

namespace FMS.TapperRedone
{
	public enum StatName
	{
		Score,
	}
}

namespace FMS.TapperRedone.Managers
{
	public class StatManager : MonoBehaviour
	{
		public static event Action<int> OnScoreChanged;
		public static event Action<StatName, string> OnStatUpdated;

		private StatHandler statHandler = new();

		private void Start()
		{
			// Register all stats
			statHandler.RegisterStat(new IntStat(StatName.Score.ToString(), false, IntStat.OverrideFunc));

			// Reload any stat values from deep storage
			statHandler.LoadPersistentStats();
		}

		public void RequestStatSave()
		{
			statHandler.SavePersistentStats();
		}

		public int GetIntStat(StatName name)
		{
			return statHandler.GetSessionStatAsInt(name.ToString());
		}

		public void UpdateIntStat(StatName name, int updateValue)
		{
			bool success = statHandler.UpdateSessionStat(name.ToString(), updateValue);
			if (success)
			{
				OnStatUpdated?.Invoke(name, statHandler.GetSessionStatAsString(name.ToString()));
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