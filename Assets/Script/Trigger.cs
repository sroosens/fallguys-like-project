using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public GameObject firework;
    private void OnTriggerEnter(Collider other)
    {
        Vector3 bounds = GetComponent<Collider>().bounds.size;
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i<8; i++)
            {
                Instantiate(firework, new Vector3(Random.Range(transform.position.x - (bounds.x * 0.5f), transform.position.x + (bounds.x * 0.5f)), transform.position.y, transform.position.z), Quaternion.identity);
            }

            other.GetComponent<Player>().Finish();
            GAMEMANAGER.Instance.SetScore(other.GetComponent<Player>().playerInput.playerIndex);
        }
    }
}
