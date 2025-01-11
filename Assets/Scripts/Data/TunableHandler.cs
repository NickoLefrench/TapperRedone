using UnityEngine;

namespace FMS.TapperRedone.Data
{
    [System.Serializable]
    public struct NpcTunables
    {
        public int MaxCount;
        public float RespawnTime;
        public float MinWaitTime;
        public float MaxWaitTime;
        public float DrinkingTime;
    }

    [System.Serializable]
    public struct NightTunables
    {
        public int Duration;
    }

    [System.Serializable]
    public struct MinigameTunables
    {
        public int BeerScore;
        public int BeerPerfectBonus;
    }

    // A singleton class that manages design tunables - a list of variables defined at editor time.
    public class TunableHandler : MonoBehaviour
    {
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

        public NpcTunables NpcTunables;
        public NightTunables NightTunables;
        public MinigameTunables MinigameTunables;

        // Singleton instance of the handler
        public static TunableHandler Instance;
    }
}
