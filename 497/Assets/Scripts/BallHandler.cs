using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHandler : MonoBehaviour
{
    bool isSwinging = false;
    bool hit = false;
    float lastSwing;
    public GameObject ball;
    private Rigidbody ballPhysics;
    public GameObject racket;
    // Start is called before the first frame update
    void Start()
    {
        lastSwing = 0;
        ballPhysics = ball.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Time.time - lastSwing > 1)
        {
            isSwinging = false;
            hit = false;
        }
        if (Input.GetMouseButton(0) && !isSwinging)
        {
            isSwinging = true;
            lastSwing = Time.time;
        }
        if (isSwinging && !hit)
        {
            bool isInFront = ballPhysics.position.x >= racket.transform.position.x;
            bool isCloseEnough = Vector3.Distance(racket.transform.position, ballPhysics.position) < 1;
            if (isInFront && isCloseEnough)
            {
                hit = true;
                HitBall();
            }
        }
    }

    void HitBall()
    {
        Vector3 currVelocity = ballPhysics.velocity;
        Vector3 normalVector = racket.transform.rotation * Vector3.right; // will probably have to change later
        //Debug.Log(normalVector);
        ballPhysics.velocity = Vector3.Reflect(ballPhysics.velocity, normalVector);
        ballPhysics.velocity *= 0.8f;
        ballPhysics.AddForce(10, 3, 0, ForceMode.VelocityChange);
    }
}
