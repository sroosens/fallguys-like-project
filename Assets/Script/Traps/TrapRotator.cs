using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapRotator : MonoBehaviour
{
    public Vector3 rotationRate = new Vector3(0, 0, 0);
    public bool activated = false;
    void FixedUpdate()
    {
        if (activated)
        transform.Rotate(rotationRate.x * Time.deltaTime, rotationRate.y * Time.deltaTime, rotationRate.z * Time.deltaTime); //rotates 50 degrees per second around z axis
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }
}
