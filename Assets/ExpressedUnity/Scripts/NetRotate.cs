using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetRotate : MonoBehaviour
{
    public Vector3 RotOffset;
    public float Speed;
    public int Layer = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(RotOffset * Speed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision collision)
    {

        if(collision.collider.gameObject.layer == Layer)
        {
            //Debug.LogError("We are on rotate");
            collision.collider.gameObject.transform.root.SetParent(this.transform);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.layer == Layer)
        {
            //Debug.LogError("We are off rotate");
            collision.collider.gameObject.transform.root.SetParent(null);
        }
    }
}
