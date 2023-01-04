using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int ChekpointIndex;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(other.GetComponent<Player>().lastChekpoint != null)
            {
                int lastcheckpointIndex = other.GetComponent<Player>().lastChekpoint.ChekpointIndex;
                if (lastcheckpointIndex < ChekpointIndex)
                    other.GetComponent<Player>().lastChekpoint = this;
            }
            else
            {
                other.GetComponent<Player>().lastChekpoint = this;
            }
        }
    }
}
