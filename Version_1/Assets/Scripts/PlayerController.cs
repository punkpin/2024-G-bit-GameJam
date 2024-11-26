using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private Vector2 currentPosition;
	private Collider2D initialBox;
	private Stack<PlayerState> stateStack = new Stack<PlayerState> ( );
    private PlayerState initialState; // 保存初始状态
    private bool isAttach;
	private List<BoxController> allBoxes = new List<BoxController> ( );

	[Header ( "Value" )]
	[SerializeField] public float Move_cushioning;
	public const float gridHalfSize = 0.25f;

	[Header ( "LayerMask" )]
	public LayerMask boxLayer;
	public LayerMask obstacleLayer;

	public void Start ( )
	{
        initialState = new PlayerState(transform.position);//储存玩家初始位置
        currentPosition = RoundToGridCenter ( transform.position );
		initialBox = Physics2D.OverlapPoint ( currentPosition , boxLayer );

		// 初始化所有箱子
		BoxController [ ] boxes = FindObjectsOfType<BoxController> ( );
		allBoxes = new List<BoxController> ( boxes );

		Debug.Log ( $"Number of boxes found: {allBoxes.Count}" );
		foreach ( BoxController box in allBoxes )
		{
			Debug.Log ( $"Box: {box.name}" );
		}
	}

	public void Update ( )
	{
		HandleMovement ( );
		HandleAttachment ( );

		if ( Input.GetKeyDown ( KeyCode.Z ) )
		{
			StartRewind ( );
		}
		if(Input.GetKeyDown(KeyCode.R))
		{
			RestoreFirstState();
            foreach (BoxController boxes in allBoxes)
            {
				boxes.RestoreFirstState();
            }
        }	
	}

	// 移动处理
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
			if ( Input.GetKeyDown ( KeyCode.W ) || Input.GetKeyDown ( KeyCode.UpArrow ) )
				MoveToNearestBox ( Vector2.up );
			else if ( Input.GetKeyDown ( KeyCode.A ) || Input.GetKeyDown ( KeyCode.LeftArrow ) )
				MoveToNearestBox ( Vector2.left );
			else if ( Input.GetKeyDown ( KeyCode.S ) || Input.GetKeyDown ( KeyCode.DownArrow ) )
				MoveToNearestBox ( Vector2.down );
			else if ( Input.GetKeyDown ( KeyCode.D ) || Input.GetKeyDown ( KeyCode.RightArrow ) )
				MoveToNearestBox ( Vector2.right );
		}
	}

	private void MoveTogether ( Vector2 direction )
	{
		Vector2 targetPosition = RoundToGridCenter ( ( Vector2 ) transform.position + direction * gridHalfSize * 4 );
		if ( CanMove ( direction , targetPosition ) )
		{
			SaveState ( );
			transform.position = targetPosition;

			SaveAllBoxState ( );
			initialBox.transform.position = targetPosition;

			Debug.Log ( $"Moved together to {targetPosition}" );
		}
		else
		{
			Debug.Log ( "Cannot move: Target position blocked." );
		}
	}

	private void SaveAllBoxState ( )
	{
		foreach ( BoxController boxes in allBoxes )
		{
			boxes.SaveState ( );
			Debug.Log ( $"Save box {boxes} current position" );
		}
	}

	private bool CanMove ( Vector2 direction , Vector2 targetPosition )
	{
		Vector2 rayStartPosition = RoundToGridCenter ( transform.position ) + direction * 0.5f;
		float rayLength = gridHalfSize * 4f;

		RaycastHit2D hitObstacle = Physics2D.Raycast ( rayStartPosition , direction , rayLength , obstacleLayer );
		RaycastHit2D hitBox = Physics2D.Raycast ( rayStartPosition , direction , rayLength , boxLayer );

		Debug.DrawRay ( rayStartPosition , direction * gridHalfSize * 4 , Color.blue , 0.5f );

		return hitObstacle.collider == null && hitBox.collider == null;
	}

	private void HandleAttachment ( )
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
		Debug.Log ( "Attached to box." );
	}

	private void DetachFromBox ( )
	{
		Debug.Log ( "Detached from box." );
	}

	private void MoveToNearestBox ( Vector2 direction )
	{
		Vector2 rayStartPosition = RoundToGridCenter ( transform.position ) + direction * 0.5f;
		RaycastHit2D hit = Physics2D.Raycast ( rayStartPosition , direction , 20f , boxLayer );
		SaveAllBoxState ( );

		if ( hit.collider != null )
		{
			StartCoroutine ( Move_Wait ( direction , hit.collider ) );
		}
	}

	private IEnumerator Move_Wait ( Vector3 direction , Collider2D closestBox )
	{
		SaveState ( );

		while ( true )
		{
			transform.position += direction * 0.5f;

			yield return new WaitForSeconds ( 0.01f );
			float distance = ( closestBox.transform.position - transform.position ).magnitude;
			if ( distance <= 0.01f )
			{
				currentPosition = RoundToGridCenter ( closestBox.transform.position );
				transform.position = currentPosition;
				initialBox = closestBox;
				Debug.Log ( $"Moved to box at {currentPosition}" );
				break;
			}
		}
	}

	private void SaveState ( )
	{
		stateStack.Push ( new PlayerState ( transform.position ) );
	}

	private void StartRewind ( )
	{
		if ( stateStack.Count > 0 )
		{
			PlayerState lastState = stateStack.Pop ( );
			transform.position = lastState.position;
		}

		// 所有箱子回退
		foreach ( BoxController box in allBoxes )
		{
			box.StartRewind ( );
		}
	}

	private Vector2 RoundToGridCenter ( Vector2 position )
	{
		return new Vector2 (
		    Mathf.Round ( position.x / 0.5f ) * 0.5f ,
		    Mathf.Round ( position.y / 0.5f ) * 0.5f
		);
	}

    //遇到了未知的问题，导致无法直接回到栈的第一步，改用创建一个 PlayerState来保存
    //void RestoreFirstState()
    //{
    //    if (stateStack.Count > 0)
    //    {
    //        PlayerState firstState = stateStack.Peek(); // 查看最早保存的状态
    //        stateStack.Clear(); // 清空所有状态，避免重复
    //        transform.position = firstState.position;
    //    }
    //}
    void RestoreFirstState()
	{
        if (initialState != null)
        {
			transform.position = initialState.position;
        }

    }
}

[System.Serializable]
public class PlayerState
{
	public Vector2 position;
	public PlayerState ( Vector2 pos )
	{
		position = pos;
	}
}
