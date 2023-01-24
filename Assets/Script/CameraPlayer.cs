using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPlayer : MonoBehaviour
{
    [Header("Anchor refs")]
    public Transform arm;

    [Header("Rotation")]
    public float rotationSmoothness = 0.125f;
    public Transform lookAnchor;

    [Header("Translate")]
    public Transform posAnchor;
    public float cameraSpeedHorizontal = 20;
    public float cameraSpeedVertical = 20;
    public float translateSmoothness = 0.125f;
    public float maxVerticalOffset = 5f;

    private Quaternion desiredRotation = Quaternion.identity;
    private float currentOffset;

    private void OnEnable()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(0, cameraInput.x, 0), smoothSpeed);

        arm.rotation = desiredRotation;
        transform.position = Vector3.Lerp(transform.position, posAnchor.position + Vector3.up * currentOffset, Time.deltaTime * translateSmoothness);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookAnchor.position - transform.position, Vector3.up), rotationSmoothness * Time.deltaTime);
    }
    public void OnCameraMove(InputAction.CallbackContext ctx)
    {
        Vector2 cameraInput = ctx.ReadValue<Vector2>();

        desiredRotation *= Quaternion.Euler(0f, cameraInput.x * cameraSpeedHorizontal, 0f);
        currentOffset = Mathf.Clamp(currentOffset + (-cameraInput.y * cameraSpeedVertical), -maxVerticalOffset, maxVerticalOffset);
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, lookAnchor.position);
        Gizmos.DrawWireSphere(lookAnchor.position, .1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, posAnchor.position);
    }
}
