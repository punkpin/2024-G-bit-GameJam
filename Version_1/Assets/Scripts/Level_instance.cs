using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_instance : MonoBehaviour
{
	[Header ( "level" )]
	[SerializeField] public GameObject level;
	[SerializeField] public string NextLevel;
	[SerializeField] public string NextLevel2;
	[SerializeField] public bool Islocked;
	[SerializeField] private Level_instance Next_level_Instance;
	[SerializeField] private Level_instance Next_level_Instance2;
	[SerializeField] public bool IsWin;
	[SerializeField] public bool IsExit;
	// Start is called before the first frame update
	void Start ( )
	{
		if ( NextLevel != "None_" )
		{
			Next_level_Instance = GameObject.Find ( NextLevel ).GetComponent<Level_instance> ( );
		}
		if ( NextLevel2 != "None_" )
		{
			Next_level_Instance2 = GameObject.Find ( NextLevel2 ).GetComponent<Level_instance> ( );
		}
	}

	// Update is called once per frame
	void Update ( )
	{
		if ( Islocked )
		{
			this.gameObject.layer = 3;

		}
		else
		{
			this.gameObject.layer = 0;
		}

		if ( IsWin )
		{
			if ( NextLevel != "None_" )
			{
				Next_level_Instance.Islocked = false;
			}
			if ( NextLevel2 != "None_" )
			{
				Next_level_Instance2.Islocked = false;
			}

			if (IsExit == false)
			{
                IsExit = true;
                Next_level_Instance.Enter_Level();
            }

			IsWin = false;
		}
		
		
	}

	public void OnTriggerEnter2D ( Collider2D collision )
	{
		if ( collision.tag == "Player" )
		{
			Enter_Level();
        }
	}
	public void Enter_Level()
	{
        level.SetActive(true);
        level.transform.GetChild(0).GetComponent<PlayerController>().Level_ = this.gameObject;
        this.transform.parent.gameObject.SetActive(false);
    }

}
