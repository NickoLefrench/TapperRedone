
namespace FMS.TapperRedone.Data
{
	// A struct to support tunable values
	// Why not just use a float? This allows us to be ready for a future where we might want to store different types
	[System.Serializable]
	public class TunableEntry
	{
		public float Value;
	}

	// Simple name + TunableEntry, to allow to serialize a Dictionary<string, TunableValue) as a List<>
	[System.Serializable]
	public class TunableEntrySerialized
	{
		public TunableEntrySerialized(string name, TunableEntry tunable)
		{
			Name = name;
			Tunable = tunable;
		}

		public string Name;
		public TunableEntry Tunable;
	}
}
