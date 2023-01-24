using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firework : MonoBehaviour
{
    private void Start()
    {
        SoundManager.sInstance.PlaySound("Firework");
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
