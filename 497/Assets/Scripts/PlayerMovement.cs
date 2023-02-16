using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject ball;

    [SerializeField] private float racketOffset;
    [SerializeField] private float playerOffset;
    [SerializeField] private float movementSpeed;

    private void Start()
    {

    }
    private void FixedUpdate()  
    {
        Vector3 ballPosition = ball.transform.position;
        //Vector3 velocityDirection = ball.GetComponent<Rigidbody>().velocity.normalized;
        //Debug.Log((Mathf.Atan2(velocityDirection.x, velocityDirection.y) * Mathf.Rad2Deg * Mathf.Sign(velocityDirection.y)));
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, ballPosition.z + (racketOffset * -Mathf.Sign(transform.position.x)));
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
    } //ballPosition.x + (playerOffset * Mathf.Sign(transform.position.x))
}
