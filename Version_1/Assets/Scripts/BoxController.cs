using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
	private Stack<BoxState> stateStack = new Stack<BoxState> ( );
	private BoxState initialState; // 保存初始状态
	public const float gridHalfSize = 0.25f;
	private PlayerController playerController;
	private bool isActive;

	[Header ( "LayerMask" )]
	public LayerMask boxLayer;
	public LayerMask obstacleLayer;
	public LayerMask holeLayer;
	[Header ( "Value" )]
	[SerializeField] public bool Can_Possessed;//是否允许附身
	[Header ( "Level" )]
	[SerializeField] private GameObject Level;
	[SerializeField] private GameObject Player;

	private void Start ( )
	{
		Level = this.transform.parent.gameObject;
		Player = Level.transform.GetChild ( 0 ).gameObject;//通过关卡物体找到玩家

		initialState = new BoxState ( transform.position , true );//储存箱子初始位置
		playerController = FindObjectOfType<PlayerController> ( );
	}
	public void SaveState ( )
	{
		stateStack.Push ( new BoxState ( transform.position , gameObject.activeSelf ) );
	}

	public void StartRewind ( )
	{
		BoxState lastState = stateStack.Pop ( );
		transform.position = lastState.position;
		gameObject.SetActive ( lastState.isActive );
	}

	public void RestoreFirstState ( )
	{
		if ( initialState != null )
		{
			transform.position = initialState.position;
			isActive = true;
		}

	}
	public void Destroy_state ( )
	{
		//Clear all states
		stateStack.Clear ( );
	}

	/// <summary>
	/// 通过检测相邻最远箱子是否接触障碍物决定能否移动
	/// </summary>
	public bool Push_Box ( Vector2 direction , Vector2 playerPosition )
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
			BoxController farthestBox = boxChain [ boxChain.Count - 1 ];


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

	private bool IsInHole ( Vector2 position )
	{
		Collider2D hitHole = Physics2D.OverlapPoint ( position , holeLayer );
		Debug.Log ( "hitholejudgement" );
		return hitHole != null;
	}

	private void MoveBoxes ( Vector2 direction , List<BoxController> boxChain )
	{
		foreach ( BoxController box in boxChain )
		{
			// 检查箱子是否会掉进坑
			if ( IsInHole ( box.transform.position ) )
			{
				// 如果箱子掉入坑，隐藏箱子
				Debug.Log ( $"destroy{box}" );
				box.gameObject.SetActive ( false );
			}
			else
			{
				Debug.Log ( "NotInHole" );
			}
			Vector2 newPosition = RoundToGridCenter ( ( Vector2 ) box.transform.position + direction * gridHalfSize * 4 );
			box.transform.position = newPosition;
		}
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

	private Vector2 RoundToGridCenter ( Vector2 position )
	{
		return new Vector2 (
		Mathf.Round ( position.x / 0.5f ) * 0.5f ,
		Mathf.Round ( position.y / 0.5f ) * 0.5f
		);
	}
	private void OnTriggerEnter2D ( Collider2D collision )
	{
		if ( collision.tag == "Target" )
		{
			Debug.Log ( "+1" );
			Player.GetComponent<PlayerController> ( ).Local_Win_Number++;

		}

	}
	private void OnTriggerExit2D ( Collider2D collision )
	{
		if ( collision.tag == "Target" )
		{
			Debug.Log ( "-1" );
			Player.GetComponent<PlayerController> ( ).Local_Win_Number--;
		}
	}
}

public class BoxState
{
	public Vector2 position;
	public bool isActive;

	public BoxState ( Vector2 pos , bool active )
	{
		position = pos;
		isActive = active;
	}
}
