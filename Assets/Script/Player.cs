using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed = 5;
    public float cameraSpeed = 5;

    private Vector2 movementInput;
    private Vector2 cameraInput;
    public Camera camera;
    private Animator animator;

    private GameObject playerModel;
    

    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public bool connected = true;

    private void Awake()
    {
        //camera = GetComponentInChildren<Camera>();
        animator = GetComponentInChildren<Animator>();
        playerModel = animator.gameObject;
        playerModel.SetActive(false);
        playerInput = transform.parent.GetComponent<PlayerInput>();
        playerInput.DeactivateInput();

    }

    private void Update()
    {
        transform.Translate(new Vector3(movementInput.x, 0, movementInput.y) * speed * Time.deltaTime);
        //camera.transform.parent.Rotate(new Vector3(0, cameraInput.x, 0) * cameraSpeed * Time.deltaTime);

        if(movementInput != new Vector2(0,0))
        {
            playerModel.transform.rotation = camera.transform.parent.rotation;
            gameObject.transform.rotation = camera.transform.parent.rotation;
        }
        
        
    }

    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();

    

    public void EnterGame()
    {
        playerModel.SetActive(true);
        playerInput.ActivateInput();
        playerInput.SwitchCurrentActionMap("Gameplay");
    }

    public void Disconect()
    {
        MenuManager.Instance.ChangePlayerState(playerInput.playerIndex, false);
        playerModel.SetActive(false);
        playerInput.DeactivateInput();
        connected = false;
        if (GAMEMANAGER.Instance.gameState == GAMEMANAGER.GameState.JoinSession || GAMEMANAGER.Instance.gameState == GAMEMANAGER.GameState.Menu)
            Destroy(playerInput.transform.gameObject);
        else
            StartCoroutine(DisconectedCheck());
    }

    public void Reconnected()
    {
        MenuManager.Instance.ChangePlayerState(playerInput.playerIndex, true, playerInput.currentControlScheme);
        connected = true;
        if (GAMEMANAGER.Instance.gameState == GAMEMANAGER.GameState.Game)
        {
            playerModel.SetActive(true);
            playerInput.ActivateInput();
        }        
    }

    public void Finish()
    {
        playerModel.SetActive(false);
        playerInput.DeactivateInput();
    }

    private IEnumerator DisconectedCheck()
    {
        yield return new WaitForSeconds(10f);
        if (!connected)
            Destroy(playerInput.transform.gameObject);
    }
}
