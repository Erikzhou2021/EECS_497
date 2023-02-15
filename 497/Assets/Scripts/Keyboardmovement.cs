using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboardmovement : MonoBehaviour
{
    public GameObject ball;
    private Rigidbody ballPhysics;
    public GameObject racket;
    public float playerSpeed;
    // Start is called before the first frame update
    void Start()
    {
        ballPhysics = ball.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey("w") || Input.GetKey("up"))
        {
            //Debug.Log("Hi");
            gameObject.transform.Translate(playerSpeed,0,0);
        }
        if (Input.GetKey("a") || Input.GetKey("left"))
        {
            gameObject.transform.Translate(0, 0, playerSpeed);
        }
        if (Input.GetKey("s") || Input.GetKey("down"))
        {
            gameObject.transform.Translate(-playerSpeed, 0, 0);
        }
        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            gameObject.transform.Translate(0, 0, -playerSpeed);
        }
        if (Input.GetKey("r"))
        {
            // lmao dont look, resetting all variables 
            BallBoundary.Instance.touchedGroundOnceOut = false;
            BallBoundary.Instance.scoreStop = false;
            BallBoundary.Instance.isAnyServing = true;
            BallBoundary.Instance.bouncedInOpponentCourtOnce = false;
            BallBoundary.Instance.playerTurn = 0;
            ballPhysics.GetComponent<Rigidbody>().useGravity = true;

            ballPhysics.position = racket.transform.position + new Vector3(0.3f, 1.5f, 0);
            ballPhysics.velocity = new Vector3(0, 0.2f, 0);
        }
        /*if (Input.GetKey("q"))
        {
            if (ballPhysics.position.x >= racket.transform.position.x && Vector3.Distance(racket.transform.position, ballPhysics.position) < 1)
            {
                ballPhysics.velocity += new Vector3(1, 0.3f, 0);
                Debug.Log("reeee");
            }
            Debug.Log(Vector3.Distance(racket.transform.position, ballPhysics.position));
        }*/
        if (Input.GetKey("q"))
        {
            gameObject.transform.Rotate(0, -0.5f, 0);
        }
        if (Input.GetKey("e"))
        {
            gameObject.transform.Rotate(0, 0.5f, 0);
        }
        if (Input.GetKey("f"))
        {
            Vector3 force = Vector3.MoveTowards(ballPhysics.position, racket.transform.position, 1);
            force.Normalize();
            force = force * 10;
            ballPhysics.AddForce(force);
        }
    }
}
