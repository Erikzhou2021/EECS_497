using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Prematch,
    Serve,
    Rally,
    Postpoint,
    Postmatch
}
//sorry bad code all over 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // this is what the article i found said to name it don't judge
    public List<GameObject> players;
    public GameObject ball;
    public GameState state = GameState.Serve; // skipping prematch because idk what that is gonna be used for yet
    public int serveCount = 0;

    public bool pauseGame = false;
    public bool newMatch = false;
    public bool endGame = false;

    //3 total matches
    public int match = 0; //0,1,2
    private int gamePoint1 = 0;
    private int gamePoint2 = 0;
    public int lead = 0; //0,1

    public Animator animator1;
    public Animator animator2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            players = new List<GameObject>();
        }
        else
        {
            Destroy(this);
        }
    }
    public void AnnounceState(string state) 
    {
        // play audio 
        // display text "DEUCE" or "MATCH POINT" or current scores
        GetComponent<ScoreDisplay>().stateDisplay.text = state;
        StartCoroutine(GetComponent<ScoreDisplay>().DisplayState());
        //yield return new WaitForSeconds(2f);
        //GetComponent<ScoreDisplay>().stateDisplayObj.SetActive(false);
    }
    public void Announce()
    {
        int p1points = players[0].GetComponent<Player>().points;
        int p2points = players[1].GetComponent<Player>().points;
        //win game (no deuce)
        if (p1points >= 4 && p2points <= 2) //if you are up 40-30, 40-15 or 40-love, and win one more point, you win the game.
        {
            //StartCoroutine(WinMatch(players[0])); //player 1 win
            WinMatch(players[0]);
        }
        if (p2points >= 4 && p1points <= 2) //if you are up 40-30, 40-15 or 40-love, and win one more point, you win the game.
        {
            //StartCoroutine(WinMatch(players[1])); //player 2 win
            WinMatch(players[1]);
        }
        //win game (deuce)
        if (p1points >= 4 && p2points >= 3 && p1points - p2points >= 2)
        {
            //StartCoroutine(WinMatch(players[0])); //player 1 win
            WinMatch(players[0]);
        }
        if (p2points >= 4 && p1points >= 3 && p2points - p1points >= 2)
        {
            //StartCoroutine(WinMatch(players[1])); //player 2 win
            WinMatch(players[1]);
        }
        //deuce 
        if (p1points >= 3 && p1points == p2points)
        {
            Debug.Log("deuce");
            //StartCoroutine(AnnounceState("DEUCE"));
            AnnounceState("deuce");
        }
        //before deuce, match point 
        if((p1points == 3 && p2points <= 2) || (p2points == 3 && p1points <= 2))
        {
            Debug.Log("match point");
            //StartCoroutine(AnnounceState("MATCH POINT"));
            AnnounceState("match point");
        }
        //after deuce, match point 
        if (p1points >= 3 && p2points >= 3 && (p1points - p2points == 1 || p2points - p1points == 1))
        {
            Debug.Log("match point");
            //StartCoroutine(AnnounceState("MATCH POINT"));
            AnnounceState("match point");
        }
    }
    public void WinMatch(GameObject player) 
    {
        // play audio 
        // display text "YOU WIN" on player's side of split screen
        // maybe display something on mobile too idk 
        // announce which team wins 

        GetComponent<ScoreDisplay>().stateDisplay.text = "player " + (player.GetComponent<Player>().playerTeam+1) + " wins this round";
        newMatch = true;
        if (players[0].GetComponent<Player>().points > players[1].GetComponent<Player>().points)
        {
            Debug.Log("win true");
            animator1.SetBool("Win", true);
            animator2.SetBool("Lose", true); //uncomment
        }
        else
        {
            animator1.SetBool("Lose", true);
            animator2.SetBool("Win", true); //uncomment

        }
        players[0].GetComponent<Player>().points = 0;
        players[1].GetComponent<Player>().points = 0;
        StartCoroutine(GetComponent<ScoreDisplay>().DisplayState());
        match++;
        if(player.GetComponent<Player>().playerTeam == 0)
        {
            gamePoint1++;
        }
        else
        {
            gamePoint2++;
        }

        // win game 
        if(match == 3)
        {
            //end game 
            endGame = true;
            if(gamePoint1 > gamePoint2)
            {
                lead = 0;
            }
            else
            {
                lead = 1;
            }
            GetComponent<ScoreDisplay>().stateDisplay.text = "player " + (lead + 1) + " wins the game";
            StartCoroutine(GetComponent<ScoreDisplay>().DisplayState());
        }
    }

    public void StartGame()
    {
        StartCoroutine(GetComponent<ScoreDisplay>().DisplayBanner("round 1"));
        animator1 = players[0].transform.GetComponentInChildren<Animator>();
        animator2 = players[1].transform.GetComponentInChildren<Animator>(); //uncomment
    }

    public void DisplayScore()
    {
        GetComponent<ScoreDisplay>().DisplayScore();
    }
}
