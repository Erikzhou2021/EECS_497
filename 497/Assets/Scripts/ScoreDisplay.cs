using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI stateDisplay;
    public GameObject stateDisplayObj;

    private void Start()
    {
        stateDisplayObj.SetActive(false);
    }
    private void Update()
    {
        DisplayScore();
    }
    public void DisplayScore()
    {
        if (GameManager.Instance.players.Count < 2)
        {
            return;
        }
        scoreDisplay.text = GameManager.Instance.players[0].GetComponent<Player>().GetScore() + "-" +
            GameManager.Instance.players[1].GetComponent<Player>().GetScore();
    }
    public void DisplayState()
    {
        stateDisplayObj.SetActive(true);
    }
    public void HideState()
    {
        stateDisplayObj.SetActive(false);
    }
}
