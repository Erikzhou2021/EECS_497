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
        float lastSwitch = 0;
        float lastSwing;
        Player p;
        bool isSwinging = false;
        bool isSwingingBack = false;
        public float swingTime = 0.25f;

        public GameObject hitEffect; 

        public float rotationSpeed = 250f;

        public AudioClip racketMiss;
        public AudioClip racketBounce;
        void Start()
        {
            lastSwing = 0;
            p = GetComponent<Player>();
            StartCoroutine(ballPhys());
            
            racket = transform.GetChild(0).gameObject;
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
            if (!bh)
            {
                Debug.Log("single player test");
                bh = racket.GetComponent<BallHandler>();
                ballPhysics = GameManager.Instance.ball.GetComponent<Rigidbody>();
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
                //racket.transform.localRotation = Quaternion.Slerp(racket.transform.rotation, newRot, 5f * Time.deltaTime);
                if (Quaternion.Angle(racket.transform.rotation, newRot) > 3f)
                {
                    racket.transform.rotation = newRot;
                }
                //debugText.text = Input.gyro.attitude.eulerAngles.ToString();
                float upForce = Vector3.Dot(Input.gyro.userAcceleration, Vector3.Normalize(Input.gyro.gravity));

                if (Time.time - lastSwing >= swingTime)
                {
                    isSwinging = false;
                }

                if (isSwinging)
                {
                    StartCoroutine(EnableTrail());
                    racket.transform.RotateAround(transform.position, new Vector3(0, 1, 0), -rotationSpeed * Time.deltaTime);
                    //if (!AudioManager.Instance.EffectsSource.isPlaying)
                    //{
                    if(!(AudioManager.Instance.EffectsSource.isPlaying && AudioManager.Instance.EffectsSource.clip == racketBounce))
                        AudioManager.Instance.Play(racketMiss);
                    //}
                }
                else if (isSwingingBack)
                {
                    racket.GetComponent<TrailRenderer>().emitting = false;
                    isSwingingBack = false;
                    float rotateAmount = Mathf.Abs(Mathf.Cos(rotationSpeed * Time.deltaTime * Mathf.Deg2Rad));
                    if (p.forehand && Mathf.Abs(racket.transform.localPosition.z + 1.5f) <= rotateAmount)
                    {
                        racket.transform.localPosition = new Vector3(0, 0, -1.5f);
                    }
                    else if (!p.forehand && Mathf.Abs(racket.transform.localPosition.z - 1.5f) <= rotateAmount)
                    {
                        racket.transform.localPosition = new Vector3(0, 0, 1.5f);
                    }
                    else
                    {
                        isSwingingBack = true;
                        racket.transform.RotateAround(transform.position, new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime);
                    }
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
                else if (!isSwingingBack && Input.gyro.userAcceleration.magnitude > 2) // need to test if this stops you from spamming swing
                {
                    float force = Input.acceleration.magnitude - 2;
                    force *= 4;
                    force += 5;
                    force = Mathf.Clamp(force, 5, 18);

                    //if (!GameManager.Instance.pauseGame)
                        bh.StartSwing(force);
                }
                else if (!isSwingingBack && /*GameManager.Instance.state != GameState.Serve &&*/ Time.time - lastSwitch > 0.25)
                {
                    if (p.forehand && Vector3.Dot(transform.right, racket.transform.forward) < -0.5) // replace with threshold later
                    {
                        Debug.Log("forehand to backhand");
                        Debug.Log(Vector3.Dot(transform.right, racket.transform.forward));
                        p.forehand = false;
                        Switch();
                    }
                    else if (!p.forehand && Vector3.Dot(transform.right, racket.transform.forward) > 0.5)
                    {
                        Debug.Log("backhand to forehand");
                        Debug.Log(Vector3.Dot(transform.right, racket.transform.forward));
                        p.forehand = true;
                        Switch();
                    }
                }
                
            }
        }
        public bool GetSwing()
        {
            return isSwinging;
        }
        public bool GetSwingBack()
        {
            return isSwingingBack;
        }
        public void SetSwing()
        {
            lastSwing = Time.time;
            isSwinging = true;
            isSwingingBack = true;
            // teleport racket into wound up position
            racket.transform.RotateAround(transform.position, new Vector3(0, 1, 0), 5f* rotationSpeed * Time.fixedDeltaTime);
            GameObject effect = Instantiate(hitEffect, racket.transform.position, Quaternion.identity);
            effect.transform.SetParent(GameObject.Find("WorldCanvas").transform);
        }

        void Switch()
        {
            Vector3 t = racket.transform.localPosition;
            racket.transform.localPosition = new Vector3(t.x, t.y, -t.z);
            lastSwitch = Time.time;
            rotationSpeed *= -1;
        }
        IEnumerator EnableTrail()
        {
            yield return new WaitForEndOfFrame();
            racket.GetComponent<TrailRenderer>().emitting = true;
        }
    }
}
