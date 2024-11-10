using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box: MonoBehaviour
{
    public bool CanMoveToDir(Vector2 dir,int speed)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3)dir*0.5f, dir, 0.5f);
        
        if (!hit)
        {
            this.transform.localPosition += new Vector3(dir.x*speed, dir.y*speed, 0);
            return true;
        }

        return false;
    }
}
