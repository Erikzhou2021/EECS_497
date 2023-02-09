using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Boundary
{
    public float left; // horizontal is z - left is positive and right is negative (might change later but im lazy so i will suffer unintuitiveness for now)
    public float right;
    public float top; // vertical is x 
    public float bottom;
}

public class BallBoundary : MonoBehaviour
{
    [SerializeField] private Boundary boundary;
    private bool touchedGround = false;
    private bool bouncedInOpponentCourtOnce = false;

    public GameObject outText;
    public float textOffset;

    private void Update()
    {
        if (OutofBounds())
        {
            Debug.Log("ball out of bounds");
        }
        if (OutofBounds() && touchedGround) // i dont know if i want to put the out sign at the first position or follow the ball 
        {
            StartCoroutine(OutBounds());
        }
    }
    IEnumerator OutBounds()
    {
        outText.SetActive(true);
        Vector3 ballOutPosition = transform.position;
        outText.transform.position = new Vector3(ballOutPosition.x, ballOutPosition.y + textOffset, ballOutPosition.z);
        yield return new WaitForSeconds(3f);
        //outText.SetActive(false);
        //update score, start new round 
    }
    private bool OutofBounds()
    {
        return transform.position.z < boundary.left || transform.position.z > boundary.right
            || transform.position.x > boundary.top || transform.position.x < boundary.bottom;
    }
    private int InPlayerCourt() // returns which player's court the ball is in; 
    {
        if(transform.position.x < 0)
        {
            return 0; // in player 1's court 
        }
        else
        {
            return 1;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            touchedGround = true;
            //if(OutofBounds() && InPlayerCourt() == 0) && bounced once in oppnent court
            //{
            //    GameManager.Instance.player2.GetComponent<Player>().AddScore();
            //}
            //if (OutofBounds() && InPlayerCourt() == 1) && bounced once in oppnent court 
            //{
            //    GameManager.Instance.player1.GetComponent<Player>().AddScore();
            //}
        }
    }
}
