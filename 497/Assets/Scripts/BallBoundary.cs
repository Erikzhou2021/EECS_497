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
    public static BallBoundary Instance;

    [SerializeField] private Boundary boundary;
    [SerializeField] private float timeTillReset;
    public bool touchedGroundOnceOut = false; // used to check if ball is out of bounds after serving 
    public bool bouncedInOpponentCourtOnce = false; // check if ball bounced in opponent's service court, else FAULT 
    public bool scoreStop = false;

    public int servingTeam;
    public int playerTurn; // this is who is supposed to hit the ball next

    public GameObject outText;
    public float textOffset;
    GameManager gm;

    //private Coroutine currentCoroutine;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gm = GameManager.Instance;
        }
        else
        {
            Destroy(this);
        }
    }

    IEnumerator OutBounds()
    {
        Vector3 ballOutPosition = transform.position;
        outText.SetActive(true);
        outText.transform.position = new Vector3(ballOutPosition.x, ballOutPosition.y + textOffset, ballOutPosition.z);
        scoreStop = true;
        yield return new WaitForSeconds(timeTillReset);
        StartCoroutine(ResetBall());
    }
    public IEnumerator ResetBall()
    {
        outText.SetActive(false);
        bouncedInOpponentCourtOnce = false;
        transform.position = new Vector3(0, 2, 0); // temporary placement
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GameManager.Instance.state = GameState.Serve;
        scoreStop = false;
        playerTurn = servingTeam;
        //update score, new round
        yield return null;
    }
    private bool OutofBounds()
    {
        return transform.position.z > boundary.left || transform.position.z < boundary.right
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
    public void SwitchTurn() // update playerTurn bool 
    {
        // determine by which player court it is, and has ball bounced in other player's court yet
        playerTurn = OtherTeam(playerTurn);
        bouncedInOpponentCourtOnce = false;
    }
    private void ScorePoint(int playerNum)
    {
        gm.players[playerNum].GetComponent<Player>().AddScore();
        StartCoroutine(ResetBall()); // maybe replace with replay system later?
    }
    private void OnCollisionEnter(Collision collision)
    {
        // prob shouldn't handle this here since the ball doesn't have to touch the racket to count as a hit
        /*if (collision.gameObject.tag == "Racket") 
        {
            // now the opponent has to hit it next
            SwitchTurn();
            return;
        }*/
        if (collision.gameObject.tag != "Ground")
        {
            return;
        }
        if (gm.state == GameState.Serve)
        { // you fucced up the serve but that's ok
            StartCoroutine(ResetBall());
            return;
        }
        if (bouncedInOpponentCourtOnce)
        {
            Debug.Log("you let the ball bounce twice bozo");
            ScorePoint(OtherTeam(playerTurn));
            return;
        }
        if (OutofBounds())
        {
            Debug.Log("you hit the ball out of bounds bogo");
            ScorePoint(playerTurn);
            return;
        }
        if (BallInWhichCourt() != playerTurn)
        {
            Debug.Log("you didn't get the ball over the net bimbo");
            ScorePoint(playerTurn);
            return;
        }
        // good job you didn't fucc up
        bouncedInOpponentCourtOnce = true;



            /*if(collision.gameObject.tag == "Ground")
            {
                if (!touchedGroundOnceOut) // is this necessary?
                {
                    if (OutofBounds()) 
                    {
                        //if (true) // if serve (if anything) goes out of bounds, award point to playerturn
                        //{
                            if (BallInWhichCourt() != playerTurn)
                            {
                                Debug.Log("out of bounds");
                                GameManager.Instance.state = GameState.Postpoint;
                                GameManager.Instance.players[BallInWhichCourt()].GetComponent<Player>().AddScore();
                                StartCoroutine(OutBounds());

                            }
                        //}
                    }
                    touchedGroundOnceOut = true;
                }
                // if ball bounced in opponent service court once, switch player turn 
                if (!bouncedInOpponentCourtOnce && BallInWhichCourt() != playerTurn)
                {
                    bouncedInOpponentCourtOnce = true;
                    //GameManager.Instance.state = GameState.Rally; // should be uncessary since it has to be rally already
                    Debug.Log("Bruh");
                    StartCoroutine(SwitchTurn());
                }
                // if ball hits the ground in your court on your turn, other team gets point 
                else if (playerTurn == BallInWhichCourt())
                {
                    Debug.Log("its your turn and u suck player: " + playerTurn);
                    GameManager.Instance.players[OtherTeam(playerTurn)].GetComponent<Player>().AddScore();
                    StartCoroutine(ResetBall());
                }
            }
            else if (collision.gameObject.tag == "Racket") //ball probably can't touch the ground and the racket at the same time
            {
                // probably can make it so its impossible to hit the ball before it bounces on the serve

                //receiving without letting ball bounce is no bueno 
                //if(!bouncedInOpponentCourtOnce && collision.transform.parent.GetComponent<Player>().playerTeam != playerTurn)
                //{
                    //Debug.Log("touched racket no bounce bad");
                    //GameManager.Instance.players[playerTurn].GetComponent<Player>().AddScore();
                    //StartCoroutine(ResetBall());
                //}

            }*/
        }
}
// player bounces in oppoenents court, if hadnt bounced before, switch turn 