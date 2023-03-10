using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Mirror
{
    public class Racket : NetworkBehaviour
    {

        BallHandler bh;
        public TextMeshPro debugText;
        public bool isPlayer = false;
        private Rigidbody ballPhysics;
        public float threshold = 0.25f;
        GameObject racket;
        Transform reference;
        float lastSwitch = 0;

        public float rotationSpeed = 300f;

        void Start()
        {
            StartCoroutine(ballPhys());

            racket = transform.GetChild(0).gameObject;
            reference = racket.transform.GetChild(0);
            lastSwitch = Time.time;
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
            Player p = GetComponent<Player>();
            if (!bh)
            {
                Debug.Log("single player test");
                bh = racket.GetComponent<BallHandler>();
                ballPhysics = bh.ball.GetComponent<Rigidbody>();
            }

            if (isLocalPlayer)
            {

                Quaternion newRot = Input.gyro.attitude;
                // change from a right handed coordinate system to left handed
                float temp = newRot.y;
                newRot.x *= -1;
                newRot.y = -newRot.z;
                newRot.z = -temp;

                newRot *= Quaternion.Euler(-90,180,90); // offset to make the racket start in the correct spot
                racket.transform.localRotation = Quaternion.Slerp(racket.transform.rotation, newRot, 5f * Time.deltaTime);

                debugText.text = Input.gyro.attitude.eulerAngles.ToString();
                float upForce = Vector3.Dot(Input.gyro.userAcceleration, Vector3.Normalize(Input.gyro.gravity));
                if (Input.gyro.userAcceleration.magnitude > 2 && !bh.GetSwing())
                {
                    float force = Input.acceleration.magnitude - 2;
                    force *= 4;
                    force += 5;
                    force = Mathf.Clamp(force, 5, 18);

                    bh.StartSwing(force);
                }
                if (bh.GetSwing())
                {
                    StartCoroutine(EnableTrail());
                    racket.transform.RotateAround(transform.position, new Vector3(0, 1, 0), -rotationSpeed * Time.deltaTime);
                }
                else if (upForce > 0.3f && GameManager.Instance.state == GameState.Serve) // need to make this take multiple frames to detect
                {
                    // lmao dont look, resetting all variables 
                    BallBoundary.Instance.touchedGroundOnceOut = false;
                    BallBoundary.Instance.scoreStop = false;
                    BallBoundary.Instance.bouncedInOpponentCourtOnce = false;
                    BallBoundary.Instance.playerTurn = GetComponent<Player>().playerTeam;
                    ballPhysics.GetComponent<Rigidbody>().useGravity = true;

                    bh.Serve();
                }
                else if (p.forehand && Time.time - lastSwitch > 0.25 && reference.position.z - racket.transform.position.z > -threshold)
                {
                    Debug.Log("forehand to backhand");
                    p.forehand = false;
                    DoSwing();
                }
                else if (!p.forehand && Time.time - lastSwitch > 0.25 && reference.position.z - racket.transform.position.z > threshold)
                {
                    Debug.Log("backhand to forehand");
                    p.forehand = true;
                    DoSwing();
                }
            }
        }
        void DoSwing()
        {
            Vector3 t = racket.transform.localPosition;
            racket.transform.localPosition = new Vector3(t.x, t.y, -t.z);
            lastSwitch = Time.time;
        }
        IEnumerator EnableTrail()
        {
            yield return new WaitForEndOfFrame();
            racket.GetComponent<TrailRenderer>().emitting = true;
        }
    }
}
