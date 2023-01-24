using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapMover : MonoBehaviour
{
    public Vector3 posA;
    public Vector3 posB;
    public float duration = 5;
    public float wait = 2;
    public bool activated = true;
    public bool eased = true;

    private float lerpAlpha = 0;
    private float waitTimer;
    private float durationFactor;
    private bool waiting = false;
    private int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        durationFactor = 1 / duration;
        waitTimer = wait;
    }


    void FixedUpdate()
    {
        if (activated)
        {
            if (waiting)
            {
                waitTimer -= Time.fixedDeltaTime;
                if (waitTimer <= 0) { 
                    waiting = false;
                    waitTimer = wait;
                }
            }
            else
            {
                if (lerpAlpha < 1 && index == 0)
                {
                    this.transform.position = Vector3.Lerp(posA, posB, Ease(lerpAlpha));
                    lerpAlpha += Time.fixedDeltaTime * durationFactor;
                } else if (lerpAlpha >= 1 && index == 0)
                {
                    waiting = true;
                    index = 1;
                }
                else if (lerpAlpha > 0 && index == 1)
                {
                    this.transform.position = Vector3.Lerp(posA, posB, Ease(lerpAlpha));
                    lerpAlpha -= Time.fixedDeltaTime * durationFactor;
                } else if (lerpAlpha <= 0 && index == 1)
                {
                    waiting = true;
                    index = 0;
                }
            }
        }
    }

    private float Ease(float _alpha) // Cubic In Out Easing
    {
        if (eased) return _alpha < 0.5 ? 4 * _alpha * _alpha * _alpha : 1 - Mathf.Pow(-2 * _alpha + 2, 3) / 2;
        else return _alpha;
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
