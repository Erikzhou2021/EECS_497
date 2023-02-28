using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallHandler : MonoBehaviour
{
    bool isSwinging = false;
    bool hit = false;
    public GameObject ball;
    public float force = 12;
    float lastSwing;
    float lastServe = 0;
    private Rigidbody ballPhysics;

    public float rotationSpeed = 200f;

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
                HitBall();
            }
        }
    }
    IEnumerator RotateBack()
    {
        GetComponent<TrailRenderer>().emitting = false;
        yield return new WaitForEndOfFrame();

        while ((transform.localPosition.z > (-1.49) && transform.parent.position.x < 0)
            || (transform.localPosition.z < (1.49) && transform.parent.position.x > 0)) //for forehand 
        {
            transform.RotateAround(transform.parent.position, new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void HitBall()
    {
        Debug.Log("Hit");
        BallBoundary.Instance.SwitchTurn();
        Vector3 currVelocity = ballPhysics.velocity;
        Vector3 normalVector = transform.rotation * Vector3.right; // will probably have to change later
        // Bounce the ball off the racket first
        ballPhysics.velocity = Vector3.Reflect(ballPhysics.velocity, normalVector);
        ballPhysics.velocity *= 0.7f;
        
        Vector3 aim;

        // then apply aimBot force
        aim = aimBot();
        ballPhysics.AddForce(aim, ForceMode.VelocityChange);
    }
    private Vector3 aimBot()
    {
        Transform otherPlayer = gameObject.GetComponentInParent<Transform>();
        Vector3 otherPos = otherPlayer.position;
        // aimbot toward center is default
        Vector3 aim = Vector3.Normalize(Vector3.MoveTowards(-ballPhysics.position, Vector3.zero, 1));
        aim = Vector3.ProjectOnPlane(aim, Vector3.up);
        if (Math.Abs(gameObject.GetComponentInParent<Transform>().position.x) > 6) // not too close to aim
        {
            if (otherPos.z <= -2) // opponent is far right
            {
                Debug.Log("Mid Left");
                // aim toward the mid left
                aim = Vector3.Normalize(new Vector3(7.5f - ballPhysics.position.x, 0, -ballPhysics.position.z + 1.5f));
            }
            else if (otherPos.z >= 2) // opponent is far left
            {
                Debug.Log("Mid Right");
                // aim toward the mid right
                aim = Vector3.Normalize(new Vector3(7.5f - ballPhysics.position.x, 0, -ballPhysics.position.z - 1.5f));
            }
            else if (otherPos.z < 0) // oponent is mid right
            {
                Debug.Log("Far Left");
                // aim toward the far left
                aim = Vector3.Normalize(new Vector3(7.5f - ballPhysics.position.x, 0, -ballPhysics.position.z + 3));
            }
            else if (otherPos.z > 0)
            {
                Debug.Log("Far Right");
                // aim toward the far right
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
        GameManager.Instance.state = GameState.Rally;
        GameManager.Instance.serveCount++;
        lastServe = Time.time;
        ballPhysics.position = transform.position + new Vector3(0.3f, 1.5f, 0);
        ballPhysics.velocity = new Vector3(0, 0.2f, 0);
    }
    public bool GetSwing()
    {
        return isSwinging;
    }
    public void StartSwing(float swingForce)
    {
        force = swingForce;
        if (Time.time - lastServe < 0.5) // Get more power on the serve because there is no momentum from a bounce
        {
            force += 5;
        }
        isSwinging = true;
        lastSwing = Time.time;
    }
}
