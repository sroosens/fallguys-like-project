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


    private void Awake()
    {
        //camera = GetComponentInChildren<Camera>();
        animator = GetComponentInChildren<Animator>();
        playerModel = animator.gameObject;
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

    
}
