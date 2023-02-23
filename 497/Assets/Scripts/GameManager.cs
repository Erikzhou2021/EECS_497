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
    public List<GameObject> players = new List<GameObject>();
    public GameObject ball;
    public GameState state = GameState.Serve; // skipping prematch because idk what that is gonna be used for yet
    public int serveCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public IEnumerator AnnounceState(string state) // my code is so ugly i apologise
    {
        // play audio 
        // display text "DEUCE" or "MATCH POINT" or current scores
        GetComponent<ScoreDisplay>().stateDisplay.text = state;
        GetComponent<ScoreDisplay>().DisplayState();
        yield return new WaitForSeconds(2f);
        GetComponent<ScoreDisplay>().stateDisplayObj.SetActive(false);
    }
    public IEnumerator WinMatch() 
    {
        // play audio 
        // display text "YOU WIN" on player's side of split screen
        // maybe display something on mobile too idk 
        // announce which team wins 
        yield return new WaitForSeconds(2f);
        // next match , if last match - win total game 
        // every other match flip court ( i dont think this is necessary but just in case pro tennis people are tight) 
    }
}
