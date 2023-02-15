using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] players;

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
