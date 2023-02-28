using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    [SerializeField] private float racketHeight;
    [SerializeField] private float playerOffset;
    [SerializeField] private float movementSpeed;
    public Boundary playerBoundary;
    public float fieldX = 10;
    public float fieldY = 12;

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
        if (gm.state == GameState.Serve)
        {
            transform.position = servingPositions[gm.serveCount % servingPositions.Count];
        }
        else if (currPlayer.playerTeam != BallBoundary.Instance.playerTurn)
        {
            Vector3 center = new Vector3(8.5f * (currPlayer.playerTeam * 2 - 1), 1, 0); // move toward the center while waiting for opponent to hit the ball
            transform.position = Vector3.MoveTowards(transform.position, center, movementSpeed * Time.deltaTime);
            Debug.Log("does not equal");
            // should consider moving at different speeds based on how far you have to move?
        }
        else
        {
            //Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, ballPosition.z + (racketOffset * -Mathf.Sign(transform.position.x)));
            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

            bool canReachBeforeBounce = true;
            // figure out when the ball is going to be at the same height as the racketHeight
            // racketHeight = 0.5 * g * t^2 + v0 * t + x0
            Vector3 ballVelocity = gm.ball.GetComponent<Rigidbody>().velocity;
            float v0 = ballVelocity.y;
            float t1 = (float) (-v0 + Math.Sqrt(v0*v0 - 2 * Physics.gravity.y * (ballPosition.y - racketHeight))) / Physics.gravity.y;
            float t2 = (float) (-v0 - Math.Sqrt(v0 * v0 - 2 * Physics.gravity.y * (ballPosition.y - racketHeight))) / Physics.gravity.y;
            float t = Math.Max(t1, t2);
            //Debug.Log("time =" + t);
            if (float.IsNaN(t))
            { // can't get the ball, give up and cry
                canReachBeforeBounce = false;
            }
            Vector3 targetPosition = gm.ball.transform.position + ballVelocity * t;
            targetPosition.y = transform.position.y;
            targetPosition.z += racketOffset * -Mathf.Sign(transform.position.x); // might break if the opponent is left handed
            if (canReachBeforeBounce && Vector3.Distance(transform.position, targetPosition) > movementSpeed * t && !BallBoundary.Instance.bouncedInOpponentCourtOnce) 
            {
                canReachBeforeBounce = false;
            }
            if (!canReachBeforeBounce)
            {   // can't make it to the ball, gotta wait till the next bounce
                float energy = 0.5f * ballVelocity.y * ballVelocity.y + Math.Abs(Physics.gravity.y * ballPosition.y);
                float bounciness = gm.ball.GetComponent<SphereCollider>().material.bounciness;
                // racketHeight = 0.5 * g * t^2 + 0.5 * sqrt(2*energy)
                float v1 = (float) Math.Sqrt(2 * energy * bounciness);
                float t3 = (float)(-v1 + Math.Sqrt(v1 * v1 - 2 * Physics.gravity.y * (-racketHeight))) / Physics.gravity.y;
                float t4 = (float)(-v1 - Math.Sqrt(v1 * v1 - 2 * Physics.gravity.y * (-racketHeight))) / Physics.gravity.y;
                t += Math.Max(t3, t4); //finds when the ball is about to bounce the second time
            }
            targetPosition = gm.ball.transform.position + ballVelocity * t;
            targetPosition.x = Math.Max(targetPosition.x, playerBoundary.bottom);
            targetPosition.x = Math.Min(targetPosition.x, playerBoundary.top);
            targetPosition.y = transform.position.y;
            targetPosition.z += Math.Max(playerBoundary.left, racketOffset * -Mathf.Sign(transform.position.x)); // might break if the opponent is left handed
            targetPosition.z = Math.Min(targetPosition.z, playerBoundary.right);
            //Debug.Log(targetPosition);
            if (float.IsNaN(t) || !float.IsNaN(t) && Vector3.Distance(transform.position, targetPosition) > movementSpeed * t) // still can't find a solution
            { // just chase the ball and hope you hit
                targetPosition = new Vector3(transform.position.x, transform.position.y, ballPosition.z + (racketOffset * -Mathf.Sign(transform.position.x)));
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        }

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
