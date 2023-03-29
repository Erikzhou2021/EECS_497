using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mirror
{
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

        [Header("Game")]
        [SerializeField] private NetworkGamePlayerTennis gamePlayerPrefab = null;
        [SerializeField] private GameObject playerSpawnSystem = null;
        [SerializeField] private GameObject roundSystem = null;

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;
        public static event Action<NetworkConnection> OnServerReadied;
        public static event Action OnServerStopped;

        public List<NetworkRoomPlayerTennis> RoomPlayers { get; } = new List<NetworkRoomPlayerTennis>();
        public List<NetworkGamePlayerTennis> GamePlayers { get; } = new List<NetworkGamePlayerTennis>();

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

            if (SceneManager.GetActiveScene().path != menuScene)
            {
                conn.Disconnect();
                return;
            }
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            if (SceneManager.GetActiveScene().path == menuScene)
            {
                bool isLeader = RoomPlayers.Count == 0;

                NetworkRoomPlayerTennis roomPlayerInstance = Instantiate(roomPlayerPrefab);

                roomPlayerInstance.IsLeader = isLeader;

                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            if (conn.identity != null)
            {
                var player = conn.identity.GetComponent<NetworkRoomPlayerTennis>();

                RoomPlayers.Remove(player);

                NotifyPlayersOfReadyState();
            }

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            OnServerStopped?.Invoke();

            RoomPlayers.Clear();
            GamePlayers.Clear();
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

        public void StartGame()
        {
            if (SceneManager.GetActiveScene().path == menuScene)
            {
                if (!IsReadyToStart()) { return; }

            }
        }

        public override void ServerChangeScene(string newSceneName)
        {
            // From menu to game
            if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("Scene_Map"))
            {
                for (int i = RoomPlayers.Count - 1; i >= 0; i--)
                {
                    var conn = RoomPlayers[i].connectionToClient;
                    var gameplayerInstance = Instantiate(gamePlayerPrefab);
                    gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                    NetworkServer.Destroy(conn.identity.gameObject);

                    NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
                }
            }

            base.ServerChangeScene(newSceneName);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (sceneName.StartsWith("Scene_Map"))
            {
                GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
                NetworkServer.Spawn(playerSpawnSystemInstance);

                GameObject roundSystemInstance = Instantiate(roundSystem);
                NetworkServer.Spawn(roundSystemInstance);
            }
        }

        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);

            OnServerReadied?.Invoke(conn);
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
                if (spawnPrefabs.Find(prefab => prefab.name == "Player"))
                {
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
