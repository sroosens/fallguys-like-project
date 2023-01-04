using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float cameraSpeed = 5f;
    public float jumpHeight = 2f;
    public float groundDistance = 0.2f;
    public LayerMask ground;

    private Vector2 movementInput;
    private Vector2 cameraInput;
    public Camera camera;
    private Animator animator;

    private GameObject playerModel;
    

    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public bool connected = true;
    [HideInInspector]
    public Checkpoint lastChekpoint;

    public Color color = Color.red;

    private Rigidbody body;
    private Vector3 inputs = Vector3.zero;
    private bool isGrounded = true;
    private bool isJumping = false;
    private Transform groundChecker;

    private void Awake()
    {
        //camera = GetComponentInChildren<Camera>();
        animator = GetComponentInChildren<Animator>();
        playerModel = animator.gameObject;
        playerModel.SetActive(false);
        playerInput = transform.parent.GetComponent<PlayerInput>();
        playerInput.DeactivateInput();
        color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        body = GetComponentInChildren<Rigidbody>();
        groundChecker = GetComponentInChildren<Transform>();

        MenuManager.Instance.ChangePlayerState(playerInput.playerIndex, true, playerInput.GetComponentInChildren<Player>().color, playerInput.currentControlScheme);
        SetColorOnCharacter();
    }

    private void Update()
    {
        /*        transform.Translate(new Vector3(movementInput.x, 0, movementInput.y) * speed * Time.deltaTime);
                //camera.transform.parent.Rotate(new Vector3(0, cameraInput.x, 0) * cameraSpeed * Time.deltaTime);

                if (movementInput != new Vector2(0, 0))
                {
                    playerModel.transform.rotation = camera.transform.parent.rotation;
                    gameObject.transform.rotation = camera.transform.parent.rotation;
                }

        */
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, ground, QueryTriggerInteraction.Ignore);

        inputs = Vector3.zero;
        inputs.x = movementInput.x;
        inputs.z = movementInput.y;
        if (inputs != Vector3.zero)
        {
            transform.forward = inputs;
        }

        if (isJumping && isGrounded)
        {
            body.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
            isJumping = false;
        }
    }

    private void FixedUpdate()
    {
        body.MovePosition(body.position + inputs * speed * Time.fixedDeltaTime);
    }

    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            isJumping = true;
    }

    public void EnterGame()
    {
        playerModel.SetActive(true);
        playerInput.ActivateInput();
        playerInput.SwitchCurrentActionMap("Gameplay");
    }

    public void Disconect()
    {
        MenuManager.Instance.ChangePlayerState(playerInput.playerIndex, false, color);
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
        MenuManager.Instance.ChangePlayerState(playerInput.playerIndex, true, color, playerInput.currentControlScheme);
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

    private void SetColorOnCharacter()
    {
        playerModel.GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_BodyColor", color);
    }

    public void Death()
    {
        transform.position = lastChekpoint.transform.position;
        transform.rotation = lastChekpoint.transform.rotation;
    }
}
