using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncing : MonoBehaviour
{
    public float bounceSpeed = 5f;
    public float bounceHeight = 0.01f;

    private float startYPos;
    private void Awake()
    {
        startYPos = transform.position.y;
    }
    private void Update()
    {
        transform.position = new Vector3(transform.position.x, startYPos + (Mathf.Sin(Time.time * bounceSpeed) * bounceHeight), transform.position.z);
    }
}
