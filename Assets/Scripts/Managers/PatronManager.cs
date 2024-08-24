using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

// The PatronManager, a component on the singleton GameManager, spawns BarPatrons for the bar
public class PatronManager : MonoBehaviour
{
	public BarPatron PatronPrefab;
	public GameObject SpawnLocation;
	public GameObject SeatsParent;

	private int maxPatrons;
	private int currentPatrons = 0;
	private int totalPatrons = 0;
	private float nextSpawnTime = Mathf.NegativeInfinity;
	private float respawnTimer;

	private Dictionary<Transform, BarPatron> managedSeats = new();

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
	}

	public void CleanupPatron(BarPatron patron)
	{
		KeyValuePair<Transform, BarPatron> foundPair = managedSeats.FirstOrDefault(pair => pair.Value == patron);
		// Could be unequal if we get default(KeyValuePair<>) as returned value
		if (foundPair.Value == patron)
		{
			managedSeats[foundPair.Key] = null;
			currentPatrons--;
		}
		// nextSpawnTime = Time.time + respawnTimer;
	}

	public void Update()
	{
		if (currentPatrons < maxPatrons && nextSpawnTime <= Time.time)
		{
			SpawnNewPatron();
		}
	}

	private void SpawnNewPatron()
	{
		// Select seat to spawn in - random from empty options
		int randomSeat = UnityEngine.Random.Range(0, managedSeats.Count - currentPatrons);
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
			Debug.LogError($"Failed to find an empty seat despite {currentPatrons} out of {managedSeats.Count} seats occupied");
			return;
		}

		// Spawn object
		BarPatron newPatron = Instantiate(PatronPrefab);
		managedSeats[selectedSeat] = newPatron;

		// Set any final properties
		totalPatrons++;
		currentPatrons++;
		string newName = $"Patron {totalPatrons}";
		newPatron.gameObject.name = newName;
		newPatron.Setup(SpawnLocation.transform, selectedSeat);
		Debug.Log($"Spawned {newName} on seat {selectedSeat.name}");

		nextSpawnTime = Time.time + respawnTimer;
	}
}
