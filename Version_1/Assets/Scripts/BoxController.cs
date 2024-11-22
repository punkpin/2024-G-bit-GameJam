using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
	[Header ( "LayerMask" )]
	public LayerMask obstacleLayer; 
	public LayerMask boxLayer; 

	public bool TryMove ( Vector2 direction )
	{
		// Find obstacle on the direction
		Vector2 targetPosition = ( Vector2 ) transform.position + direction;

		// Raycast judgemnet to find ovelap box
		if ( IsValidMove ( targetPosition ) )
		{
			// move box to target
			transform.position = targetPosition;
			return true;
		}
		else
		{
			// if cannot miive
			return false;
		}
	}

	// judge whether can move
	private bool IsValidMove ( Vector2 targetPosition )
	{
		Collider2D obstacleHit = Physics2D.OverlapCircle ( targetPosition , 0.25f , obstacleLayer );
		Collider2D boxHit = Physics2D.OverlapCircle ( targetPosition , 0.25f , boxLayer );

		if ( obstacleHit == null && boxHit == null )
		{
			return true;
		}

		return false; 
	}
}
