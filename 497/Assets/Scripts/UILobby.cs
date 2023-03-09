using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Mirror
{
    public class UILobby : MonoBehaviour
    {
        public static UILobby instance;

        [Header("Host Join")]
        [SerializeField] TMP_InputField joinMatchInput;
        [SerializeField] Button joinButton;
        [SerializeField] Button hostButton;
        [SerializeField] Canvas lobbyCanvas;

        [Header("Lobby")]
        [SerializeField] Transform UIPlayerParent;
        [SerializeField] GameObject UIPlayerPrefab;
        [SerializeField] TextMeshProUGUI matchIDText;
        [SerializeField] GameObject beginGameButton;

        void Start()
        {
            instance = this; 
        }
        public void Host()
        {
            joinMatchInput.interactable = false;
            joinButton.interactable = false;
            hostButton.interactable = false;
            CharacterNetwork.localPlayer.HostGame();
        }

        public void HostSuccess(bool success)
        {
            if (success)
            {
                lobbyCanvas.enabled = true;
                //we shoudlnt spawm a client on our game
                SpawnPlayerUIPrefab(CharacterNetwork.localPlayer);
                matchIDText.text = "Match: " + CharacterNetwork.localPlayer.matchID;
                beginGameButton.SetActive(true);
            } else
            {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }

        public void Join()
        {
            joinMatchInput.interactable = false;
            joinButton.interactable = false;
            hostButton.interactable = false;
            Debug.Log(joinMatchInput.text);
            CharacterNetwork.localPlayer.JoinGame(joinMatchInput.text);
        }

        public void JoinSuccess(bool success)
        {
            if (success)
            {
                lobbyCanvas.enabled = true;
                SpawnPlayerUIPrefab(CharacterNetwork.localPlayer);
                matchIDText.text = "Match: " + CharacterNetwork.localPlayer.matchID;
            }
            else
            {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }

        public void SpawnPlayerUIPrefab(CharacterNetwork player)
        {
            GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
            newUIPlayer.GetComponent<UIPlayer> ().SetPlayer (player);
        }

        public void BeginGame()
        {
            CharacterNetwork.localPlayer.BeginGame();
        }
    }
}

