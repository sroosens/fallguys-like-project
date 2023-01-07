using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float cameraSpeed = 5f;
    public float jumpHeight = 2f;
    public float groundDistance = 0.2f;
    public float climbForce = 0.05f;
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
    private bool preIsGrounded = false;
    private bool isJumping = false;
    private bool toggleJump = false;
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

        // Get movement input
        inputs = Vector3.zero;
        inputs.x = movementInput.x;
        inputs.z = movementInput.y;

        // Appky force to body if requested
        if (inputs != Vector3.zero)
        {
            transform.forward = inputs;

            // Check platform slope with raycast
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            {
                // If raycast meets a slope
                if ((hit.normal.y < 0.78f) && isGrounded)
                {
                    // Apply force to slope direction
                    body.AddForce(hit.normal * climbForce * speed * Time.fixedDeltaTime);
                }
            }
        }

        // Check if body is on the ground
        preIsGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, ground, QueryTriggerInteraction.Ignore);

        // Add force to body if jump is requested and body is on the ground
        if (toggleJump && isGrounded)
        {
            if(toggleJump)
            {
                body.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
                toggleJump = false;
                isJumping = true;
            }
        }

        // Body is no more jumping if previous position was in the air and changed to ground
        if (!preIsGrounded && isGrounded)
            isJumping = false;

        // Update Jumping state for animation
        animator.SetBool("Jumping", isJumping);
    }

    private void FixedUpdate()
    {
        // Add force to body in accordance with movement input and character speed
        body.AddForce(new Vector3(movementInput.x, 0, movementInput.y).normalized * speed * Time.fixedDeltaTime);

        // Update Speed value for animation
        animator.SetFloat("Speed", Vector3.Project(body.velocity, body.transform.forward).magnitude);
    }

    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            toggleJump = true;
        }
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
