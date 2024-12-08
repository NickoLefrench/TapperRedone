using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FMS.TapperRedone.Data
{
	// A singleton class that manages design tunables - a list of variables defined at editor time.
	public class TunableHandler : MonoBehaviour, ISerializationCallbackReceiver
	{
        private static TunableHandler _instance; // Singleton instance, so cocktail minigame can access tunable

        // Ensure the existence of only one tunable handler at any one time
        private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
				return;
			}

			Instance = this;
		}

		// To allow serialize (save), move the dictionary into a list
		public void OnBeforeSerialize()
		{
			_tunableSerialized.Clear();
			foreach (KeyValuePair<string, TunableEntry> pair in _tunables)
			{
				_tunableSerialized.Add(new TunableEntrySerialized(pair.Key, pair.Value));
			}
		}

		// When deserializing, move saved list back into more useful dictionary
		public void OnAfterDeserialize()
		{
			_tunables.Clear();
			foreach (TunableEntrySerialized entry in _tunableSerialized)
			{
				if (!_tunables.TryAdd(entry.Name, entry.Tunable))
				{
					Debug.LogWarning("Could not add " + entry.Name + " with value " + entry.Tunable.Value + " to the dictionary. Patch your serialized tunables.");
					if (!_tunables.TryAdd("", entry.Tunable))
					{
						Debug.LogError("Found an entry with an empty name: Patch your serialized tunables!");
					}
				}
			}
		}

		// Main accessor - get a variable by its name
		public static float GetTunableFloat(string name)
		{
			if (Instance._tunables.TryGetValue(name, out TunableEntry entry))
			{
				return entry.Value;
			}
			else
			{
				Debug.LogWarning($"Requested tunable {name} but no value set for it");
				return 0f;
			}
		}

		public static int GetTunableInt(string name)
		{
			return Mathf.RoundToInt(GetTunableFloat(name));
		}

		// Runtime data structure, name pointing to value
		private Dictionary<string, TunableEntry> _tunables = new();

		// Saved data structure, flat list of names and values
		[SerializeField]
		private List<TunableEntrySerialized> _tunableSerialized = new();

		// Singleton instance of the handler
		private static TunableHandler Instance;

        /* // trying to connect scoremultiplier too 
         public float GetScoreMultiplier()
         {
             // Return score multiplier value
             return someScoreMultiplier;
         }*/

		//Method so that other classes can access instance of tunable handler without making tunable public
        public static TunableHandler GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TunableHandler();
            }
            return _instance;
        }
    }
}
