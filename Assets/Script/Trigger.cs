using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<Player>().Finish();
            GAMEMANAGER.Instance.SetScore(other.GetComponent<Player>().playerInput.playerIndex);
        }
    }
}
