using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPlayer : MonoBehaviour
{
    private Vector2 cameraInput;
    public float cameraSpeed = 20;

    // Update is called once per frame
    void Update()
    {
        transform.parent.Rotate(new Vector3(0, cameraInput.x, 0) * cameraSpeed * Time.deltaTime);
    }


    public void OnCameraMove(InputAction.CallbackContext ctx) => cameraInput = ctx.ReadValue<Vector2>();
}
