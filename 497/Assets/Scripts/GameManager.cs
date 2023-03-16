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

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // this is what the article i found said to name it don't judge
    public List<GameObject> players;
    public GameObject ball;
    public GameState state = GameState.Serve; // skipping prematch because idk what that is gonna be used for yet
    public int serveCount = 0;

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

    public IEnumerator AnnounceState(string state) 
    {
        // play audio 
        // display text "DEUCE" or "MATCH POINT" or current scores
        GetComponent<ScoreDisplay>().stateDisplay.text = state;
        GetComponent<ScoreDisplay>().DisplayState();
        yield return new WaitForSeconds(2f);
        GetComponent<ScoreDisplay>().stateDisplayObj.SetActive(false);
    }
    public void Announce()
    {
        int p1points = players[0].GetComponent<Player>().points;
        int p2points = players[1].GetComponent<Player>().points;
        //win game (no deuce)
        if (p1points >= 4 && p2points <= 2) //if you are up 40-30, 40-15 or 40-love, and win one more point, you win the game.
        {
            StartCoroutine(WinMatch(players[0])); //player 1 win
        }
        if (p2points >= 4 && p1points <= 2) //if you are up 40-30, 40-15 or 40-love, and win one more point, you win the game.
        {
            StartCoroutine(WinMatch(players[1])); //player 2 win
        }
        //win game (deuce)
        if (p1points >= 4 && p2points >= 3 && p1points - p2points >= 2)
        {
            StartCoroutine(WinMatch(players[0])); // player 1 win
        }
        if (p2points >= 4 && p1points >= 3 && p2points - p1points >= 2)
        {
            StartCoroutine(WinMatch(players[1])); // player 1 win
        }
        //deuce 
        if (p1points >= 3 && p1points == p2points)
        {
            Debug.Log("deuce");
            StartCoroutine(AnnounceState("DEUCE"));
        }
        //not deuce, match point 
        if (p1points >= 3 && p2points >= 3 && (p1points - p2points == 1 || p2points - p1points == 1))
        {
            Debug.Log("match point");
            StartCoroutine(AnnounceState("MATCH POINT"));
        }
    }
    public IEnumerator WinMatch(GameObject player) 
    {
        // play audio 
        // display text "YOU WIN" on player's side of split screen
        // maybe display something on mobile too idk 
        // announce which team wins 
        Debug.Log("player " + (player.GetComponent<Player>().playerTeam + 1) + " wins this round");
        GetComponent<ScoreDisplay>().stateDisplay.text = "player " + (player.GetComponent<Player>().playerTeam+1) + " wins this round";
        GetComponent<ScoreDisplay>().DisplayState();
        players[0].GetComponent<Player>().points = 0;
        players[1].GetComponent<Player>().points = 0;
        yield return new WaitForSeconds(2f);
        GetComponent<ScoreDisplay>().HideState();
        // next match , if last match - win total game 
    }
}
