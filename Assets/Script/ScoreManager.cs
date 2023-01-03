using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text[] scoreArray;

    public void SetScore(int _scoreIndex, float _scoreValue)
    {
        scoreArray[_scoreIndex].text = _scoreValue.ToString();
    }
}
