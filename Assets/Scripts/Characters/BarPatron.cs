using System;
using UnityEngine;

public class BarPatron : InteractableObject
{
	public enum State
	{
		Spawning,
		WalkingIn,
		Ordering,
		WaitingOrder,
		Drinking,
		Leaving,
		Despawning
	};

	public float WalkSpeed;

	// What is the maximum time can stay in this state; means different things depending on active state
	public float StateTimeRemaining { get; private set; }
	public State CurrentState { get; private set; }
	public Item.ItemType OrderItem { get; private set; }

	private Vector3 spawnPosition;
	private GameObject seat;
	private Item receivedItem = null;

	public BarPatron(Transform spawnTransform, GameObject assignedSeat)
	{
		spawnPosition = spawnTransform.position;
		transform.SetPositionAndRotation(spawnTransform.position, spawnTransform.rotation);
		seat = assignedSeat;
		CurrentState = State.Spawning;
	}

	public void Update()
	{
		bool ranOutOfTime;

		switch (CurrentState)
		{
		case State.Spawning:
			// A manager will spawn us only when walking in is possible - transition straight away.
			UpdateState(State.WalkingIn);
			break;
		case State.WalkingIn:
			// Move towards seat on the horizontal axis
			bool reachedSeat = MoveTowards(seat.transform.position);
			if (reachedSeat)
			{
				UpdateState(State.Ordering);
			}
			break;
		case State.Ordering:
			// In current implementation, order is automatically selected, does not require interaction
			// In very first implementation, only orders beer.
			SelectOrderParameters();
			UpdateState(State.WaitingOrder);
			break;
		case State.WaitingOrder:
			ranOutOfTime = AdvanceStateTime();
			if (ranOutOfTime)
			{
				// Order failed!
				receivedItem = null;
				UpdateState(State.Leaving);
			}
			break;
		case State.Drinking:
			ranOutOfTime = AdvanceStateTime();
			if (ranOutOfTime)
			{
				// Ready to leave!
				UpdateState(State.Leaving);
			}
			break;
		case State.Leaving:
			// Move back towards spawn on the horizontal axis
			bool reachedSpawn = MoveTowards(spawnPosition);
			if (reachedSpawn)
			{
				UpdateState(State.Despawning);
			}
			break;
		case State.Despawning:
			// Bye-bye!
			Destroy(this);
			break;
		default:
			throw new NotImplementedException($"Missing implementation for state {CurrentState} in BarPatron.Update");
		}
	}

	private void UpdateState(State state)
	{
		Debug.Log($"Updating state of {gameObject.name} from {this.CurrentState} to {state}");
		this.CurrentState = state;
	}

	// Returns whether target is reached as part of this frame
	private bool MoveTowards(Vector3 targetPosition)
	{
		float hereX = transform.position.x;
		float targetX = targetPosition.x;
		float distanceLeft = Mathf.Abs(targetX - hereX);
		float distanceExpected = WalkSpeed * Time.deltaTime;
		bool willReachTarget = distanceExpected <= distanceLeft;
		float distance = willReachTarget ? distanceLeft : distanceExpected;
		
		Vector3 movementDirection = Mathf.Sign(targetX - hereX) * Vector3.right;
		Vector3 movementDelta = movementDirection * distance;
		transform.Translate(movementDelta);

		return willReachTarget;
	}

	private void SelectOrderParameters()
	{
		float minTime = TunableHandler.GetTunableFloat("NPC.MIN_WAIT_TIME");
		float maxTime = TunableHandler.GetTunableFloat("NPC.MAX_WAIT_TIME");
		StateTimeRemaining = UnityEngine.Random.Range(minTime, maxTime);
		// For now, simple only Beer option.
		OrderItem = Item.ItemType.Beer;
	}

	// Returns whether state time has reached 0.
	private bool AdvanceStateTime()
	{
		if (StateTimeRemaining < Time.deltaTime)
		{
			// Order failed!
			StateTimeRemaining = 0f;
			return true;
		}
		else
		{
			StateTimeRemaining -= Time.deltaTime;
			return false;
		}
	}

	public override void Interact(PlayerScript player)
	{
		base.Interact(player);
		if (CurrentState != State.WaitingOrder)
		{
			return;
		}

		Item returnedItem = player.AttemptInventoryTransaction(OrderItem);
		if (returnedItem != null)
		{
			receivedItem = returnedItem;
			UpdateState(State.Drinking);
		}
	}
}
