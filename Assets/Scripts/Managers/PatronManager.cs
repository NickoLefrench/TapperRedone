using System.Collections.Generic;
using System.Linq;

using FMS.TapperRedone.Characters;
using FMS.TapperRedone.Data;

using UnityEngine;
using UnityEngine.Assertions;

namespace FMS.TapperRedone.Managers
{
    // The PatronManager, a component on the singleton GameManager, spawns BarPatrons for the bar
    public class PatronManager : MonoBehaviour
    {
        public BarPatron PatronPrefab;
        public GameObject SpawnLocation;
        public GameObject SeatsParent;

        public int CurrentPatrons { get; private set; } = 0;

        private bool running = false;
        private int maxPatrons;
        private int totalPatrons = 0;
        private float nextSpawnTime = Mathf.NegativeInfinity;
        private float respawnTimer;
        private Dictionary<Transform, BarPatron> managedSeats = new();

        public static PatronManager Instance => GameManager.PatronManager;

        public void Start()
        {
            maxPatrons = TunableHandler.GetTunableInt("NPC.MAX_COUNT");
            respawnTimer = TunableHandler.GetTunableFloat("NPC.RESPAWN_TIMER");

            Assert.AreNotEqual(SeatsParent, null, "The empty GameObject containing all the available seat positions must be a set parameter on PatronManager!");
            if (SeatsParent != null)
            {
                int seatsCount = SeatsParent.transform.childCount;
                managedSeats.EnsureCapacity(seatsCount);
                for (int i = 0; i < seatsCount; i++)
                {
                    managedSeats.Add(SeatsParent.transform.GetChild(i), null);
                }
            }

            GameManager.OnGameStateChanged += OnGameStateChanged;

            //StartNight(1);
        }

        private void OnGameStateChanged(GameManager.GameState newState)
        {
            bool newRunning = newState == GameManager.GameState.MainGame || newState == GameManager.GameState.BeerMiniGame || newState == GameManager.GameState.CocktailMiniGame;
            if (running && !newRunning)
            {
                ResetAllPatrons();
            }
            else if (!running && newRunning)
            {
                nextSpawnTime = Time.time;
            }

            running = newRunning;
        }

        private void ResetAllPatrons()
        {
            // Need to set ToList, as we are modifying the dictionary while iterating on it, and that can break iterator.
            foreach (Transform seat in managedSeats.Keys.ToList())
            {
                if (managedSeats[seat] != null)
                {
                    Destroy(managedSeats[seat].gameObject);
                    managedSeats[seat] = null;
                }
            }

            running = false;
            CurrentPatrons = 0;
            totalPatrons = 0;
            nextSpawnTime = Mathf.NegativeInfinity;
        }

        public void CleanupPatron(BarPatron patron)
        {
            KeyValuePair<Transform, BarPatron> foundPair = managedSeats.FirstOrDefault(pair => pair.Value == patron);
            // Could be unequal if we get default(KeyValuePair<>) as returned value
            if (foundPair.Value == patron)
            {
                managedSeats[foundPair.Key] = null;
                CurrentPatrons--;
            }
            // nextSpawnTime = Time.time + respawnTimer;
        }

        public void Update()
        {
            if (running && GameManager.Instance.RemainingTime > 0.0f && CurrentPatrons < maxPatrons && nextSpawnTime <= Time.time)
            {
                SpawnNewPatron();
            }
        }

        private void SpawnNewPatron()
        {
            // Select seat to spawn in - random from empty options
            int randomSeat = UnityEngine.Random.Range(0, managedSeats.Count - CurrentPatrons);
            Transform selectedSeat = null;
            foreach (Transform seat in managedSeats.Keys)
            {
                // Skip already occupied
                if (managedSeats[seat] != null)
                {
                    continue;
                }

                // Skip empty seat if not reached index
                if (randomSeat > 0)
                {
                    randomSeat--;
                    continue;
                }

                // Found seat
                selectedSeat = seat;
                break;
            }

            if (selectedSeat == null)
            {
                Debug.LogError($"Failed to find an empty seat despite {CurrentPatrons} out of {managedSeats.Count} seats occupied");
                return;
            }

            // Spawn object
            BarPatron newPatron = Instantiate(PatronPrefab);
            managedSeats[selectedSeat] = newPatron;

            // Set any final properties
            totalPatrons++;
            CurrentPatrons++;
            string newName = $"Patron {totalPatrons}";
            newPatron.gameObject.name = newName;
            newPatron.Setup(SpawnLocation.transform, selectedSeat);
            Debug.Log($"Spawned {newName} on seat {selectedSeat.name}");

            nextSpawnTime = Time.time + respawnTimer;

            // Determine allowed orders based on the night
            var allowedOrders = GetAllowedOrders();
            newPatron.SetAllowedOrders(allowedOrders);
        }

        public List<BarPatron> GetActivePatrons()
        {
            List<BarPatron> patrons = new();
            foreach (var seat in managedSeats.Values)
            {
                if (seat != null)
                {
                    patrons.Add(seat);
                }
            }
            return patrons;
        }

        public void SetPatronOrderPreferences(List<BarPatron.OrderType> allowedOrders)
        {
            /*foreach (var patron in managedSeats.Values.Where(p => p != null))
            {
                patron.SetAllowedOrders(allowedOrders);
            }*/

            var activePatrons = GetActivePatrons();
            foreach (var patron in activePatrons)
            {
                patron.SetAllowedOrders(allowedOrders);
               
            }
        }

        private List<BarPatron.OrderType> GetAllowedOrders()
        {
            int currentNight = ProgressionManager.Instance.GetCurrentNight();

            return currentNight switch
            {
                1 => new List<BarPatron.OrderType> { BarPatron.OrderType.Beer },
                2 => new List<BarPatron.OrderType> { BarPatron.OrderType.Cocktail },
                _ => new List<BarPatron.OrderType> { BarPatron.OrderType.Beer, BarPatron.OrderType.Cocktail }
            };
        }
    }
}
