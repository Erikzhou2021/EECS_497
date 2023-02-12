using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    // player boundary == half of court 
    // if player 0, boundary = 0, -5
    // if player 1, boundary = 0, 5 
    public Boundary playerBoundary;
    public float fieldX;
    public float fieldY;

    private void Start()
    {
        if(GetComponent<Player>().playerTeam == 0)
        {
            playerBoundary.top = 0;
            playerBoundary.bottom = -fieldY;
            playerBoundary.left = -fieldX;
            playerBoundary.right = fieldX;
        }
        else
        {
            playerBoundary.top = fieldY;
            playerBoundary.bottom = 0;
            playerBoundary.left = -fieldX;
            playerBoundary.right = fieldX;
        }
    }
    private void Update()
    {
        //prevent player from going out of bounds 
        if(transform.position.z < playerBoundary.left)
        {
            transform.position =  new Vector3(transform.position.x, transform.position.y, playerBoundary.left);
        }
        if (transform.position.z > playerBoundary.right)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, playerBoundary.right);
        }
        if (transform.position.x < playerBoundary.bottom)
        {
            transform.position = new Vector3(playerBoundary.bottom, transform.position.y, transform.position.z);
        }
        if (transform.position.x > playerBoundary.top)
        {
            transform.position = new Vector3(playerBoundary.top, transform.position.y, transform.position.z);
        }
    }
}
