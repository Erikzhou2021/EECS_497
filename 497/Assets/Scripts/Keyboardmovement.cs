using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

    public class Keyboardmovement : NetworkBehaviour
    {
        public GameObject ball;
        private Rigidbody ballPhysics;
        private BallHandler bh;
        public GameObject racket;
        public float playerSpeed;
        float resetCounter;
        Racket r;
        // Start is called before the first frame update
        void Start()
        {
            r = GetComponent<Racket>();
            resetCounter = 0;
            ballPhysics = ball.GetComponent<Rigidbody>();
            bh = racket.GetComponent<BallHandler>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (IsOwner)
            {
                if (resetCounter > 0)
                {
                    resetCounter++;
                }
                if (resetCounter > 5)
                {
                    // lmao dont look, resetting all variables 
                    BallBoundary.Instance.touchedGroundOnceOut = false;
                    BallBoundary.Instance.scoreStop = false;
                    BallBoundary.Instance.bouncedInOpponentCourtOnce = false;
                    BallBoundary.Instance.playerTurn = 0;
                    resetCounter = 0;
                    bh.Serve();
                }
                if (Input.GetKey("w") || Input.GetKey("up"))
                {
                    //Debug.Log("Hi");
                    gameObject.transform.Translate(playerSpeed, 0, 0, Space.World);
                }
                if (Input.GetKey("a") || Input.GetKey("left"))
                {
                    gameObject.transform.Translate(0, 0, playerSpeed, Space.World);
                }
                if (Input.GetKey("s") || Input.GetKey("down"))
                {
                    gameObject.transform.Translate(-playerSpeed, 0, 0, Space.World);
                }
                if (Input.GetKey("d") || Input.GetKey("right"))
                {
                    gameObject.transform.Translate(0, 0, -playerSpeed, Space.World);
                }
                if (Input.GetKey("r"))
                {
                    resetCounter = 1; // just delaying the serve for a few frames so the other code can run
                    GameManager.Instance.state = GameState.Serve;
                }
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
                if (Input.GetMouseButton(0) && !r.GetSwing() && !r.GetSwingBack())
                {
                    if(!GameManager.Instance.pauseGame)
                        bh.StartSwing(5);
                }
            }
        }
    }
