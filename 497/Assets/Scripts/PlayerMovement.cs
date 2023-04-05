using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
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

    private float oldPosZ = 0;
    public Animator animator;

    private void Start()
    {
        //testing 
            animator = transform.GetComponentInChildren<Animator>();

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
        if (IsLocalPlayer)
        {
            plmove();
           // plmoveServerRpc();
        }
     
    }

    private void plmove()
    {
        racketHeight = transform.GetChild(0).position.y;
        racketOffset = transform.position.z - transform.GetChild(0).position.z;
        Debug.Log("entered fix update");
        Vector3 ballPosition = gm.ball.transform.position;
        if (gm.state == GameState.Serve)
        {
            Debug.Log("game state is server");
            transform.position = servingPositions[gm.serveCount % servingPositions.Count];
            animator.SetBool("Serve", true);
        }
        else if (currPlayer.playerTeam != BallBoundary.Instance.playerTurn)
        {
            Vector3 center = new Vector3(8.5f * (currPlayer.playerTeam * 2 - 1), 1, 0); // move toward the center while waiting for opponent to hit the ball
            transform.position = Vector3.MoveTowards(transform.position, center, movementSpeed * Time.deltaTime);
            // should consider moving at different speeds based on how far you have to move?
            animator.SetBool("Serve", false);
        }
        else
        {
            float v1, t3 = 0, t4 = 0;
            // figure out when the ball is going to be at the same height as the racketHeight
            // racketHeight = 0.5 * g * t^2 + v0 * t + x0
            Vector3 ballVelocity = gm.ball.GetComponent<Rigidbody>().velocity;
            float v0 = ballVelocity.y;
            float t1 = (float)(-v0 + Math.Sqrt(v0 * v0 - 2 * Physics.gravity.y * (ballPosition.y - racketHeight))) / Physics.gravity.y;
            float t2 = (float)(-v0 - Math.Sqrt(v0 * v0 - 2 * Physics.gravity.y * (ballPosition.y - racketHeight))) / Physics.gravity.y;
            float t = Math.Max(t1, t2);

            Vector3 targetPos = calculateTargetPos(t, ballVelocity);
            if (float.IsNaN(t) || (!float.IsNaN(t) && Vector3.Distance(transform.position, targetPos) > movementSpeed * t && !BallBoundary.Instance.bouncedInOpponentCourtOnce))
            {   // can't make it to the ball, gotta wait till after the bounce
                float energy = 0.5f * ballVelocity.y * ballVelocity.y + Math.Abs(Physics.gravity.y * ballPosition.y);
                float bounciness = gm.ball.GetComponent<SphereCollider>().material.bounciness;
                // racketHeight = 0.5 * g * t^2 + 0.5 * sqrt(2*energy)
                v1 = (float)Math.Sqrt(2 * energy * bounciness);
                t3 = (float)(-v1 + Math.Sqrt(v1 * v1 - 2 * Physics.gravity.y * (-racketHeight))) / Physics.gravity.y;
                t4 = (float)(-v1 - Math.Sqrt(v1 * v1 - 2 * Physics.gravity.y * (-racketHeight))) / Physics.gravity.y;
                t += Math.Min(t3, t4); // finds the time just after the ball bounces

                targetPos = calculateTargetPos(t, ballVelocity);

                if (Vector3.Distance(transform.position, targetPos) > movementSpeed * t) // still can't reach the ball
                { // wait until the ball is about to bounce for the second time
                    t = Math.Max(t1, t2) + Math.Max(t3, t4);
                    targetPos = calculateTargetPos(t, ballVelocity);
                }
            }
            targetPos.x = Math.Clamp(targetPos.x, playerBoundary.bottom, playerBoundary.top);
            targetPos.z = Math.Clamp(targetPos.z, playerBoundary.left, playerBoundary.right);
            if (float.IsNaN(t) || (!float.IsNaN(t) && Vector3.Distance(transform.position, targetPos) > movementSpeed * t)) // still can't find a solution
            { // just chase the ball and hope you hit
                targetPos = new Vector3(transform.position.x, transform.position.y, ballPosition.z + racketOffset);
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPos, movementSpeed * Time.deltaTime);
            Debug.Log(transform.position);
            animator.SetBool("Serve", false);
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


        //if(transform.position.z < oldPosZ)
        //{
        //    //go left
        //    Debug.Log("lefte");
        //    animator.SetBool("WalkLeft", true);
        //}
        //if (transform.position.z > oldPosZ) 
        //{
        //    //go right
        //    animator.SetBool("WalkLeft", false);
        //}
        animator.SetFloat("Direction", transform.position.z - oldPosZ);
        if (transform.position.z == oldPosZ)
        {
            animator.SetBool("Serve", true);
        }

        oldPosZ = transform.position.z;
    }

    private Vector3 calculateTargetPos(float t, Vector3 ballVelocity)
    {
        Vector3 targetPos = gm.ball.transform.position + ballVelocity * t;
        targetPos.y = transform.position.y;
        targetPos.z += racketOffset; // might break if the opponent is left handed
        return targetPos;
    }

    [ServerRpc]
    private void plmoveServerRpc()
    {
        plmove();
    }


}
