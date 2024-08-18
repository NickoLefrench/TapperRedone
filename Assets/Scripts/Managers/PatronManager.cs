using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The PatronManager, a component on the singleton GameManager, spawns BarPatrons for the bar
public class PatronManager : MonoBehaviour
{
	public BarPatron PatronPrefab;
	public GameObject SpawnLocation;
	public GameObject SeatsParent;

	private int patronCounter = 0;
	private int maxPatrons;
	private float lastSpawnTime = Mathf.NegativeInfinity;
	private float respawnTimer;

	private List<BarPatron> managedPatrons = new();

	public void Start()
	{
		maxPatrons = TunableHandler.GetTunableInt("NPC.MAX_COUNT");
		respawnTimer = TunableHandler.GetTunableFloat("NPC.RESPAWN_TIMER");
	}

	public void Update()
	{
		managedPatrons.RemoveAll(patron => (patron == null || patron.CurrentState == BarPatron.State.Despawning));
		if (managedPatrons.Count < maxPatrons && lastSpawnTime + respawnTimer <= Time.time)
		{
			SpawnNewPatron();
		}
	}

	private void SpawnNewPatron()
	{
		BarPatron newPatron = Instantiate<BarPatron>(PatronPrefab);
		patronCounter++;
		newPatron.gameObject.name = $"Patron {patronCounter}";
		managedPatrons.Add(newPatron);
		lastSpawnTime = Time.time;
		int randomSeat = UnityEngine.Random.Range(0, SeatsParent.transform.childCount);
		newPatron.Setup(SpawnLocation.transform, SeatsParent.transform.GetChild(randomSeat));
	}
}
