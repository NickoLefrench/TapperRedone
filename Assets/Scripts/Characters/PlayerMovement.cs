using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
	public float WalkSpeed;

	private Rigidbody2D _kinematicBody;

	public void Start()
	{
		_kinematicBody = GetComponent<Rigidbody2D>();
		if ( _kinematicBody == null || _kinematicBody.bodyType != RigidbodyType2D.Kinematic)
		{
			throw new MissingComponentException("A PlayerMovement requires the same game object to have a Kinematic mode Rigidbody2D!");
		}
	}

	// Fixed update is called on a fixed time clock, and is used for physics updates
	private void FixedUpdate()
	{
		HandleMovement();
	}

	void HandleMovement()
	{
		// Player direction, based on horizontal movement axis
		Vector2 horizontalMovement = Vector2.right * Input.GetAxis("Horizontal");
		// Multiply by speed and time to get distance
		Vector2 positionDelta = horizontalMovement * WalkSpeed * Time.fixedDeltaTime;

		_kinematicBody.MovePosition(_kinematicBody.position + positionDelta);
	}
}
