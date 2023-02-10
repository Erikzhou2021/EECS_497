using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racket : MonoBehaviour
{
    BallHandler bh;
    // Start is called before the first frame update
    void Start()
    {
        bh = gameObject.GetComponent<BallHandler>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (bh.getSwing())
        {
            transform.Rotate(0,-7.2f,0);
        }
    }
}
