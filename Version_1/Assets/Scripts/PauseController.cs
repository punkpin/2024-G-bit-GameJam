using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
        public GameObject pauseCanvas;
	public GameObject MainMenuCanvas;
	public GameObject levelSelection;
        public PlayerController playerController;

        void Update()
        {
                if ( Input.GetKeyUp ( KeyCode.Q ) )
                {
                        Time.timeScale = 0f;
                        pauseCanvas.SetActive ( true );
                }
        }

        public void ContinuousGame ( )
        {
                Time.timeScale = 1f;
                ExitPauseCanvas ( );
        }

        public void BackToMenu ( )
	{
                playerController.Exit ( );
        }

        public void BackToScreen ( )
        {
                Application.Quit ( );
        }

        private void ExitPauseCanvas ( )
        {
                pauseCanvas.SetActive ( false );
        }

        public void MainStartGame ( )
        {
                levelSelection.SetActive ( true );
		MainMenuCanvas.gameObject.SetActive ( false );
	}
}
