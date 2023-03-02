using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Mirror
{
    public class Racket : NetworkBehaviour
    {

        BallHandler bh;
        public TextMeshProUGUI debugText;
        public bool isPlayer = false;
        private Rigidbody ballPhysics;
        GameObject racket;

        public float rotationSpeed = 300f;

        void Start()
        {
            StartCoroutine(ballPhys());

            racket = transform.GetChild(0).gameObject;
        }

        IEnumerator ballPhys() // have to use coroutine bc everything instantiated
        {
            yield return new WaitForSeconds(2);
            bh = racket.GetComponent<BallHandler>();
            ballPhysics = bh.ball.GetComponent<Rigidbody>();
            Input.gyro.enabled = true;
            racket.GetComponent<TrailRenderer>().emitting = false;
        }

        void FixedUpdate()
        {
            //if (!isPlayer)
            //{
            //    Debug.Log("yoyoyo");
            //    return;
            //}
            //transform.rotation = Input.gyro.attitude;

            if (isLocalPlayer)
            {
                
                //Debug.Log("It is local player");
                // idk if this clamp does shit
                float rotx = Mathf.Clamp(Input.acceleration.x, -45, 0);
                float roty = Mathf.Clamp(Input.acceleration.y, -45, 0);
                float rotz = Mathf.Clamp(Input.acceleration.z, -20, 20);


                Quaternion newRot = new Quaternion(-rotx, -rotz, -roty, 0);
                newRot = Input.gyro.attitude;
                float temp = newRot.y;
                newRot.x *= -1;
                newRot.y = -newRot.z;
                newRot.z = -temp;
                newRot *= Quaternion.Euler(-90,180,90);
                //Debug.Log(racket.name);
                racket.transform.localRotation = Quaternion.Slerp(racket.transform.rotation, newRot, 2f * Time.deltaTime);
                
                

                //debugText.text = Input.gyro.userAcceleration.y.ToString();
                if (Input.gyro.userAcceleration.magnitude > 2)
                {
                    float force = Input.gyro.userAcceleration.magnitude - 2;
                    force *= 4;
                    force += 5;
                    force = Mathf.Clamp(force, 5, 18);
                    //debugText.text = force.ToString();
                    Debug.Log("startswing");
                    bh.StartSwing(force);
                }
                if (bh.GetSwing())
                {
                    //racket.transform.Rotate(0, -7.2f, 0);
                    StartCoroutine(EnableTrail());
                    racket.transform.RotateAround(transform.position, new Vector3(0, 1, 0), -rotationSpeed * Time.deltaTime);
                }
                else if (Input.gyro.userAcceleration.y > 0.3 && GameManager.Instance.state == GameState.Serve) // need to make this take multiple frames to detect
                {
                    // lmao dont look, resetting all variables 
                    BallBoundary.Instance.touchedGroundOnceOut = false;
                    BallBoundary.Instance.scoreStop = false;
                    BallBoundary.Instance.bouncedInOpponentCourtOnce = false;
                    BallBoundary.Instance.playerTurn = 0;
                    ballPhysics.GetComponent<Rigidbody>().useGravity = true;

                    bh.Serve();
                }
            }
        }
        IEnumerator EnableTrail()
        {
            yield return new WaitForEndOfFrame();
            racket.GetComponent<TrailRenderer>().emitting = true;
        }
    }
}
