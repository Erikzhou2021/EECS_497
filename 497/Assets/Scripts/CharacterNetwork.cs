using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

namespace Mirror
{
    public class CharacterNetwork : NetworkBehaviour
    {

        // for lobby
        public static CharacterNetwork localPlayer;
        [SyncVar] public string matchID;
        [SyncVar] public int playerIndex;

        NetworkMatch networkMatch;
        //
        // Start is called before the first frame update
        void Start()
        {
            networkMatch = GetComponent<NetworkMatch>();
            if (isLocalPlayer)
            {
                localPlayer = this;
            } else
            {
                UILobby.instance.SpawnPlayerUIPrefab(this);
            }
            
        }
        //for join
        public void JoinGame(string _inputID)
        {
            CmdJoinGame(_inputID);
        }

        [Command]
        void CmdJoinGame(string _matchID)
        {
            matchID = _matchID;
            // if did manage to join game
            if (MatchMaker.instance.JoinGame(_matchID, this, out playerIndex)) //*
            {
                Debug.Log("Game joined successfully");
                networkMatch.matchId = _matchID.ToGuid();
                TargetJoinGame(true, _matchID);
            }
            else
            {
                Debug.Log("Join game failed");
                TargetJoinGame(false, _matchID);
            }
        }

        [TargetRpc]
        void TargetJoinGame(bool success, string _matchID)
        {
            Debug.Log("MatchID: " + matchID + "==" + _matchID.ToString());
            UILobby.instance.JoinSuccess(success);
        }

        //for host
        public void HostGame()
        {
            string matchID = MatchMaker.GetRandomMatchID();
            CmdHostGame(matchID);
        }

        [Command]
        void CmdHostGame(string _matchID)
        {
            matchID = _matchID;
            // if did manage to host game
            if (MatchMaker.instance.HostGame(_matchID, this, out playerIndex)) //*
            {
                Debug.Log("Game hosted successfully");
                networkMatch.matchId = _matchID.ToGuid();
                TargetHostGame(true, _matchID);
            } else
            {
                Debug.Log("Host game failed");
                TargetHostGame(false, _matchID);
            }
        }

        [TargetRpc]
        void TargetHostGame(bool success, string _matchID)
        {
            Debug.Log("MatchID: " + matchID + "==" + _matchID.ToString());
            UILobby.instance.HostSuccess(success);
        }

        //BEGIN MATCH
        public void BeginGame()
        {
            CmdBeginGame();
        }

        [Command]
        void CmdBeginGame()
        {
            MatchMaker.instance.BeginGame(matchID);

            Debug.Log("Beginning game");
                
        }

        public void StartGame()
        {
            TargetBeginGame();
        }

        [TargetRpc]
        void TargetBeginGame()
        {
            Debug.Log("Begin MatchID: " + matchID);
            //additively load game scene
            //NetworkManagerTennis.instance.ServerChangeScene("multiplayer");
           SceneManager.LoadScene("multiplayer",LoadSceneMode.Additive);
            Debug.Log("local player name = " + localPlayer.name);
            NetworkManagerTennis.instance.Redo(localPlayer.connectionToClient);

        }
    }
}

