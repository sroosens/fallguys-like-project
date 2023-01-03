using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GAMEMANAGER : MonoBehaviour
{
    public static GAMEMANAGER Instance;

    [HideInInspector]
    public int playerNumber = 1;
    [HideInInspector]
    public GameState gameState = GameState.Menu;

    public PlayerInputManager inputManager;
    public bool LoadAtMenu = true;

    private int playerInGame = 0;
    private float[] scoreArray;
    private float time;




    public enum GameState
    {
        Menu,
        JoinSession,
        Game,
        Pause,
        Score
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
        if (LoadAtMenu)
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }


    public void StartParty()
    {
        scoreArray = new float[playerNumber];
        gameState = GameState.Game;
        playerInGame = playerNumber;

        Player[] players = FindObjectsOfType<Player>();
        Vector3[] posArray = new Vector3[] { new Vector3(-2, 0, 2), new Vector3(2, 0, 2), new Vector3(-2, 0, -2), new Vector3(2, 0, -2) };
        int i = 0;
        foreach (Player player in players)
        {
            player.EnterGame();
            player.transform.position = posArray[i];
            i++;
        }
        SceneManager.UnloadSceneAsync(1);
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }

    private void Update()
    {
        if (gameState == GameState.Game)
            time += Time.deltaTime;
        playerNumber = inputManager.playerCount;
    }

    public void ScoreSequence()
    {
        gameState = GameState.Score;
    }

    public void StopParty()
    {
        gameState = GameState.Menu;
        scoreArray = null;
    }

    public void SetScore(int _playerID)
    {
        scoreArray[_playerID] = time;
        playerInGame -= 1;
        if (playerInGame == 0)
            ScoreSequence();
    }

    public void JoinPlayer(PlayerInput _playerInput)
    {
        MenuManager.Instance.ChangePlayerState(_playerInput.playerIndex, true);
    }

    public void LeavePlayer(PlayerInput _playerInput)
    {
        print("Leave");
        MenuManager.Instance.ChangePlayerState(_playerInput.playerIndex, false);
    }

    public void OpenJoinSession()
    {
        gameState = GameState.JoinSession;
        inputManager.EnableJoining();

        inputManager.onPlayerJoined += JoinPlayer;
        inputManager.onPlayerLeft += LeavePlayer;
    }

    public void CloseJoinSession()
    {
        gameState = GameState.Menu;
        inputManager.DisableJoining();

        inputManager.onPlayerJoined -= JoinPlayer;
        inputManager.onPlayerLeft -= LeavePlayer;
    }

    
}
