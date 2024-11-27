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

	[Header("Ui")]
	[SerializeField] public GameObject Ui_Text1;//储存Ui文字栏1
	[SerializeField] public GameObject Ui_Text2;//储存Ui文字栏2
	[SerializeField] public GameObject Canves;//储存Canves
	[SerializeField] public float Destroy_Timer;//设置销毁时间


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

		//按Z回到上一步
		if ( Input.GetKeyDown ( KeyCode.Z ) )
		{
			//设置判断，为空时不能提示
			if (stateStack.Count > 0)
			{
				
				GameObject text1_prefabs = Instantiate(Ui_Text1, Canves.transform);
				Destroy(text1_prefabs, Destroy_Timer);//设置多少s后销毁
			}
            StartRewind ( );
        }
		//按R重新开始
		if(Input.GetKeyDown(KeyCode.R))
		{
            GameObject text2_prefabs = Instantiate(Ui_Text2, Canves.transform);
            Destroy(text2_prefabs, Destroy_Timer);//设置多少s后销毁

            RestoreFirstState();
            stateStack.Clear();//清空储存的所有栈

            foreach (BoxController boxes in allBoxes)
            {
				boxes.RestoreFirstState();
				boxes.Destroy_state();
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
		bool IsPush = true;
		Vector2 rayStartPosition = RoundToGridCenter ( transform.position ) + direction * 0.5f;
		float rayLength = gridHalfSize * 4f;

		RaycastHit2D hitObstacle = Physics2D.Raycast ( rayStartPosition , direction , rayLength , obstacleLayer );
		RaycastHit2D hitBox = Physics2D.Raycast ( rayStartPosition , direction , rayLength , boxLayer );

		//这里可以看着改
		if (hitBox)
		{
			IsPush = hitBox.collider.GetComponent<BoxController>().Push_Box(direction);
		}

		Debug.DrawRay ( rayStartPosition , direction * gridHalfSize * 4 , Color.blue , 0.5f );

		return hitObstacle.collider == null&& IsPush;
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
