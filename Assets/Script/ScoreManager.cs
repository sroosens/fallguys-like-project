using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text[] scoreArray;
    public TMP_Text[] nameArray;
    public GameObject[] modelsArray;
    public Camera scoreCam;

    public void SetScore(float[] _scoreArray)
    {
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        int numberOfPlayer = playerInputs.Length;

        for(int i = 0; i < numberOfPlayer; i++)
        {
            nameArray[i].gameObject.SetActive(true);
        }

        float[] _sortScoreArray = _scoreArray;
        System.Array.Sort(_sortScoreArray);

        for (int i = 0; i < numberOfPlayer; i++)
        {
            int scoreToDisplay = (int)Mathf.Round(10000 / _scoreArray[i]);
            for (int j = 0; j < numberOfPlayer; j++)
            {
                if (_scoreArray[i] == _sortScoreArray[j])
                {
                    scoreArray[j].text = scoreToDisplay.ToString();
                    nameArray[j].text = "Player : " + (i+1);
                    modelsArray[j].GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_BodyColor", playerInputs[i].GetComponentInChildren<Player>().color);
                }
            }
        }
    }

    public void DisplayScore(bool _enable)
    {
        if (!_enable)
        {
            foreach (TMP_Text name in nameArray)
            {
                name.gameObject.SetActive(false);
            }
        }
        scoreCam.gameObject.SetActive(_enable);
    }
}