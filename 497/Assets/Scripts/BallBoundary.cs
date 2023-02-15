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

    public bool isAnyServing = true;
    public int servingTeam;
    public int playerTurn;

    public GameObject outText;
    public float textOffset;

    //private Coroutine currentCoroutine;
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

    private void Update()
    {
 
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
        touchedGroundOnceOut = false;
        transform.position = new Vector3(0, 2, 0); // temporary placement
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        isAnyServing = true;
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
    private IEnumerator SwitchTurn() // update playerTurn bool 
    {
        // determine by which player court it is, and has ball bounced in other player's court yet
        yield return null;
        playerTurn = OtherTeam(playerTurn);
        bouncedInOpponentCourtOnce = false;
        touchedGroundOnceOut = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            if (!touchedGroundOnceOut)
            {
                if (OutofBounds())
                {
                    //if (true) // if serve (if anything) goes out of bounds, award point to playerturn
                    //{
                        if (BallInWhichCourt() != playerTurn)
                        {
                            Debug.Log("out of bounds");
                            isAnyServing = false;
                            GameManager.Instance.players[BallInWhichCourt()].GetComponent<Player>().AddScore();
                            StartCoroutine(OutBounds());

                        }
                    //}
                }
                touchedGroundOnceOut = true;
            }
            // if ball hits the ground in your court on your turn, other team gets point 
            if(playerTurn == BallInWhichCourt())
            {
                Debug.Log("its your turn and u suck player: " + playerTurn);
                GameManager.Instance.players[OtherTeam(playerTurn)].GetComponent<Player>().AddScore();
                StartCoroutine(ResetBall());
            }
            // if ball bounced in opponent service court once, switch player turn 
            if(!bouncedInOpponentCourtOnce && BallInWhichCourt() != playerTurn)
            {
                bouncedInOpponentCourtOnce = true;
                isAnyServing = false;
                StartCoroutine(SwitchTurn());
            }
        }
        if (collision.gameObject.tag == "Racket")
        {
            //receiving without letting ball bounce is no bueno 
            if(!bouncedInOpponentCourtOnce && collision.transform.parent.GetComponent<Player>().playerTeam != playerTurn)
            {
                Debug.Log("touched racket no bounce bad");
                GameManager.Instance.players[playerTurn].GetComponent<Player>().AddScore();
                StartCoroutine(ResetBall());
            }
        }
    }
}
// player bounces in oppoenents court, if hadnt bounced before, switch turn 