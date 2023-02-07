using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboardmovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
        {
            //Debug.Log("Hi");
            gameObject.transform.Translate(0.01f,0,0);
        }
        if (Input.GetKey("a"))
        {
            gameObject.transform.Translate(0, 0, 0.01f);
        }
        if (Input.GetKey("s"))
        {
            gameObject.transform.Translate(-0.01f, 0, 0);
        }
        if (Input.GetKey("d"))
        {
            gameObject.transform.Translate(0, 0, -0.01f);
        }
    }
}
