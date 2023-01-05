using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPlayer : MonoBehaviour
{
    private Vector2 cameraInput;
    public float cameraSpeed = 20;
    public float smoothSpeed = 0.125f;

    // Update is called once per frame
    void Update()
    {
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(0, cameraInput.x, 0), smoothSpeed);

        transform.parent.Rotate(smoothedPosition * cameraSpeed * Time.deltaTime);
    }
    public void OnCameraMove(InputAction.CallbackContext ctx) => cameraInput = ctx.ReadValue<Vector2>();
}
