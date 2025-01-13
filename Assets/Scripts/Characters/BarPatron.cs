using System;
using System.Collections.Generic;

using FMS.TapperRedone.Data;
using FMS.TapperRedone.Interactables;
using FMS.TapperRedone.Inventory;

using UnityEngine;

using static FMS.TapperRedone.Inventory.Item;

using Random = UnityEngine.Random;

namespace FMS.TapperRedone.Characters
{
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
        public Coin CoinsPrefab;
        public float CoinSpawnOffsetY;
        public SpriteRenderer SpeechBubble;
        [SerializeField] private BarPatronSounds patronSounds;

        // TODO after issue #2: Add randomization to cocktail types
        public List<Item.ItemType> OrderOptions;
        public List<Sprite> OrderSprites;

        // What is the maximum time can stay in this state; means different things depending on active state
        public float StateTimeRemaining { get; private set; }
        public State CurrentState { get; private set; }
        public Item.ItemType OrderItem { get; private set; }

        private Vector3 spawnPosition;
        private Transform seat;
        private Item receivedItem = null;

        //what is the batron allowed ordering
        public enum OrderType { Beer, Cocktail }

        private List<OrderType> allowedOrders;

        //validating orders
        public void SetAllowedOrders(List<OrderType> orders)
        {
            allowedOrders = orders;
            Debug.Log($"Patron {name}: Allowed orders updated to {string.Join(", ", orders)}");
        }

        public OrderType GetOrder()
        {
            if (allowedOrders == null || allowedOrders.Count == 0)
            {
                Debug.LogError("Allowed orders are not set for this patron!");
                return OrderType.Beer; // Default to Beer as a fallback
            }

            return allowedOrders[Random.Range(0, allowedOrders.Count)];
        }


        public void Setup(Transform spawnTransform, Transform assignedSeat)
        {
            transform.SetParent(assignedSeat);
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
                HideSpeechBubble();
                UpdateState(State.WalkingIn);
                break;
            case State.WalkingIn:
                // Move towards seat on the horizontal axis
                bool reachedSeat = MoveTowards(seat.position);
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
                    HideSpeechBubble();
                    UpdateState(State.Leaving);
                }
                break;
            case State.Drinking:
                ranOutOfTime = AdvanceStateTime();
                if (ranOutOfTime)
                {
                    // Give up some coins
                    SpawnCoins();
                    // Ready to leave!
                    HideSpeechBubble();
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
                Managers.PatronManager.Instance.CleanupPatron(this);
                Destroy(gameObject);
                break;
            default:
                throw new NotImplementedException($"Missing implementation for state {CurrentState} in BarPatron.Update");
            }
        }

        private void UpdateState(State state)
        {
            Debug.Log($"Updating state of {gameObject.name} from {CurrentState} to {state}");
            CurrentState = state;
        }

        // Returns whether target is reached as part of this frame
        private bool MoveTowards(Vector3 targetPosition)
        {
            float hereX = transform.position.x;
            float targetX = targetPosition.x;
            float distanceLeft = Mathf.Abs(targetX - hereX);
            float distanceExpected = WalkSpeed * Time.deltaTime;
            bool willReachTarget = distanceLeft <= distanceExpected;
            float distance = willReachTarget ? distanceLeft : distanceExpected;

            float directionSign = Mathf.Sign(targetX - hereX);
            Vector3 movementDirection = directionSign * Vector3.right;
            Vector3 movementDelta = movementDirection * distance;
            transform.Translate(movementDelta);

            SpriteRenderer selfRender = GetComponent<SpriteRenderer>();
            if (selfRender != null)
            {
                selfRender.flipX = directionSign < 0;
            }

            return willReachTarget;
        }

        private void SelectOrderParameters()
        {
            float minTime = TunableHandler.GetTunableFloat("NPC.MIN_WAIT_TIME");
            float maxTime = TunableHandler.GetTunableFloat("NPC.MAX_WAIT_TIME");
            StateTimeRemaining = UnityEngine.Random.Range(minTime, maxTime);

            int chosenItemIdx = UnityEngine.Random.Range(0, OrderOptions.Count - 1);
            OrderItem = OrderOptions[chosenItemIdx];
            SpeechBubble.sprite = OrderSprites[chosenItemIdx];
            SpeechBubble.gameObject.SetActive(true);


        }

        private void HideSpeechBubble()
        {
            SpeechBubble.gameObject.SetActive(false);
        }

        private void SpawnCoins()
        {
            Vector3 coinLocation = transform.position + new Vector3(0, CoinSpawnOffsetY, 0);
            Coin spawnedCoins = Instantiate(CoinsPrefab, coinLocation, Quaternion.identity);

            spawnedCoins.scoreValue = receivedItem.itemScore;
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

        public override void Interact(PlayerInteraction player)
        {
            base.Interact(player);
            if (CurrentState != State.WaitingOrder)
            {
                return;
            }

            // Interact if player has a drink in inventory, take it, drink if success, straight leave if fail
            if (player.CurrentInventory.HasDrink())
            {
                Item returnedItem = player.CurrentInventory.RemoveDrink();
                receivedItem = returnedItem;
                if (returnedItem.itemType == OrderItem)
                {
                    // Correct order fulfilled - start drinking
                    patronSounds.PlaySound(BarPatronSounds.SoundType.HappyOrder);
                    StateTimeRemaining = TunableHandler.GetTunableFloat("NPC.DRINKING_TIME");
                    UpdateState(State.Drinking);
                }
                else
                {
                    // Incorrect order - just leave
                    patronSounds.PlaySound(BarPatronSounds.SoundType.SadOrder);
                    HideSpeechBubble();
                    UpdateState(State.Leaving);
                }
            }
        }

        public void RegenerateOrder()
        {
            OrderType newOrder = GetOrder(); // Generate a new order
            Debug.Log($"{name} regenerated order: {newOrder}");
        }
    }
}
