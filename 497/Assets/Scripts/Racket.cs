using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Mirror
{
    public class Racket : NetworkBehaviour
    {

        //public override void OnStartAuthority()
        //{
        //    base.OnStartAuthority();

        //    UnityEngine.InputSystem.PlayerInput playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        //    playerInput.enabled = true;
        //}

        BallHandler bh;
        public TextMeshProUGUI debugText;
        public bool isPlayer = false;
        private Rigidbody ballPhysics;
        GameObject racket;

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
                Debug.Log("It is local player");
                // idk if this clamp does shit
                float rotx = Mathf.Clamp(Input.acceleration.x, -45, 0);
                float roty = Mathf.Clamp(Input.acceleration.y, -45, 0);
                float rotz = Mathf.Clamp(Input.acceleration.z, -20, 20);
                Quaternion newRot = new Quaternion(-rotz, roty, -rotx, 0);        //transform.rotation = newRot;


                newRot = new Quaternion(-rotz, -roty, -rotx, 0);
                Debug.Log(racket.name);
                racket.transform.rotation = Quaternion.Slerp(racket.transform.rotation, newRot, 2f * Time.deltaTime);

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
                    racket.transform.Rotate(0, -7.2f, 0);
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
    }

}
