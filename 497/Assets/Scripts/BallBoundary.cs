using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Boundary
{
    public float left; // horizontal is z - left is positive and right is negative (might change later but im lazy so i will suffer unintuitiveness for now)
    public float right;
    public float top; // vertical is x 
    public float bottom;
}

public class BallBoundary : MonoBehaviour
{
    [SerializeField] private Boundary boundary;
    [SerializeField] private float timeTillReset;
    private bool touchedGroundOnceOut = false;
    private bool bouncedInOpponentCourtOnce = false;

    public bool isAnyServing = true;
    public int servingTeam;
    public int playerTurn;

    public GameObject outText;
    public float textOffset;

    //private Coroutine currentCoroutine;

    private void Update()
    {

    }
    IEnumerator OutBounds()
    {
        Vector3 ballOutPosition = transform.position;
        outText.SetActive(true);
        outText.transform.position = new Vector3(ballOutPosition.x, ballOutPosition.y + textOffset, ballOutPosition.z);
        yield return new WaitForSeconds(timeTillReset);
        StartCoroutine(ResetBall());
    }
    IEnumerator ResetBall()
    {
        outText.SetActive(false);
        touchedGroundOnceOut = false;
        transform.position = new Vector3(0, 2, 0); // temporary placement
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        isAnyServing = true;
        //update score, new round
        yield return null;
    }
    private bool OutofBounds()
    {
        return transform.position.z < boundary.left || transform.position.z > boundary.right
            || transform.position.x > boundary.top || transform.position.x < boundary.bottom;
    }
    private int BallInWhichCourt() // returns which player's court the ball is in; only considers which side of net 
    {
        return (transform.position.x < 0) ? 0 : 1;
    }
    private int OtherTeam(int thisPlayer) // returns team number of other player
    {
        return (thisPlayer == 0) ? 1 : 0;
    }
    private void SwitchTurn() // update playerTurn bool 
    {
        // determine by which player court it is, and has ball bounced in other player's court yet 
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            Debug.Log(touchedGroundOnceOut);
            if (!touchedGroundOnceOut)
            {
                if (OutofBounds())
                {
                    if (isAnyServing) // if serve goes out of bounds, award point to playerturn
                    {
                        if (BallInWhichCourt() != servingTeam)
                        {
                            isAnyServing = false;
                            GameManager.Instance.players[BallInWhichCourt()].GetComponent<Player>().AddScore();
                            StartCoroutine(OutBounds());

                        }
                    }
                }
                touchedGroundOnceOut = true;
            }
            // if ball hits the ground in your court on your turn, 
        }
    }
}
