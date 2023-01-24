using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject mainPanel;
    public GameObject joinPanel;
    public GameObject controlsPanel;

    public GameObject playButton;
    public GameObject backButtonControls;
    public GameObject playButtonLobby;

    public AudioSource buttonSelect;

    [Header ("Players")]
    public Image[] playerDevice;
    public GameObject[] playerModels;
    public GameObject[] playerRemoveBtns;
    public Sprite[] controllerIcons;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        foreach (Image device in playerDevice)
        {
            if (device != null)
                device.enabled = false;
        }
        EventSystem.current.SetSelectedGameObject(playButton);
    }

    public void Play()
    {
        mainPanel.SetActive(false);
        joinPanel.SetActive(true);        
        GAMEMANAGER.Instance.OpenJoinSession();
        EventSystem.current.SetSelectedGameObject(playButtonLobby);
        VerifyDevice();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Controls()
    {
        mainPanel.SetActive(false);      
        controlsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(backButtonControls);
    }
    public void Back()
    {
        mainPanel.SetActive(true);
        controlsPanel.SetActive(false);
        joinPanel.SetActive(false);
        GAMEMANAGER.Instance.CloseJoinSession();
        EventSystem.current.SetSelectedGameObject(playButton);
    }

    public void ChangePlayerState(int _playerIndex, bool _connected, Color _color, string _deviceName = null)
    {
        if (_connected)
        {
			if (GAMEMANAGER.Instance.gameState == GAMEMANAGER.GameState.JoinSession)
            {
                // Model
                playerModels[_playerIndex].GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_BodyColor", _color);
                playerModels[_playerIndex].gameObject.SetActive(true);

                // Device Sprite
                if (_deviceName.Contains("Keyboard"))
                    playerDevice[_playerIndex].sprite = controllerIcons[0];
                else if (_deviceName.Contains("GamePad"))
                    playerDevice[_playerIndex].sprite = controllerIcons[1];
                else
                    playerDevice[_playerIndex].sprite = null;
                playerDevice[_playerIndex].enabled = true;

                // Remove Button
                playerRemoveBtns[_playerIndex].SetActive(true);
            }     
        }
        else
        {
            if (GAMEMANAGER.Instance.gameState == GAMEMANAGER.GameState.JoinSession)
            {
                // Model
                playerModels[_playerIndex].gameObject.SetActive(false);

                // Device Sprite
                playerDevice[_playerIndex].enabled = false;

                // Remove Button
                playerRemoveBtns[_playerIndex].SetActive(false);
            }
        }
	}

    public void LaunchParty()
    {
        if (GAMEMANAGER.Instance.playerNumber > 0)
            GAMEMANAGER.Instance.StartParty();
        else
            print("Error cannot Start Party (PlayerNumber=" + GAMEMANAGER.Instance.playerNumber + ")");
    }

    public void RemovePlayer(int _player)
    {
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();

        for(int i = 0; i < playerInputs.Length; i++)
        {
            if (playerInputs[i].playerIndex == _player)
            {
                Player player = playerInputs[i].GetComponentInChildren<Player>();
                if (player.connected)
                {
                    ChangePlayerState(playerInputs[i].playerIndex, false, Color.black);
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
                    ChangePlayerState(playerInput.playerIndex, true, playerInput.GetComponentInChildren<Player>().color, playerInput.currentControlScheme);
                    break;
                }
            }
        }
    }


    public void PlayButtonSound()
    {
        buttonSelect.Play();
    }
}
