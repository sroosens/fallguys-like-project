using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TrapRotator : MonoBehaviour
{
    Rigidbody rb;
    public Vector3 rotationRate = new Vector3(0, 0, 0);
    public bool activated = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (activated)
            rb.AddTorque(rotationRate * Time.deltaTime * .2f, ForceMode.VelocityChange);
        //rb.MoveRotation(rb.rotation * Quaternion.Euler(rotationRate * Time.deltaTime)); //rotates 50 degrees per second around z axis
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.parent.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.parent.SetParent(null);
    }
}
