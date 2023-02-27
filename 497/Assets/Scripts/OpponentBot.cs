using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OpponentBot : MonoBehaviour
{
    bool isSwinging = false;
    float lastSwing;
    GameObject ball;
    public float force = 8;
    private Rigidbody ballPhysics;

    void Start()
    {
        lastSwing = 0;
        ball = GameManager.Instance.ball;
        ballPhysics = ball.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Time.time - lastSwing <= 1)
        {
            return;
        }

        bool isInFront = ballPhysics.position.x <= transform.position.x;
        bool isCloseEnough = Vector3.Distance(transform.position, ballPhysics.position) < 1;
        //Debug.Log(Vector3.Distance(transform.position, ballPhysics.position).ToString());
        if (isInFront && isCloseEnough)
        {
            Debug.Log("poggers");
            lastSwing = Time.time;
            HitBall();
        }
        
    }

    void HitBall()
    {
        Vector3 currVelocity = ballPhysics.velocity;
        Vector3 normalVector = transform.rotation * Vector3.right; // will probably have to change later
        // Bounce the ball off the racket first
        ballPhysics.velocity = Vector3.Reflect(ballPhysics.velocity, normalVector);
        ballPhysics.velocity *= 0.7f;

        Vector3 aim;
       
        aim = aimBot();
        ballPhysics.AddForce(aim, ForceMode.VelocityChange);

        BallBoundary.Instance.SwitchTurn();
    }
    private Vector3 aimBot()
    {
        Transform otherPlayer = gameObject.GetComponentInParent<Transform>();
        Vector3 otherPos = otherPlayer.position;
        // aimbot toward center is default
        Vector3 aim = Vector3.Normalize(Vector3.MoveTowards(-ballPhysics.position, Vector3.zero, 1));
        aim = Vector3.ProjectOnPlane(aim, Vector3.up);
        aim *= force;
        aim += Vector3.up * 3;
        if (Math.Abs(gameObject.GetComponentInParent<Transform>().position.x) > 4)
        {
            aim += Vector3.up * 3;
            if (force < 10)
            {
                aim += Vector3.up * 2;
            }
        }

        return aim;
    }
    public bool getSwing()
    {
        return isSwinging;
    }
}
