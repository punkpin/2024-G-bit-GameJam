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
		//保存状态
		stateStack.Push ( new BoxState ( transform.position ) );
	}

	public void StartRewind ( )
	{
		if ( stateStack.Count > 0 )
		{
			BoxState lastState = stateStack.Pop ( );
			transform.position = lastState.position;
		}
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
		stateStack.Clear();
    }

	public bool Push_Box(Vector2 direction)//推箱子传递
	{
		bool IsPush = true;
        Vector2 rayStartPosition = RoundToGridCenter(transform.position) + direction * 0.5f;
        float rayLength = gridHalfSize * 4f;
		RaycastHit2D hitBox = Physics2D.Raycast(rayStartPosition, direction, 0.5f, boxLayer);
        RaycastHit2D hitObstacle = Physics2D.Raycast(rayStartPosition, direction, rayLength, obstacleLayer);
        if (hitBox)
		{
            IsPush = hitBox.collider.GetComponent<BoxController>().Push_Box(direction);
		}
		if (!hitObstacle)
		{
            Vector2 targetPosition = RoundToGridCenter((Vector2)transform.position + direction * gridHalfSize * 4);
            transform.position = targetPosition;
            IsPush = true;
		}
		else
		{
            IsPush = false;
        }
		return IsPush;
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
