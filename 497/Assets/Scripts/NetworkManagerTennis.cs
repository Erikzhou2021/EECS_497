using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Mirror;

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
        Vector3 leftPlayerSpawn;
        Vector3 rightPlayerSpawn;
        GameObject ball;
        public GameObject[] cameras;
        GameObject player;
        public static NetworkManagerTennis instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }

        }

        [SerializeField] private int minPlayers = 2;
        [Scene] [SerializeField] private string menuScene = string.Empty;

        [Header("Room")]
        [SerializeField] private NetworkRoomPlayerTennis roomPlayerPrefab = null;

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;

        public List<NetworkRoomPlayerTennis> RoomPlayers { get; } = new List<NetworkRoomPlayerTennis>();

        public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

        public override void OnStartClient()
        {
            var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

            foreach (var prefab in spawnablePrefabs)
            {
                NetworkClient.RegisterPrefab(prefab);
            }
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();

            OnClientConnected?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();

            OnClientDisconnected?.Invoke();
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            if (numPlayers >= maxConnections)
            {
                conn.Disconnect();
                return;
            }

            if (SceneManager.GetActiveScene().name != menuScene)
            {
                conn.Disconnect();
                return;
            }
        }



        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            if (SceneManager.GetActiveScene().name == menuScene)
            {
                bool isLeader = RoomPlayers.Count == 0;

                NetworkRoomPlayerTennis roomPlayerInstance = Instantiate(roomPlayerPrefab);

                roomPlayerInstance.IsLeader = isLeader;

                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            }
        //    // add player at correct spawn position
        //    Transform start = numPlayers == 0 ? leftPlayerSpawn : rightPlayerSpawn;
        //    if(numPlayers == 0)
        //    {
        //        player = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Player"), start.position, start.rotation);

        //    } else
        //    {
        //       player = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Player2"), start.position, start.rotation);
        //    }

        //    NetworkServer.AddPlayerForConnection(conn, player);
        //    GameManager.Instance.players.Add(player);
        //    player.GetComponent<Player>().playerTeam = numPlayers - 1;

        //    cameras[numPlayers - 1].GetComponent<CameraFollow>().setTarget(player.transform);

        //    // spawn ball if two players
        //    if (numPlayers == 1)
        //    {
        //        ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
        //        NetworkServer.Spawn(ball);
        //        GameManager.Instance.ball = ball;
        //    }

        //    if (GameObject.Find("Ball(Clone)"))
        //    {
        //        Debug.Log("balle exisgs!");
        //    }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            // destroy ball
            if (ball != null)
                NetworkServer.Destroy(ball);

            if (conn.identity != null)
            {
                var player = conn.identity.GetComponent<NetworkRoomPlayerTennis>();

                RoomPlayers.Remove(player);

                NotifyPlayersOfReadyState();
            }

            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            RoomPlayers.Clear();
        }

        public void NotifyPlayersOfReadyState()
        {
            foreach (var player in RoomPlayers)
            {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }

        private bool IsReadyToStart()
        {
            if (numPlayers < minPlayers) { return false; }

            foreach (var player in RoomPlayers)
            {
                if (!player.IsReady) { return false; }
            }

            return true;
        }

        public override void ServerChangeScene(string newSceneName)
        {

            //if (SceneManager.GetActiveScene().name == "lobbyScene")
            //{
            //    Debug.Log(numPlayers);
            //    Transform start = numPlayers == 0 ? leftPlayerSpawn : rightPlayerSpawn;
            //    //    for (int i = MatchMaker.matches[1].players.Count - 1; i >= 0; i--)
            //    //    {

            //    //    }
            //}
            base.ServerChangeScene(newSceneName);
        }

        public void Redo(NetworkConnectionToClient conn)
        {
            Scene s = SceneManager.GetActiveScene();
            Debug.Log(s.name);
            //ServerChangeScene("multiplayer");
            Scene sce = SceneManager.GetActiveScene();
            Debug.Log(sce.name);
            Debug.Log("time to change chara by redoing");
            Debug.Log(numPlayers);
           

            leftPlayerSpawn = new Vector3(-8, 1, 0);
            rightPlayerSpawn = new Vector3(8, 1, 0);

            if (numPlayers == 1) //change this 2 later if server = 1player
            {
                if (spawnPrefabs.Find(prefab => prefab.name == "Player")) {
                    Debug.Log("yes exists");
                }
                Debug.Log(leftPlayerSpawn);
                Scene sc = SceneManager.GetActiveScene();
                Debug.Log(sc.name);
                player = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Player"), leftPlayerSpawn, Quaternion.identity);
                Scene scene = player.scene;
                Debug.Log(scene.name);
                Debug.Log("Player 1 gameobject instantiated");

            }
            else if (numPlayers == 2) //change this 3 later if server = 1player
            {
                player = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Player2"), rightPlayerSpawn, Quaternion.identity);
            }

            var reconn = conn;
            NetworkServer.Destroy(conn.identity.gameObject);
            NetworkServer.ReplacePlayerForConnection(reconn, player.gameObject);
            Debug.Log("successfully replaced chara");
            

            


        }
    }
}