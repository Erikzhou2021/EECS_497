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

    IEnumerator OutBounds(int playerTurn)
    {
        if (!touchedGroundOnceOut)
        {
            if (!scoreStop)
            {
                outText = GameObject.Find("WorldCanvas").transform.Find("Out").gameObject;
                Vector3 ballOutPosition = transform.position;
                outText.SetActive(true);
                outText.transform.position = new Vector3(ballOutPosition.x, ballOutPosition.y + textOffset, ballOutPosition.z);
            }
            gm.players[playerTurn].GetComponent<Player>().AddScore();

            scoreStop = true;
            yield return new WaitForSeconds(timeTillReset);
            outText.SetActive(false);
            StartCoroutine(ResetBall());
            //ScorePoint(playerTurn);

            touchedGroundOnceOut = true;
        }
    }
    public IEnumerator ResetBall()
    {
        scoreStop = false;
        touchedGroundOnceOut = false;
        bouncedInOpponentCourtOnce = false;
        transform.position = new Vector3(0, 2, 0); // temporary placement
        //GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GameManager.Instance.state = GameState.Serve;
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

        if(transform.position.x != 0)
        {
            GameObject.Find("Imprint").SetActive(true);
            GameObject.Find("Imprint").transform.position = new Vector3(transform.position.x, 0.0002f, transform.position.z);
        }

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
            StartCoroutine(OutBounds(playerTurn));
            //ScorePoint(playerTurn);
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
        }
}
