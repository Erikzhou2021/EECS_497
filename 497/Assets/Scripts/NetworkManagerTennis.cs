using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

namespace Mirror
{
    // Custom NetworkManager that simply assigns the correct racket positions when
    // spawning players. The built in RoundRobin spawn method wouldn't work after
    // someone reconnects (both players would be on the same side).
    [AddComponentMenu("")]
    public class NetworkManagerTennis : NetworkManager
    {
        public Transform leftPlayerSpawn;
        public Transform rightPlayerSpawn;
        GameObject ball;
        public GameObject[] cameras;
        GameObject player;

        public bool debug = false;


        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // add player at correct spawn position
            Transform start = numPlayers == 0 ? leftPlayerSpawn : rightPlayerSpawn;
            if(numPlayers == 0)
            {
                player = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Player"), start.position, start.rotation);

            } else
            {
               player = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Player2"), start.position, start.rotation);
            }
            
            NetworkServer.AddPlayerForConnection(conn, player);
            GameManager.Instance.players.Add(player);
            player.GetComponent<Player>().playerTeam = numPlayers - 1;

            cameras[numPlayers - 1].GetComponent<CameraFollow>().setTarget(player.transform);

            // spawn ball if two players
            if (numPlayers == 1)
            {
                ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
                NetworkServer.Spawn(ball);
                GameManager.Instance.ball = ball;

                GameManager.Instance.StartGame();

            }

            if (GameObject.Find("Ball(Clone)"))
            {
                Debug.Log("balle exisgs!");
            }
            // for debuging only
            if (debug)
            {
                GameObject player2 = GameObject.Find("Player2");
                GameManager.Instance.players.Add(player2);
            }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            // destroy ball
            if (ball != null)
                NetworkServer.Destroy(ball);

            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }
    }
}