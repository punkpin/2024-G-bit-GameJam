using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
	private Stack<BoxState> stateStack = new Stack<BoxState> ( );
    private BoxState initialState; // 保存初始状态

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
		if ( stateStack.Count > 0 )
		{
			BoxState lastState = stateStack.Pop ( );
			transform.position = lastState.position;
		}
	}

    public void RestoreFirstState()
	{
        if (initialState != null)
        {
            transform.position = initialState.position;
        }

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
