using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject ball;

    [SerializeField] private float racketOffset;
    [SerializeField] private float playerOffset;
    [SerializeField] private float smoothSpeed;

    private void Start()
    {

    }
    private void FixedUpdate() //temporary player follows ball movement; issue: player is greedy no bueno kinda gross 
    {
        Vector3 ballPosition = ball.transform.position;
        //Vector3 velocityDirection = ball.GetComponent<Rigidbody>().velocity.normalized;
        //Debug.Log((Mathf.Atan2(velocityDirection.x, velocityDirection.y) * Mathf.Rad2Deg * Mathf.Sign(velocityDirection.y)));
        transform.position = Vector3.Lerp(transform.position, new Vector3(ballPosition.x + (playerOffset * Mathf.Sign(transform.position.x)), transform.position.y, ballPosition.z + (racketOffset * -Mathf.Sign(transform.position.x))), smoothSpeed);
    }
}
