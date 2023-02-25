using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //public GameObject ball;
    GameManager gm;
    Player currPlayer;
    List<Vector3> servingPositions;

    public Boundary playerBoundary;
    public float fieldX = 10;
    public float fieldY = 12;

    [SerializeField] private float racketOffset;
    [SerializeField] private float playerOffset;
    [SerializeField] private float movementSpeed;

    private void Start()
    {
        gm = GameManager.Instance;
        currPlayer = GetComponent<Player>();
        servingPositions = new List<Vector3>();
        if (currPlayer.playerTeam == 0)
        {
            servingPositions.Add(new Vector3(-10.5f, 1, -2.5f));
            servingPositions.Add(new Vector3(-10.5f, 1, 2.5f));

            playerBoundary.top = 0;
            playerBoundary.bottom = -fieldY;
            playerBoundary.left = -fieldX;
            playerBoundary.right = fieldX;
        }
        else
        {
            servingPositions.Add(new Vector3(10.5f, 1, 2.5f)); 
            servingPositions.Add(new Vector3(10.5f, 1, -2.5f));

            playerBoundary.top = fieldY;
            playerBoundary.bottom = 0;
            playerBoundary.left = -fieldX;
            playerBoundary.right = fieldX;
        }
    }
    private void FixedUpdate()  
    {
        Vector3 ballPosition = gm.ball.transform.position;
        /*
        //Vector3 velocityDirection = ball.GetComponent<Rigidbody>().velocity.normalized;
        //Debug.Log((Mathf.Atan2(velocityDirection.x, velocityDirection.y) * Mathf.Rad2Deg * Mathf.Sign(velocityDirection.y)));
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, ballPosition.z + (racketOffset * -Mathf.Sign(transform.position.x)));
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        //ballPosition.x + (playerOffset * Mathf.Sign(transform.position.x))
        */

        if (gm.state == GameState.Serve)
        {
            transform.position = servingPositions[gm.serveCount % servingPositions.Count];
        }
        else if (currPlayer.playerTeam != BallBoundary.Instance.playerTurn)
        {
            Vector3 center = new Vector3(8.5f * (currPlayer.playerTeam * 2 - 1), 1, 0); // move toward the center while waiting for opponent to hit the ball
            transform.position = Vector3.MoveTowards(transform.position, center, movementSpeed * Time.deltaTime);
            // should consider moving at different speeds based on how far you have to move?
            return;
        }

        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, ballPosition.z + (racketOffset * -Mathf.Sign(transform.position.x)));
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        //prevent player from going out of bounds 
        if (transform.position.z < playerBoundary.left)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, playerBoundary.left);
        }
        if (transform.position.z > playerBoundary.right)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, playerBoundary.right);
        }
        if (transform.position.x < playerBoundary.bottom)
        {
            transform.position = new Vector3(playerBoundary.bottom, transform.position.y, transform.position.z);
        }
        if (transform.position.x > playerBoundary.top)
        {
            transform.position = new Vector3(playerBoundary.top, transform.position.y, transform.position.z);
        }
    } 
}
