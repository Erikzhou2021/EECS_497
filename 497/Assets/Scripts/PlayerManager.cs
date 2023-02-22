using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

    //public class Player : NetworkBehaviour
    //{
    //    public float speed = 30;
    //    public Rigidbody2D rigidbody2d;

    //    // need to use FixedUpdate for rigidbody
    //    void FixedUpdate()
    //    {
    //        // only let the local player control the racket.
    //        // don't control other player's rackets
    //        if (isLocalPlayer)
    //            rigidbody2d.velocity = new Vector2(0, Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;
    //    }
    //}


namespace Mirror
{
        public class PlayerManager : NetworkBehaviour
    {
        BallHandler bh;
        public TextMeshProUGUI debugText;
        public bool isPlayer = false;
        private Rigidbody ballPhysics;

        public override void OnStartServer()
        {
            base.OnStartServer();
            Debug.Log("Server started");
        }
        // Start is called before the first frame update
        void Start()
        {
            bh = gameObject.GetComponent<BallHandler>();
            Input.gyro.enabled = true;
            ballPhysics = bh.ball.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void FixedUpdate()
        {
            if (!isPlayer)
            {
                return;
            }
            //transform.rotation = Input.gyro.attitude;
            // idk if this clamp does shit
            float rotx = Mathf.Clamp(Input.acceleration.x, -45, 0);
            float roty = Mathf.Clamp(Input.acceleration.y, -45, 0);
            float rotz = Mathf.Clamp(Input.acceleration.z, -20, 20);
            Quaternion newRot = new Quaternion(-rotz, roty, -rotx, 0);        //transform.rotation = newRot;

            if (transform.parent.GetComponent<Player>().playerTeam == 1)
            {
                newRot = new Quaternion(-rotz, -roty, -rotx, 0);
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, newRot, 2f * Time.deltaTime);

            //debugText.text = Input.gyro.userAcceleration.y.ToString();
            if (Input.gyro.userAcceleration.magnitude > 2)
            {
                float force = Input.gyro.userAcceleration.magnitude - 2;
                force *= 4;
                force += 5;
                force = Mathf.Clamp(force, 5, 18);
                debugText.text = force.ToString();
                bh.StartSwing(force);
            }
            if (bh.GetSwing())
            {
                transform.Rotate(0, -7.2f, 0);
            }
            else if (Input.gyro.userAcceleration.y > 0.3) // need to make this take multiple frames to detect
            {
                // lmao dont look, resetting all variables 
                BallBoundary.Instance.touchedGroundOnceOut = false;
                BallBoundary.Instance.scoreStop = false;
                BallBoundary.Instance.isAnyServing = true;
                BallBoundary.Instance.bouncedInOpponentCourtOnce = false;
                BallBoundary.Instance.playerTurn = 0;
                ballPhysics.GetComponent<Rigidbody>().useGravity = true;

                bh.Serve();
            }
        }
    }
}
