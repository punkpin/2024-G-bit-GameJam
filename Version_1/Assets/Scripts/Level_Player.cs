using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Level_Player : MonoBehaviour
{
    public const float gridHalfSize = 0.25f;
    public LayerMask boxLayer;
    public LayerMask obstacleLayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            MoveTogether(Vector2.up);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            MoveTogether(Vector2.left);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            MoveTogether(Vector2.down);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            MoveTogether(Vector2.right);
    }
    private void MoveTogether(Vector2 direction)
    {
        
        Vector2 targetPosition = RoundToGridCenter((Vector2)transform.position + direction * gridHalfSize * 4);
        if (CanMove(direction, targetPosition))
        {
            transform.position = targetPosition;
        }
        else
        {
            Debug.Log("Cannot move: Target position blocked.");
        }
    }

    private Vector2 RoundToGridCenter(Vector2 position)
    {
        return new Vector2(
            Mathf.Round(position.x / 0.5f) * 0.5f,
            Mathf.Round(position.y / 0.5f) * 0.5f
        );
    }
    private bool CanMove(Vector2 direction, Vector2 targetPosition)
    {
        bool isPush = true;
        Vector2 rayStartPosition = RoundToGridCenter(transform.position) + direction * 0.5f;
        float rayLength = gridHalfSize * 4f;

        RaycastHit2D hitObstacle = Physics2D.Raycast(rayStartPosition, direction, rayLength, obstacleLayer);
        RaycastHit2D hitBox = Physics2D.Raycast(rayStartPosition, direction, rayLength, boxLayer);

        if (hitBox)
        {
            isPush = hitBox.collider.GetComponent<BoxController>().Push_Box(direction, transform.position);
        }

        //Debug.DrawRay ( rayStartPosition , direction * gridHalfSize * 4 , Color.green , 0.5f );

        return hitObstacle.collider == null && isPush;
    }

}
