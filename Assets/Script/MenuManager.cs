using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject mainPanel;
    public GameObject joinPanel;

    [Header ("PlayerState")]
    public Image[] playerState;
    public GameObject[] playerModels;
    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        foreach (Image state in playerState)
        {
            if(state != null)
                state.color = Color.red;
        }           
    }

    public void Play()
    {
        mainPanel.SetActive(false);
        joinPanel.SetActive(true);        
        GAMEMANAGER.Instance.OpenJoinSession();
        VerifyDevice();
    }

    public void Back()
    {
        mainPanel.SetActive(true);
        joinPanel.SetActive(false);
        GAMEMANAGER.Instance.CloseJoinSession();
    }

    public void ChangePlayerState(int _playerIndex, bool _connected, Color _color)
    {
        if (_connected)
        {
            if (GAMEMANAGER.Instance.gameState == GAMEMANAGER.GameState.JoinSession)
            {
                playerModels[_playerIndex].GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_BodyColor", _color);
                playerModels[_playerIndex].gameObject.SetActive(true);
            }                
        }
        else
        {
            if (GAMEMANAGER.Instance.gameState == GAMEMANAGER.GameState.JoinSession)
                playerModels[_playerIndex].gameObject.SetActive(false);
        }
    }

    public void LaunchParty()
    {
        GAMEMANAGER.Instance.StartParty();
    }

    private void VerifyDevice()
    {
        int pCount = GAMEMANAGER.Instance.inputManager.playerCount;
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (PlayerInput playerInput in playerInputs)
        {
            Player player = playerInput.GetComponentInChildren<Player>();
            if (!player.connected)
                Destroy(playerInput.transform.gameObject);
        }
            for (int i = 0; i < pCount; i++) 
        {
            foreach (PlayerInput playerInput in playerInputs)
            {               
                if (playerInput != null && playerInput.playerIndex == i)
                {
                    ChangePlayerState(playerInput.playerIndex, true, playerInput.GetComponentInChildren<Player>().color);
                    break;
                }
            }
        }
    }
}
