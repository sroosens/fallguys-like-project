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
    public ScoreManager scoreManager;
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

        inputManager.splitScreen = true;
    }

    private void Update()
    {
        if (gameState == GameState.Game)
            time += Time.deltaTime;
        playerNumber = inputManager.playerCount;
    }

    public IEnumerator ScoreSequence()
    {
        inputManager.splitScreen = false;

        gameState = GameState.Score;
        int i = 0;
        foreach (float score in scoreArray)
        {
            scoreManager.SetScore(i, Mathf.Ceil(score));
            i++;
        }
        scoreManager.transform.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        StopParty();
    }

    public void StopParty()
    {
        gameState = GameState.Menu;
        scoreArray = null;
        time = 0;

        scoreManager.transform.gameObject.SetActive(false);

        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(2);
    }

    public void SetScore(int _playerID)
    {
        scoreArray[_playerID] = time;
        playerInGame -= 1;
        if (playerInGame == 0)
            StartCoroutine(ScoreSequence());
    }

    public void JoinPlayer(PlayerInput _playerInput)
    {
        
    }

    public void LeavePlayer(PlayerInput _playerInput)
    {        
        MenuManager.Instance.ChangePlayerState(_playerInput.playerIndex, false, Color.black);
        playerInGame -= 1;
        if (playerInGame == 0)
            StartCoroutine(ScoreSequence());
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
