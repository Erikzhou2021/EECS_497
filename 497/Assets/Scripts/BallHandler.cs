using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallHandler : MonoBehaviour
{
    bool isSwinging = false;
    bool hit = false;
    public GameObject ball;
    public float swingForce = 12;
    float lastSwing;
    float lastServe = 0;
    private Rigidbody ballPhysics;

    public float rotationSpeed = 200f;
    public float aimStrength = 1;

    void Start()
    {
        lastSwing = 0;
        lastServe = 0;
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
        if (Time.time - lastSwing >= 0.5)
        {
            isSwinging = false;
            hit = false;

            StartCoroutine(RotateBack());
        }
        if (isSwinging && !hit)
        {
            bool isInFront = ballPhysics.position.x >= transform.position.x;
            bool isCloseEnough = Vector3.Distance(transform.position, ballPhysics.position) < 1;
            if (isInFront && isCloseEnough)
            {
                hit = true;
                Debug.Log(swingForce);
                HitBall(swingForce);
            }
        }
    }
    IEnumerator RotateBack()
    {
        GetComponent<TrailRenderer>().emitting = false;
        yield return new WaitForEndOfFrame();

        while (transform.parent.position.x < 0 && (transform.localPosition.z > (-1.49))
            || (transform.parent.position.x > 0 && (transform.localPosition.z < (1.49)))) //for forehand 
        {
            transform.RotateAround(transform.parent.position, new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void HitBall(float force)
    {
        BallBoundary.Instance.SwitchTurn();
        if (GameManager.Instance.state == GameState.Serve)
        {
            force += 7;
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
            serveVect.Normalize();
            serveVect *= force;
            serveVect.y = 5;
            GameManager.Instance.state = GameState.Rally; // could break if they hit the ball right after it goes out
            ballPhysics.AddForce(serveVect, ForceMode.VelocityChange);
            return;
        }
        Vector3 currVelocity = ballPhysics.velocity;
        Vector3 normalVector = transform.rotation * Vector3.right; // will probably have to change later
        // Bounce the ball off the racket first
        ballPhysics.velocity = Vector3.Reflect(ballPhysics.velocity, normalVector);
        ballPhysics.velocity *= 0.64f;

        // combine aimbot force
        Vector3 aim = aimBot(force);
        //aim *= aimStrength;
        //Debug.Log((ballPhysics.velocity.magnitude + aimStrength));
        //ballPhysics.velocity = (ballPhysics.velocity + aim)/(2); // take the average of the two
        //Debug.Log(ballPhysics.velocity);
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
        if (Math.Abs(gameObject.GetComponentInParent<Transform>().position.x) > 6) // not too close to aim
        {
            if (otherPos.z <= -2) // opponent is far right
            {
                //Debug.Log("Mid Left"); // aim toward the mid left
                aim = Vector3.Normalize(new Vector3(7.5f - ballPhysics.position.x, 0, -ballPhysics.position.z + 1.5f));
            }
            else if (otherPos.z >= 2) // opponent is far left
            {
                //Debug.Log("Mid Right") // aim toward the mid right
                aim = Vector3.Normalize(new Vector3(7.5f - ballPhysics.position.x, 0, -ballPhysics.position.z - 1.5f));
            }
            else if (otherPos.z < 0) // oponent is mid right
            {
                //Debug.Log("Far Left"); // aim toward the far left
                aim = Vector3.Normalize(new Vector3(7.5f - ballPhysics.position.x, 0, -ballPhysics.position.z + 3));
            }
            else if (otherPos.z > 0)
            {
                //Debug.Log("Far Right"); // aim toward the far right
                aim = Vector3.Normalize(new Vector3(7.5f - ballPhysics.position.x, 0, -ballPhysics.position.z - 3));
            }
        }
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
    public void Serve()
    {
        lastServe = Time.time;
        ballPhysics.position = transform.position + new Vector3(0.3f, 1.5f, 0);
        ballPhysics.velocity = new Vector3(0, 0.2f, 0);
    }
    public bool GetSwing()
    {
        return isSwinging;
    }
    public void StartSwing(float force)
    {
        swingForce = force;
        isSwinging = true;
        lastSwing = Time.time;
    }
}
