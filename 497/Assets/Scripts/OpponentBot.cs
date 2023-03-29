using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OpponentBot : MonoBehaviour
{
    bool isSwinging = false;
    float lastSwing;
    GameObject ball;
    public float force = 6;
    private Rigidbody ballPhysics;
    public float aimStrength = 1;
    public float speedCap = 20;
    public float targetHeight = 1.5f;

    public AudioClip racketBounce;

    void Start()
    {
        lastSwing = 0;
        ball = GameManager.Instance.ball;
        ballPhysics = ball.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!ball)
        {
            //Debug.Log("single player test");
            ball = GameManager.Instance.ball;
            ballPhysics = ball.GetComponent<Rigidbody>();
        }
        if (Time.time - lastSwing <= 1)
        {
            return;
        }

        bool isInFront = ballPhysics.position.x <= transform.position.x;
        bool isCloseEnough = Vector3.Distance(transform.position, ballPhysics.position) < 1;
        //Debug.Log(Vector3.Distance(transform.position, ballPhysics.position).ToString());

        if (isInFront && isCloseEnough)
        {
            lastSwing = Time.time;
            AudioManager.Instance.Play(racketBounce);
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

        aim *= aimStrength;

        ballPhysics.AddForce(aim, ForceMode.VelocityChange);

        BallBoundary.Instance.SwitchTurn();
    }
    private Vector3 aimBot()
    {
        Vector3 aim = Vector3.Normalize(Vector3.MoveTowards(-ballPhysics.position, new Vector3(-8, 0, 0), 1));
        aim = Vector3.ProjectOnPlane(aim, Vector3.up);
        aim = Vector3.Normalize(aim);
        aim *= ballPhysics.velocity.magnitude + (Math.Max(speedCap - ballPhysics.velocity.magnitude, 0)) * force / speedCap; // enforces the horizontal speed cap (kinda)

        // mgh = 1/2 m v^2 + mgh
        aim.y = Mathf.Sqrt((2 * targetHeight - ballPhysics.position.y) * Math.Abs(Physics.gravity.y));

        Vector3 correction = aim - ballPhysics.velocity;

        return correction;
    }
    public bool getSwing()
    {
        return isSwinging;
    }
}
