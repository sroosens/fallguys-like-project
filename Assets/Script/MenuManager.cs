using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject mainPanel;
    public GameObject joinPanel;

    [Header ("PlayerState")]
    public Image[] playerState;


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
    }

    public void Back()
    {
        mainPanel.SetActive(true);
        joinPanel.SetActive(false);
        GAMEMANAGER.Instance.CloseJoinSession();
    }

    public void ChangePlayerState(int _playerIndex, bool _connected)
    {
        if (_connected)
        {
            if(playerState[_playerIndex] != null)
                playerState[_playerIndex].color = Color.green;
        }
        else
        {
            if (playerState[_playerIndex] != null)
                playerState[_playerIndex].color = Color.red;
        }
    }

    public void LaunchParty()
    {
        GAMEMANAGER.Instance.StartParty();
    }
}
