using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Racket : MonoBehaviour
{
    BallHandler bh;
    public TextMeshProUGUI debugText;
    // Start is called before the first frame update
    void Start()
    {
        bh = gameObject.GetComponent<BallHandler>();
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.rotation = Input.gyro.attitude;
        debugText.text = Input.gyro.userAcceleration.y.ToString();
        if (Input.gyro.userAcceleration.magnitude > 3)
        {
            bh.SetSwing();
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
