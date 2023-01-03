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
    private PlayerInput playerInput;


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
    }

    public void Reconnected()
    {
        MenuManager.Instance.ChangePlayerState(playerInput.playerIndex, true);
        if(GAMEMANAGER.Instance.gameState == GAMEMANAGER.GameState.Game)
        {
            playerModel.SetActive(true);
            playerInput.ActivateInput();
        }        
    }
}
