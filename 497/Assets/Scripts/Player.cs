using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerTeam;
    [SerializeField] private Boundary playerBoundary;
    public int points = 0; //1, 2, 3, ...
    public string score; //15, 30, 40, ...
    public bool forehand = true;
    //public GameObject otherPlayerObject;
    //Player otherPlayer;

    GameManager Instance;

    private void Start()
    {
        Instance = GameManager.Instance;
        //otherPlayer = otherPlayerObject.GetComponent<Player>();
        playerBoundary = GetComponent<PlayerMovement>().playerBoundary; 
    }
    private void FixedUpdate() //confusing bc idk if we are doing ai opponent or multiplayer yet 
        //dont put this in update cus bad, announcement whenever score changes 
    {
        if (Instance.state == GameState.Serve && playerTeam == Instance.ball.GetComponent<BallBoundary>().playerTurn && !Instance.pauseGame)
        {
            transform.GetChild(1).Find("Serve").gameObject.SetActive(true);
        }
        else{
            transform.GetChild(1).Find("Serve").gameObject.SetActive(false);
        }

        //research how to win a tiebreak game 

        //add lose game condition for multiplayer i think
    }
    public void AddScore()
    {
        if(!BallBoundary.Instance.scoreStop){
            points += 1;
            score = GetScore();
            Instance.Announce(); //uncomment later
            Instance.DisplayScore();
        }
    }

    public string GetScore()
    {
        switch (points)
        {
            case 0:
                score = "Love";
                break;
            case 1:
                score = "15";
                break;
            case 2:
                score = "30";
                break;
            case 3:
                score = "40";
                break;
            case (> 3):
                score = "40";
                int otherTeam = (playerTeam == 0) ? 1 : 0;
                if (points - Instance.players[otherTeam].GetComponent<Player>().points == 1)
                {
                    score = "Advantage";
                }
                break;
            default:
                break;
        }
        return score;
    }
}
