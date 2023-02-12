using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerTeam;
    [SerializeField] private Boundary playerBoundary;
    public int points = 0; //1, 2, 3, ...
    public int score = 0; //15, 30, 40, ...
    private bool myTurn = false;

    public GameObject otherPlayerObject;
    Player otherPlayer;

    private void Start()
    {
        otherPlayer = otherPlayerObject.GetComponent<Player>();
        playerBoundary = GetComponent<PlayerBoundary>().playerBoundary; 
    }
    private void Update()
    {
        //win game (no deuce)
        if(points >= 4 && otherPlayer.points <= 2) //if you are up 40-30, 40-15 or 40-love, and win one more point, you win the game.
        {
            GameManager.Instance.WinMatch();
        }
        //win game (deuce)
        if(points >= 4 && otherPlayer.points >= 3 && points - otherPlayer.points >= 2)
        {
            GameManager.Instance.WinMatch();
        }

        //deuce 
        if(points >= 3 && points == otherPlayer.points)
        {
            GameManager.Instance.AnnounceState();
        }
        //not deuce, match point 
        if(points - otherPlayer.points > 1 || otherPlayer.points - points > 1)
        {
            GameManager.Instance.AnnounceState();
        }

        //research how to win a tiebreak game 

        //add lose game condition for multiplayer i think
    }

    //how to score a point 
    // if other player hits ball out of bounds immediately, you receive a point : done 

    // if other player hits ball and ball bounces in their court or hits net, you receive a point  
    // if other player misses ball and ball hits within bounds, you receive a point 
    //          ball collides with opponent ground after bouncng once on their side 
    //  when you score a point, reset ball, [i think change who is serving? i'll need to research specific rules sadge] 
    //why are there so many rules luigi gah

    //collisions, if racket hits ball 
    public void AddScore()
    {
        points += 1;
        score = GetScore();
    }
    public int GetScore()
    {
        switch (points)
        {
            case 0:
                score = 0;
                break;
            case 1:
                score = 15;
                break;
            case 2:
                score = 30;
                break;
            case 3:
                score = 40;
                break;
            case (> 3):
                score = 40 + (points - 3);
                break;
            default:
                break;
        }
        return score;
    }
}
