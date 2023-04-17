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
    public AudioClip racketMiss;
    bool isSwingingBack = false;

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
            Debug.Log("single player test");
            ball = GameManager.Instance.ball;
            ballPhysics = ball.GetComponent<Rigidbody>();
        }
        if (Time.time - lastSwing >= 0.35)
        {
            isSwinging = false;
        }

        bool isInFront = ballPhysics.position.x <= transform.position.x;
        bool isCloseEnough = Vector3.Distance(transform.position, ballPhysics.position) < 1;
        //Debug.Log(Vector3.Distance(transform.position, ballPhysics.position).ToString());

        if (isInFront && isCloseEnough)
        {
            lastSwing = Time.time;
            isSwinging = true;
            AudioManager.Instance.Play(racketBounce);
            isSwingingBack = true;
            HitBall();
        }

        if(isSwinging)
        {
            Debug.Log("entered if statement");
            transform.RotateAround(transform.parent.position, new Vector3(0, 1, 0), -250 * Time.deltaTime);
            //if (!AudioManager.Instance.EffectsSource.isPlaying)
            //{
            if (!(AudioManager.Instance.EffectsSource.isPlaying && AudioManager.Instance.EffectsSource.clip == racketBounce))
                AudioManager.Instance.Play(racketMiss);
        }
        else if (isSwingingBack)
        {
            Debug.Log("is swinging back");
            transform.GetComponent<TrailRenderer>().emitting = false;
            isSwingingBack = false;
            float rotateAmount = Mathf.Abs(Mathf.Cos(250 * Time.deltaTime * Mathf.Deg2Rad));
            Debug.Log(rotateAmount);
            if (Mathf.Abs(transform.localPosition.z + 1.5f) <= rotateAmount)
            {
                Debug.Log(transform.localPosition);
                transform.localPosition = new Vector3(0, 0, -1.5f);
                transform.rotation = Quaternion.Euler(0, -90, 120);
               
            }
            else
            {
                isSwingingBack = true;
                transform.RotateAround(transform.parent.position, new Vector3(0, 1, 0), 250 * Time.deltaTime);
            }
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
