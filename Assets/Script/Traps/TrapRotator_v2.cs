using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapRotator_v2 : MonoBehaviour
{
    Rigidbody _rigidbody = null;

    [SerializeField] bool _rotateEnabled = true;
    [SerializeField] float _rotationSpeed = 20.0f;

    Dictionary<Rigidbody, float> RBsOnPlatformAndTime = new Dictionary<Rigidbody, float>();
    [SerializeField] List<Rigidbody> RBsOnPlatform = new List<Rigidbody>();

    private void Awake()
    {
        _rigidbody = GetComponent < Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_rotateEnabled)
        {
            _rigidbody.rotation = Quaternion.Euler(_rigidbody.rotation.eulerAngles.x, _rigidbody.rotation.eulerAngles.y + _rotationSpeed * Time.fixedDeltaTime, _rigidbody.rotation.eulerAngles.z);
        }

        if(RBsOnPlatform.Count != RBsOnPlatformAndTime.Count)
        {
            RBsOnPlatformAndTime.Clear();
            foreach (Rigidbody rb in RBsOnPlatform)
            {
                RBsOnPlatformAndTime.Add(rb, 1.0f);
            }
        }
        
        foreach (Rigidbody rb in RBsOnPlatform)
        {
            RBsOnPlatformAndTime.TryGetValue(rb, out float timer);
            
            if(timer < 1.0f)
            {
                RBsOnPlatformAndTime[rb] += Time.deltaTime * 4.0f;
            }
            else if (timer > 1.0f)
            {
                RBsOnPlatformAndTime[rb] = 1.0f;
            }
            RotateRBOnPlatform(rb, timer);
        }
    }

    private void RotateRBOnPlatform(Rigidbody rb, float timer)
    {
        if(_rotateEnabled)
        {
            float rotationAmount = _rotationSpeed * timer * Time.deltaTime;

            Quaternion localAngleAxis = Quaternion.AngleAxis(rotationAmount, _rigidbody.transform.up);
            rb.position = (localAngleAxis * (rb.position - _rigidbody.position)) + _rigidbody.position;

            Quaternion globalAngleAxis = Quaternion.AngleAxis(rotationAmount, rb.transform.InverseTransformDirection(_rigidbody.transform.up));
            rb.rotation *= globalAngleAxis;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("OnTriggerEnter");

        if(!(other.attachedRigidbody == null) && !(other.attachedRigidbody.isKinematic))
        {
            if(!(RBsOnPlatform.Contains(other.attachedRigidbody)))
            {
                RBsOnPlatform.Add(other.attachedRigidbody);
                RBsOnPlatformAndTime.Add(other.attachedRigidbody, 0.0f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        print("OnTriggerExit");

        if (!(other.attachedRigidbody == null))
        {
            if(RBsOnPlatform.Contains(other.attachedRigidbody))
            {
                RBsOnPlatform.Remove(other.attachedRigidbody);
                RBsOnPlatformAndTime.Remove(other.attachedRigidbody);
            }
        }
    }
}
