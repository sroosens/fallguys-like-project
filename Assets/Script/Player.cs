using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpHeight = 2f;
    public float groundDistance = 0.2f;
    public float climbForce = 0.05f;
    public float fallMultiplier = 2.0f;
    public LayerMask ground;
    public GameObject deathParticule;

    private Vector2 movementInput;
    private Vector2 cameraInput;
    public Camera mainCamera;
    private Animator animator;

    [HideInInspector]
    public GameObject playerModel;
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public bool connected = true;
    [HideInInspector]
    public Checkpoint lastChekpoint;
    [HideInInspector]
    public Rigidbody rb;

    public Color color = Color.red;

    private Rigidbody body;
    private Vector3 inputs = Vector3.zero;
    private bool isGrounded = true;
    private bool wasGrounded = false;
    private bool isJumping = false;
    private int maxJumps = 2;
    private int curJump;
    private Transform groundChecker;
    private bool toggleJump = false;
    private bool isCollided = false;
    private SoundManager mSoundManager;   

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerModel = animator.gameObject;
        playerModel.SetActive(false);
        playerInput = transform.parent.GetComponent<PlayerInput>();
        playerInput.DeactivateInput();
        color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        body = GetComponentInChildren<Rigidbody>();
        groundChecker = GetComponentInChildren<Transform>();

        curJump = maxJumps;

        MenuManager.Instance.ChangePlayerState(playerInput.playerIndex, true, playerInput.GetComponentInChildren<Player>().color, playerInput.currentControlScheme);
        SetColorOnCharacter();
    }

    private void Start()
    {
        mSoundManager = SoundManager.sInstance;
    }

    private void Update()
    {
        // Manage Player rotation to movement input direction
        if(movementInput != Vector2.zero && body.velocity != Vector3.zero)
        {
            Debug.Log("rotate");
            // Find target rotation IAW velocity
            Vector3 dir = body.velocity;
            dir.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(dir, transform.up);
            // Retrieve only Y axis value
            //targetRotation.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y, 0);
            // Rotate Player Model to target rotation
            
            playerModel.transform.rotation = Quaternion.Lerp(playerModel.transform.rotation, targetRotation, 10.0f * Time.deltaTime);

        }
    }

    private void FixedUpdate()
    {
        // Check if body is on the ground
        wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, ground, QueryTriggerInteraction.Ignore);


        //Vector3 dir = Vector3.forward * movementInput.y + Vector3.right * movementInput.x;
        //
        //animator.SetFloat("Speed", Mathf.Clamp01(dir.magnitude));
        //
        //
        //
        //Vector3 force = dir * speed;
        //body.MovePosition(body.position + force * Time.fixedDeltaTime);
        
        ManageGravity();

        ManageJump();

        Move();
    }

    private void Move()
    {
        if(!isJumping)
            body.velocity = Vector3.ClampMagnitude(body.velocity, 4f);
        else
            body.velocity = Vector3.ClampMagnitude(body.velocity, 10f);

        // Get horizontal and vertical inputs and assign to vector3 "move"
        Vector3 move = new Vector3(movementInput.x, 0f, movementInput.y);

        // Assign camera transform forward to the ball forward
        move = mainCamera.transform.TransformDirection(move);
        move.y = 0f;

        // Add force to body in accordance with movement input and character speed
        // Avoid to be stuck against an object when falling
        if ( !(!isGrounded && isCollided) )
        {
            body.AddForce(move * speed, ForceMode.Force);
        }

        // Update Speed value for animation
        animator.SetFloat("Speed", Mathf.Clamp01(body.velocity.magnitude));
        animator.SetFloat("Orient", (Vector3.Dot(playerModel.transform.TransformDirection(Vector3.right).normalized, move.normalized)));

    }

    void OnCollisionEnter(Collision collision)
    {
        isCollided = true;
        mSoundManager.PlaySound("Collision");
    }
    void OnCollisionExit(Collision collision) => isCollided = false;

    private void ManageGravity()
    {
        // Get movement input
        inputs = Vector3.zero;
        inputs.x = movementInput.x;
        inputs.z = movementInput.y;

        if (inputs != Vector3.zero)
        {
            // Check platform slope with raycast
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            {
                // If raycast meets a slope
                if ((hit.normal.y < 0.78f) && isGrounded)
                {
                    if (hit.distance < 0.16f) // Apply force to slope direction
                        body.AddForce(hit.normal * climbForce * speed * Time.fixedDeltaTime);
                    else // Apply downforce when body is in a negative slope
                        body.AddForce(-Vector3.up * 0.1f * speed * Time.fixedDeltaTime);
                }
            }
        }
        else
        {
            // Slow down body when no input and is grounded
            if (isGrounded)
                body.velocity = body.velocity * 0.99f;
        }

        // Add fall multiplier to gravity
        if (isGrounded)
            Physics.gravity = new Vector3(0, -9.78f, 0);
        else
            Physics.gravity = new Vector3(0, -9.78f, 0) * fallMultiplier;
    }

    private void ManageJump()
    {
        if (toggleJump)
        {
            // Add force to body if jump is requested and body is on the ground
            if (curJump > 0)
            {

                if (curJump == 2 && isGrounded) // Simple jump
                {
                    body.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                    animator.SetBool("Jumping", true);
                    mSoundManager.PlaySound("Jump");
                }
                else if (curJump == 1) // Double jump
                {
                    body.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                    animator.SetTrigger("DoubleJump");
                    mSoundManager.PlaySound("DoubleJump");
                }

                isJumping = true;
                curJump--;
            }
            toggleJump = false;
        }

        // Body is no more jumping if previous position was in the air and changed to ground
        if (!wasGrounded && isGrounded)
        {
            isJumping = false;
            curJump = maxJumps;
        }

        // Update Jumping state for animation
        animator.SetBool("Jumping", isJumping);
    }
    public void OnPause(InputAction.CallbackContext ctx) => GAMEMANAGER.Instance.ChangePause(ctx);
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
        rb.useGravity = true;
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
        rb.useGravity = false;
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

    public IEnumerator Death()
    {
        Instantiate(deathParticule, transform.position, Quaternion.identity);
        mSoundManager.PlaySound("PlayerDeath");

        rb.isKinematic = true;
        playerInput.DeactivateInput();
        playerModel.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        rb.isKinematic = false;
        playerInput.ActivateInput();
        playerModel.SetActive(true);

        transform.position = lastChekpoint.transform.position;
        transform.rotation = lastChekpoint.transform.rotation;

        isJumping = false;
        curJump = maxJumps;
        Physics.gravity = new Vector3(0, -9.78f, 0);
    }
}
