using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private Vector2 currentPosition; // dynamic variable to record position
	private Collider2D initialBox; 
	private Collider2D boxHit;

	private bool isAttach;

	[Header("Value")]
	[SerializeField] public float Move_cushioning;
    public const float gridHalfSize = 0.25f;

	[Header ( "LayerMask" )]
	public LayerMask boxLayer;
	public LayerMask obstacleLayer;


	public void Start ( )
	{
		currentPosition = RoundToGridCenter ( transform.position );
		initialBox = Physics2D.OverlapPoint ( currentPosition , boxLayer );

    }

	public void Update ( )
	{
		HandleMovement ( );
		HandleAttachment ( );
	}

	//MOVEMENT
	private void HandleMovement ( )
	{
		if ( isAttach )
		{
			if ( Input.GetKeyDown ( KeyCode.W ) || Input.GetKeyDown ( KeyCode.UpArrow ) )
				MoveTogether ( Vector2.up );
			else if ( Input.GetKeyDown ( KeyCode.A ) || Input.GetKeyDown ( KeyCode.LeftArrow ) )
				MoveTogether ( Vector2.left );
			else if ( Input.GetKeyDown ( KeyCode.S ) || Input.GetKeyDown ( KeyCode.DownArrow ) )
				MoveTogether ( Vector2.down );
			else if ( Input.GetKeyDown ( KeyCode.D ) || Input.GetKeyDown ( KeyCode.RightArrow ) )
				MoveTogether ( Vector2.right );
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
				MoveToNearestBox(Vector2.up);
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
				MoveToNearestBox(Vector2.left);
			else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
				MoveToNearestBox(Vector2.down);
			else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                MoveToNearestBox ( Vector2.right );
                

        }
	}

	private void MoveTogether ( Vector2 direction )
	{
		Vector2 targetPosition = RoundToGridCenter ( ( Vector2 ) transform.position + direction * gridHalfSize * 4 );
		if ( CanMove ( direction, targetPosition ) )
		{
			transform.position = targetPosition;
			initialBox.transform.position = targetPosition;

			Debug.Log ( $"Moved together to {targetPosition}" );
		}
		else
		{
			Debug.Log ( "Cannot move: Target position blocked." );
		}
	}

	private bool CanMove ( Vector2 direction, Vector2 targetPosition )
	{
		Vector2 rayStartPosition = RoundToGridCenter ( transform.position ) + direction * 0.5f;
		float rayLength = gridHalfSize * 4f;
		// obstacle?
		RaycastHit2D hitObstacle = Physics2D.Raycast ( rayStartPosition , direction , rayLength , obstacleLayer );
		//box?    
		RaycastHit2D hitBox = Physics2D.Raycast ( rayStartPosition , direction , rayLength , boxLayer );
		Debug.DrawRay ( rayStartPosition , direction * gridHalfSize * 4 , Color.blue , 0.5f );

		if ( hitObstacle.collider != null || hitBox.collider != null )
		{
			return false;
		}
		return true;
	}

	//ATTACH
	private void HandleAttachment ( )
		//IT'S AN EXAMPLE USING TO CHAGE THE STAGE OF BOX=>CHANGEABLE
	{
		if ( Input.GetKeyDown ( KeyCode.Space ) )
		{
			isAttach = !isAttach;
			Debug.Log ( isAttach );
			if ( isAttach )
			{
				AttachToBox ( );
			}
			else
			{
				DetachFromBox ( );
			}
		}
	}

	private void AttachToBox ( )
	{
		if ( initialBox != null )
		{
			//change stage

			Debug.Log ( "Attached to box." );
		}
	}

	private void DetachFromBox ( )
	{
		if ( initialBox != null )
		{
			//change stage

			Debug.Log ( "Detached from box." );
		}
	}

	//LOGIC OF MOVEMENT
	private void MoveToNearestBox ( Vector2 direction )
	{
		float rayLength = 1f;
		float maxRayLength = 20f; 
		float rayIncrement = 1f; 

		Collider2D closestBox = null;
		Vector2? obstaclePosition = null;

		// start raycastJudgement beyond the initial box
		Vector2 boxCenter = RoundToGridCenter ( transform.position ); // position of player current box
		Vector2 rayStartPosition = RoundToGridCenter(transform.position) + direction * 0.5f;

		while ( rayLength <= maxRayLength)
		{
			// raycast judgement by adding the edge length of box
			Debug.Log ( rayLength );
			Debug.DrawRay ( rayStartPosition , direction * rayLength , Color.red , 0.5f );

			RaycastHit2D hit = Physics2D.Raycast ( rayStartPosition , direction , rayLength , boxLayer );
			RaycastHit2D obstacleHit = Physics2D.Raycast ( rayStartPosition , direction , rayLength , obstacleLayer );

			

            if ( hit.collider != null )
			{
				closestBox = hit.collider;
				Debug.Log ( closestBox.transform.position );
				break; // find the nearest box along the way, if exists, break the loop
			}

			if ( obstacleHit.collider != null && !obstaclePosition.HasValue )
			{
				Debug.Log ( $"Hit obstacle at: {obstacleHit.point}" );
				obstaclePosition = RoundToGridCenter ( obstacleHit.point - direction * 0.5f );
				break;
			}
            
            rayLength += rayIncrement;

		}

		if ( closestBox != null )
		{
            StartCoroutine(Move_Wait(direction, closestBox));

           
		}
		else if ( obstaclePosition.HasValue )
		{
			StartCoroutine ( MoveToObstacleAndBack ( obstaclePosition.Value ) );
		}
	}

	private void MoveToBox ( Collider2D box )
	{
		currentPosition = RoundToGridCenter ( box.transform.position );
		transform.position = currentPosition;
		initialBox = box; // reset initialbox
		Debug.Log ( $"Moved to box at {currentPosition}" );
	}

	private IEnumerator MoveToObstacleAndBack ( Vector2 obstaclePosition )
	{
		Vector2 originalPosition = transform.position;

		// move to the previous block of the obstacle
		transform.position = obstaclePosition;
		Debug.Log ( $"Moved to obstacle at {obstaclePosition}" );
		yield return new WaitForSeconds ( 0.5f );

		// back to the box
		transform.position = originalPosition;
		Debug.Log ( $"Returned to box at {originalPosition}" );
	}

	private Vector2 RoundToGridCenter ( Vector2 position )
	{
		return new Vector2 
		(
		    Mathf.Round ( position.x / 0.5f ) * 0.5f ,
		    Mathf.Round ( position.y / 0.5f ) * 0.5f
		);
	}
    private IEnumerator Move_Wait(Vector3 direction, Collider2D closestBox)
	{
		while (true)
		{

			this.transform.position += direction*0.01f;

            yield return new WaitForSeconds(0.01f);
			float Ray = (closestBox.transform.position -this.transform.position).magnitude;
            if (Ray <= 0.01f)
			{
                MoveToBox ( closestBox );
                break;
			}
        }
    }
}

