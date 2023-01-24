using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePlayer : MonoBehaviour
{
    public Collider triggerZone;
    public float speed = 20;

    private Vector3 destination;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("Speed", 1f);
        destination = GetRandomPointInBounds(triggerZone.bounds);
        GetComponentInChildren<Renderer>().material.SetColor("_BodyColor", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
    }

    private void Update()
    {
        Vector3 roundedPos = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        Vector3 RoundedDestination = new Vector3(Mathf.Round(destination.x), Mathf.Round(destination.y), Mathf.Round(destination.z));
        if(roundedPos == RoundedDestination)
        {
            destination = GetRandomPointInBounds(triggerZone.bounds);
        }
        transform.rotation = (Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(destination, Vector3.up), 60f * Time.deltaTime));
        transform.Translate(Vector3.forward * speed * Time.deltaTime);        
    }

    private Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(Random.Range(bounds.min.x, bounds.max.x), transform.position.y, Random.Range(bounds.min.z, bounds.max.z));
    }
}
