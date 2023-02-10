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

    void Start()
    {
        lastSwing = 0;
        ballPhysics = ball.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Time.time - lastSwing >= 1)
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
            bool isInFront = ballPhysics.position.x >= transform.position.x;
            bool isCloseEnough = Vector3.Distance(transform.position, ballPhysics.position) < 1;
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
        Vector3 normalVector = transform.rotation * Vector3.right; // will probably have to change later
        //Debug.Log(normalVector);
        ballPhysics.velocity = Vector3.Reflect(ballPhysics.velocity, normalVector);
        ballPhysics.velocity *= 0.8f;

        float force = 12;
        int aimPos = 0;
        float height = 1;
        Vector3 aim;
        //straight force
        //aim = new Vector3(10,0,0);

        switch (aimPos)
        {
            case -1:
                // aim toward the mid left
                aim = Vector3.Normalize(new Vector3(-ballPhysics.position.x, 0, -ballPhysics.position.z + 2));
                break;
            case -2:
                // aim toward the far left
                aim = Vector3.Normalize(new Vector3(-ballPhysics.position.x, 0, -ballPhysics.position.z + 4));
                break;
            case 1:
                // aim toward the mid right
                aim = Vector3.Normalize(new Vector3(-ballPhysics.position.x, 0, -ballPhysics.position.z - 2));
                break;
            case 2:
                // aim toward the far right
                aim = Vector3.Normalize(new Vector3(-ballPhysics.position.x, 0, -ballPhysics.position.z - 4));
                break;
            default:
                // aimbot toward center
                aim = Vector3.Normalize(Vector3.MoveTowards(-ballPhysics.position, Vector3.zero, 1));
                aim = Vector3.ProjectOnPlane(aim, Vector3.up);
                break;
        }   
        ballPhysics.AddForce(aim*force, ForceMode.VelocityChange);
        
        ballPhysics.AddForce(0, height*4 + 4,0, ForceMode.VelocityChange);
    }
    public bool getSwing()
    {
        return isSwinging;
    }
}
