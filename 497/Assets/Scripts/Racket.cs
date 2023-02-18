using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Racket : MonoBehaviour
{
    BallHandler bh;
    public TextMeshProUGUI debugText;

    void Start()
    {
        bh = gameObject.GetComponent<BallHandler>();
        Input.gyro.enabled = true;
    }

    void FixedUpdate()
    {
        //transform.rotation = Input.gyro.attitude;
        // idk if this clamp does shit
        float rotx = Mathf.Clamp(Input.acceleration.x, -45, 0);
        float roty = Mathf.Clamp(Input.acceleration.y, -45, 0);
        float rotz = Mathf.Clamp(Input.acceleration.z, -20, 20);
        Quaternion newRot = new Quaternion(-rotz, roty, -rotx, 0);        //transform.rotation = newRot;

        if(transform.parent.GetComponent<Player>().playerTeam == 1)
        {
            newRot = new Quaternion(-rotz, -roty, -rotx, 0);
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, newRot, 2f * Time.deltaTime);

        //debugText.text = Input.gyro.userAcceleration.y.ToString();
        if (Input.gyro.userAcceleration.magnitude > 3)
        {
            bh.StartSwing();
        }
        if (bh.GetSwing())
        {
            transform.Rotate(0,-7.2f,0);
        }
        else if(Input.gyro.userAcceleration.y > 0.5) // need to make this take multiple frames to detect
        {
            bh.Serve();
        }
    }
}
