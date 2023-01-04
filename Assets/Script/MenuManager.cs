using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject mainPanel;
    public GameObject joinPanel;

    [Header ("PlayerState")]
    public Image[] playerState;
    [Header("PlayerControllerIcon")]
    public Sprite[] controllerIcons;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        foreach (Image state in playerState)
        {
            if (state != null)
                state.enabled = false;
        }           
    }

    public void Play()
    {
        mainPanel.SetActive(false);
        joinPanel.SetActive(true);
        VerifyDevice();
        GAMEMANAGER.Instance.OpenJoinSession();
    }

    public void Back()
    {
        mainPanel.SetActive(true);
        joinPanel.SetActive(false);
        GAMEMANAGER.Instance.CloseJoinSession();
    }

    public void ChangePlayerState(int _playerIndex, bool _connected, string _deviceName = null)
    {
        if (_connected)
        {
            if (playerState[_playerIndex] != null && _deviceName != null)
            {
                if(_deviceName.Contains("Keyboard"))
                    playerState[_playerIndex].sprite = controllerIcons[0];
                else if (_deviceName.Contains("GamePad"))
                    playerState[_playerIndex].sprite = controllerIcons[1];
                else
                    playerState[_playerIndex].sprite = null;

                playerState[_playerIndex].enabled = true;
            }
        }
        else
        {
            playerState[_playerIndex].enabled = false;
        }
    }

    public void LaunchParty()
    {
        GAMEMANAGER.Instance.StartParty();
    }

    public void RemovePlayer(int _player)
    {
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();

        for(int i = 0; i < playerInputs.Length; i++)
        {
            print(playerInputs[i].playerIndex);
            if (playerInputs[i].playerIndex == _player)
            {
                Player player = playerInputs[i].GetComponentInChildren<Player>();
                if (player.connected)
                {
                    ChangePlayerState(playerInputs[i].playerIndex, false);
                    Destroy(playerInputs[i].transform.gameObject);
                    break;
                }
            }
        }
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
                    ChangePlayerState(playerInput.playerIndex, true, playerInput.currentControlScheme);
                    break;
                }
            }
        }
    }
}
