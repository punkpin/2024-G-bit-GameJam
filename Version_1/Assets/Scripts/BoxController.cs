using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
	[Header ( "LayerMask" )]
	public LayerMask obstacleLayer; 
	public LayerMask boxLayer;
	private Stack<BoxState> stateStack = new Stack<BoxState>();//实现栈的功能
	[Header("Script")]
	private PlayerController playerController;

	private bool IsLocked_Player;

    private void Start()
    {
        playerController = GameObject.Find("Character").GetComponent<PlayerController>();
    }

    private void Update()
    {
		IsLocked_Player = playerController.isLockState;//状态统一

        if (!IsLocked_Player &&Input.GetKeyDown(KeyCode.Z))
		{
			StartRewind();
        }
    }
    public bool TryMove ( Vector2 direction )
	{
		// Find obstacle on the direction
		Vector2 targetPosition = ( Vector2 ) transform.position + direction;
        // Raycast judgemnet to find ovelap box
        if ( IsValidMove ( targetPosition ) )
		{
            // move box to target
            SaveState();
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
    public void SaveState()
    {
        // 保存当前状态到栈
        stateStack.Push(new BoxState(transform.position));
    }

    public void StartRewind()
    {
        if (stateStack.Count > 0)
        {
            BoxState lastState = stateStack.Pop();
            transform.position = lastState.position;
        }
    }

}
public class BoxState
{
    public Vector2 position;

    public BoxState(Vector2 pos)
    {
        position = pos;
    }
}
