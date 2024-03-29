using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallHandler : MonoBehaviour
{
    bool hit = false;
    public GameObject ball;
    public float swingForce = 6;
    
    float lastServe = 0;
    private Rigidbody ballPhysics;

    public float rotationSpeed = 200f;
    public float aimStrength = 1;

    public float bounceHeight;
    public float bounceSpeed;
    public float speedCap = 20;
    public float targetHeight = 1.35f;

    Player p;
    Racket r;
    Transform reference;

    public AudioClip racketBounce;
    void Start()
    {
        Physics.gravity = new Vector3(0, -7f, 0); // reduce gravity
        lastServe = 0;
        p = transform.parent.GetComponent<Player>();
        r = transform.parent.GetComponent<Racket>();
        reference = transform.Find("reference");
        StartCoroutine(setVars()); // have to use coroutine bc everything instantiated
    }

    IEnumerator setVars()
    {
        yield return new WaitForSeconds(1);
        ball = GameObject.Find("Ball(Clone)");
        ballPhysics = ball.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!ball)
        {
            Debug.Log("single player test: ball");
            ball = GameManager.Instance.ball;
            ballPhysics = ball.GetComponent<Rigidbody>();
            r = transform.parent.GetComponent<Racket>();
        }
        if (r.GetSwing() && !hit)
        {
            bool isInFront = Math.Abs(ballPhysics.position.x) <= Math.Abs(reference.position.x);
            bool isCloseEnough = Vector3.Distance(reference.position, ballPhysics.position) < 1;
            if (isInFront && isCloseEnough)
            {
                hit = true;
                HitBall(swingForce);
                AudioManager.Instance.Play(racketBounce);
            }
        }
        else if (!r.GetSwing())
        {
            hit = false;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y + (Mathf.Sin(Time.time * bounceSpeed) * bounceHeight), transform.position.z);
    }

    void HitBall(float force)
    {
        BallBoundary.Instance.SwitchTurn();
        if (GameManager.Instance.state == GameState.Serve)
        {
            ballPhysics.velocity = new Vector3(ballPhysics.velocity.x, 0, ballPhysics.velocity.z);
            force += 8;
            GameManager.Instance.serveCount++;
            Vector3 serveVect = Vector3.zero;
            if (transform.position.x < 0)
            {
                serveVect.x = 4 - ballPhysics.position.x;
            }
            else
            {
                serveVect.x = -4 - ballPhysics.position.x;
            }
            if (transform.position.z < 0)
            {
                serveVect.z = 2.5f - ballPhysics.position.z;
            }
            else
            {
                serveVect.z = -2.5f - ballPhysics.position.z;
            }
            Debug.Log(force);
            serveVect.Normalize();
            serveVect *= force;
            serveVect.y = 2f;
            //serveVect.y = Mathf.Sqrt((2 * targetHeight - ballPhysics.position.y) * Math.Abs(Physics.gravity.y));
            GameManager.Instance.state = GameState.Rally;
            ballPhysics.AddForce(serveVect, ForceMode.VelocityChange);
            return;
        }
        Vector3 currVelocity = ballPhysics.velocity;
        Vector3 normalVector = transform.forward;
        // Bounce the ball off the racket first
        ballPhysics.velocity = Vector3.Reflect(ballPhysics.velocity, normalVector);
        ballPhysics.velocity *= 0.64f;

        Vector3 aim = aimBot(force);
        aim *= aimStrength;

        ballPhysics.AddForce(aim, ForceMode.VelocityChange);
    }
    private Vector3 aimBot(float force)
    {
        int otherTeam = (gameObject.GetComponentInParent<Player>().playerTeam == 0) ? 1 : 0;
        Transform otherPlayer = GameManager.Instance.players[otherTeam].GetComponent<Transform>();
        Vector3 otherPos = otherPlayer.position;
        // aimbot toward center is default
        Vector3 aim = Vector3.Normalize(Vector3.MoveTowards(-ballPhysics.position, Vector3.zero, 1));
        aim = Vector3.ProjectOnPlane(aim, Vector3.up);

        float zPos = otherPos.z;
        if (zPos == 0)
        {
            if (transform.forward.z > 0)
            {
                zPos = -0.1f;
            } 
            else if (transform.forward.z < 0)
            {
                zPos = 0.1f;
            }
            else {
                zPos += UnityEngine.Random.Range(-0.1f, 0.1f);
            }
        }

        if (zPos <= -2) // opponent is far right
        {
            Debug.Log("Mid Left"); // aim toward the mid left
            aim = new Vector3(7.5f - ballPhysics.position.x, 0, -ballPhysics.position.z + 1.5f);
        }
        else if (zPos >= 2) // opponent is far left
        {
            Debug.Log("Mid Right"); // aim toward the mid right
            aim = new Vector3(7.5f - ballPhysics.position.x, 0, -ballPhysics.position.z - 1.5f);
        }
        else if (zPos < 0) // oponent is mid right
        {
            Debug.Log("Far Left"); // aim toward the far left
            aim = new Vector3(7.5f - ballPhysics.position.x, 0, -ballPhysics.position.z + 3);
        }
        else if (zPos > 0)
        {
            Debug.Log("Far Right"); // aim toward the far right
            aim = new Vector3(7.5f - ballPhysics.position.x, 0, -ballPhysics.position.z - 3);
        }
        aim = Vector3.Normalize(aim);
        aim *= ballPhysics.velocity.magnitude + (Math.Max(speedCap - ballPhysics.velocity.magnitude, 0)) * force / speedCap; // enforces the horizontal speed cap (kinda)

        // mgh = 1/2 m v^2 + mgh
        aim.y = Mathf.Sqrt((2 * targetHeight - ballPhysics.position.y) * Math.Abs(Physics.gravity.y));

        Vector3 correction = aim - ballPhysics.velocity;

        return correction;
    }
    public void Serve()
    {
        if (!ball)
        {
            ball = GameManager.Instance.ball;
            ballPhysics = ball.GetComponent<Rigidbody>();
            r = transform.parent.GetComponent<Racket>();
        }
        if (Time.time - lastServe < 1.5f)
        {
            return;
        }
        lastServe = Time.time;
        ballPhysics.position = transform.position + new Vector3(0.3f, 1.2f, 0);
        ballPhysics.velocity = new Vector3(0, 4.2f, 0);
    }
    public void StartSwing(float force)
    {
        if (!r)
        {
            Debug.Log(!ball);
            r = transform.parent.GetComponent<Racket>();
        }
        swingForce = force;
        r.SetSwing();
    }
}
