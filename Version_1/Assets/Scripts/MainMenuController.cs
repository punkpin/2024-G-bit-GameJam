using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
	[Header ( "Button" )]
	public Button startButton;
	public Button quitButton;

	[Header ( "CatBox" )]
	public GameObject ca1;
	public GameObject ca2;

	private Button selectedButton;

	public void Start ( )
	{
		selectedButton = startButton;
		ca1.gameObject.SetActive ( true );
		ca2.gameObject.SetActive ( false );
	}

	public void Update ( )
	{
		if ( Input.GetKeyDown ( KeyCode.UpArrow ) || Input.GetKeyDown ( KeyCode.W ) )
		{
			selectedButton = startButton;
			ca1.gameObject.SetActive ( true );
			ca2.gameObject.SetActive ( false );
		}

		if ( Input.GetKeyDown ( KeyCode.DownArrow ) || Input.GetKeyDown ( KeyCode.S ) )
		{
			selectedButton = quitButton;
			ca1.gameObject.SetActive ( false );
			ca2.gameObject.SetActive ( true );
		}

		if ( Input.GetKeyDown ( KeyCode.Space ) || Input.GetKeyDown ( KeyCode.Return ) )
		{
			if ( selectedButton != null )
			{
				selectedButton.onClick.Invoke ( );
			}
		}
	}

	/*
	public void OnPointerEnter ( PointerEventData eventData )
	{
		if ( eventData.pointerCurrentRaycast.gameObject == startButton )
		{
			Debug.Log ( "Mouse entered startButton" );
			ca1.gameObject.SetActive ( true );
		}
		else if ( eventData.pointerCurrentRaycast.gameObject == quitButton )
		{
			Debug.Log ( "Mouse entered quitButton" );
			ca2.gameObject.SetActive ( true );
		}
	}

	public void OnPointerExit ( PointerEventData eventData )
	{
		if ( eventData.pointerCurrentRaycast.gameObject == startButton )
		{
			Debug.Log ( "Mouse exited startButton" );
			ca1.gameObject.SetActive ( false );
		}
		else if ( eventData.pointerCurrentRaycast.gameObject == quitButton )
		{
			Debug.Log ( "Mouse exited quitButton" );
			ca2.gameObject.SetActive ( false );
		}
	}*/
}
