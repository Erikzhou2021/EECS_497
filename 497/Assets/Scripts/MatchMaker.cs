using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Security.Cryptography;
using System.Text;

namespace Mirror
{
    [System.Serializable]
    public class Match
    {
        public string matchID;
        public List<CharacterNetwork> players = new List<CharacterNetwork>();

        public Match(string matchID, CharacterNetwork player)
        {
            this.matchID = matchID;
            players.Add(player);
        }

        public Match() {}
    }
    public class MatchMaker : NetworkBehaviour
    {
        public static MatchMaker instance;
        public SyncList<Match> matches = new SyncList<Match>();
        public SyncList<String> matchIDs = new SyncList<String>();
        [SerializeField] GameObject turnManagerPrefab;

        void Start()
        {
            instance = this;
        }
        public bool HostGame(string _matchID, CharacterNetwork _player, out int playerIndex) //*
        {
            playerIndex = -1;
            if(!matchIDs.Contains(_matchID))
            {
                matchIDs.Add(_matchID);
                matches.Add(new Match(_matchID, _player));
                Debug.Log("checking match correct;ly added");
                Debug.Log(matches[0].matchID);
                Debug.Log("Match generated");
                playerIndex = 0;
                return true;
            } else
            {
                Debug.Log("Match ID already exists");
                return false;
            }

        }

        public bool JoinGame(string _matchID, CharacterNetwork _player, out int playerIndex) //*
        {
            playerIndex = -1;
            if (matchIDs.Contains(_matchID))
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    if(matches[i].matchID == _matchID)
                    {
                        matches[i].players.Add(_player);
                        playerIndex = matches[i].players.Count - 1;
                        break;

                    }
                }
                Debug.Log("Match joined");
                return true;
            }
            else
            {
                Debug.Log("Match ID does not exist");
                return false;
            }

        }
        public void BeginGame(string _matchID)
        {
            Debug.Log("entered matchmaker begin game");
            GameObject newTurnManager = Instantiate(turnManagerPrefab);
            newTurnManager.GetComponent<NetworkMatch>().matchId = _matchID.ToGuid();
            TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();
            //Debug.Log(matches.Count);
            for (int i=0; i< matches.Count; i++)
            {
                Debug.Log(matches[i].matchID);
                Debug.Log(_matchID);
                if (matches[i].matchID == _matchID)
                {
                    Debug.Log("Yes equiv");
                    foreach( var player in matches[i].players)
                    {
                        CharacterNetwork _player = player.GetComponent<CharacterNetwork>();
                        turnManager.AddPlayer(_player);
                        _player.StartGame();
                    }
                }
            }
        }
        public static string GetRandomMatchID()
        {
            string _id = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                int random = UnityEngine.Random.Range(0, 36);
                if (random < 26)
                {
                    _id += (char)(random + 65);
                }
                else
                {
                    _id += (random - 26).ToString();
                }
            }
            Debug.Log($"Random Match ID: {_id}");
            return _id;

        }
    }

    public static class MatchExtensions
    {
        public static Guid ToGuid(this string id)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.Default.GetBytes(id);
            byte[] hashBytes = provider.ComputeHash(inputBytes);

            return new Guid(hashBytes);

        }
    }

}

