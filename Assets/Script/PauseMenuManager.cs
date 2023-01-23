using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pausePanel;

    bool isSetPause = false;

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (ctx.started && GAMEMANAGER.Instance.gameState == GAMEMANAGER.GameState.Game)
        {
            GAMEMANAGER.Instance.gameState = GAMEMANAGER.GameState.Pause;
        }
        else if (ctx.canceled && GAMEMANAGER.Instance.gameState == GAMEMANAGER.GameState.Pause)
        {
            GAMEMANAGER.Instance.gameState = GAMEMANAGER.GameState.Game;
        }
    }

    private void Update()
    {
        if(GAMEMANAGER.Instance.gameState == GAMEMANAGER.GameState.Pause && !isSetPause)
        {
            SetPauseMenu(true);
        }
        else if (GAMEMANAGER.Instance.gameState == GAMEMANAGER.GameState.Game && isSetPause)
        {
            SetPauseMenu(false);
        }
    }

    private void SetPauseMenu(bool _active)
    {
        if (_active == true)
        {
            isSetPause = true;
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        }
        else
        {
            isSetPause = false;
            Time.timeScale = 1;
            pausePanel.SetActive(false);
        }
    }
}
