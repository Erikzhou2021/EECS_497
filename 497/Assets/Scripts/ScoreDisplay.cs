using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreDisplay;

    private void Update()
    {
        DisplayScore();
    }
    public void DisplayScore()
    {
        scoreDisplay.text = GameManager.Instance.players[0].GetComponent<Player>().GetScore().ToString() + "-" +
            GameManager.Instance.players[1].GetComponent<Player>().GetScore().ToString();
    }
}
