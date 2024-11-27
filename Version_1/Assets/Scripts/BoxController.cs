using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
	private Stack<BoxState> stateStack = new Stack<BoxState> ( );
	private BoxState initialState; // 保存初始状态
	public const float gridHalfSize = 0.25f;

	[Header("LayerMask")]
	public LayerMask boxLayer;
	public LayerMask obstacleLayer;

	private void Start()
	{
		initialState = new BoxState(transform.position);//储存箱子初始位置
	}
	public void SaveState ( )
	{
		stateStack.Push ( new BoxState ( transform.position ) );
	}

	public void StartRewind ( )
	{
		BoxState lastState = stateStack.Pop ( );
		transform.position = lastState.position;
	}

	public void RestoreFirstState()
	{
		//回归最初状态
		if (initialState != null)
		{
			transform.position = initialState.position;
		}

	}
	public void Destroy_state()
	{
		//Clear all states
		stateStack.Clear ( );
	}

	/// <summary>
	/// 通过检测相邻最远箱子是否接触障碍物决定能否移动
	/// </summary>
	public bool Push_Box ( Vector2 direction, Vector2 playerPosition ) 
	{
		bool isPush = true;
		Vector2 rayStartPosition = RoundToGridCenter ( playerPosition ) + direction * 0.5f;
		float rayLength = gridHalfSize * 4f + 0.5f;

		RaycastHit2D hitBox = Physics2D.Raycast ( rayStartPosition , direction , 0.5f , boxLayer );
		RaycastHit2D hitObstacle = Physics2D.Raycast ( rayStartPosition , direction , rayLength , obstacleLayer );

		if ( hitBox )
		{
			List<BoxController> boxChain = new List<BoxController> ( );
			BoxController currentBox = this;
			boxChain.Add ( currentBox );

			while ( currentBox != null )
			{
				BoxController nextBox = currentBox.FindNextBox ( direction );
				if ( nextBox != null )
				{
					boxChain.Add ( nextBox );
					currentBox = nextBox; 
				}
				else
				{
					break; 
				}
			}

			// 检查最远箱子前方是否有障碍物
			BoxController farthestBox = boxChain [ boxChain.Count - 1 ];
			Debug.Log ( boxChain.Count );
			if ( CanMove ( farthestBox , direction ) )
			{
				MoveBoxes ( direction , boxChain );
			}
			else
			{
				isPush = false; 
			}
		}
		else if ( hitObstacle )
		{
			isPush = false;
		}

		return isPush;
	}

	private BoxController FindNextBox ( Vector2 direction )
	{
		Vector2 rayStartPosition = RoundToGridCenter ( transform.position ) + direction * 0.5f;
		RaycastHit2D hitBox = Physics2D.Raycast ( rayStartPosition , direction , 0.5f , boxLayer );
		if ( hitBox )
		{
			return hitBox.collider.GetComponent<BoxController> ( );
		}
		return null;
	}

	private bool CanMove ( BoxController farthestBox , Vector2 direction )
	{
		Vector2 rayStartPosition = RoundToGridCenter ( farthestBox.transform.position ) + direction * 0.5f;
		float rayLength = gridHalfSize * 4f;
		Debug.DrawRay ( rayStartPosition , direction * rayLength , Color.red , 1f );
		RaycastHit2D hitObstacle = Physics2D.Raycast ( rayStartPosition , direction , rayLength , obstacleLayer );
		return hitObstacle.collider == null;
	}

	private void MoveBoxes ( Vector2 direction , List<BoxController> boxChain )
	{
		foreach ( BoxController box in boxChain )
		{
			Vector2 newPosition = RoundToGridCenter ( ( Vector2 ) box.transform.position + direction * gridHalfSize * 4 );
			box.transform.position = newPosition;
		}
	}

	private Vector2 RoundToGridCenter(Vector2 position)
	{
		return new Vector2(
		Mathf.Round(position.x / 0.5f) * 0.5f,
		Mathf.Round(position.y / 0.5f) * 0.5f
		);
	}
}

public class BoxState
{
	public Vector2 position;

	public BoxState ( Vector2 pos )
	{
		position = pos;
	}
}
