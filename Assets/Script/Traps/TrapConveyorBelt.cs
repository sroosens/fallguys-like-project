using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapConveyorBelt : MonoBehaviour
{
    public float speed;
    public List<GameObject> others;

    private Vector3 direction;

    private void Start()
    {
        direction = transform.forward;
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < others.Count; i++)
        {
            others[i].GetComponent<Rigidbody>().velocity = speed * direction * Time.fixedDeltaTime;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        others.Add(collision.gameObject);
    }

    public void OnCollisionExit(Collision collision)
    {
        others.Remove(collision.gameObject);
    }
}
