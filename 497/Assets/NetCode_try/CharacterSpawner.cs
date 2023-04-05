using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSpawner : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterDatabase characterDatabase;
    //[SerializeField] private CharacterSelectDisplay playerList;
    //var spawnPos = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
    //var characterInstance = Instantiate(character.GameplayPrefab, spawnPos, Quaternion.identity);
    //characterInstance.SpawnAsPlayerObject(client.Value.clientId);
    public Transform firstPlayerSpawn;
    public Transform secondPlayerSpawn;
    public GameObject ballPrefab;
    GameObject ball;
    public GameObject[] cameras;

    //
    public override void OnNetworkSpawn()
    {
        Debug.Log("!!!on Network spawn called!!!!");
        if (!IsServer) { return; }

        foreach (var client in ServerManager.Instance.ClientData)
        {
            var character = characterDatabase.GetCharacterById(client.Value.characterId);
            if (character != null)
            {
                //Debug.Log("entered null");

                if (client.Value.clientId == 1) {
                    var spawnPos = firstPlayerSpawn;
                    var characterInstance = Instantiate(character.GameplayPrefab, spawnPos.position, Quaternion.identity);
                    characterInstance.SpawnAsPlayerObject(client.Value.clientId);
                    Debug.Log(spawnPos.position);
                    Debug.Log(characterInstance.transform.position + "client 0 pos");

                    GameManager.Instance.players.Add(characterInstance.gameObject);
                    characterInstance.gameObject.GetComponent<Player>().playerTeam = (int)client.Value.clientId - 1;
                    cameras[(int)client.Value.clientId - 1].GetComponent<CameraFollow>().setTarget(characterInstance.gameObject.transform);

                    instantiatePlayerClientRpc(characterInstance, (int)client.Value.clientId - 1);
                }
                if (client.Value.clientId == 2)
                {
                    var spaPos = secondPlayerSpawn;
                    var characterInstance = Instantiate(character.GameplayPrefab, spaPos.position, Quaternion.identity);
                    characterInstance.SpawnAsPlayerObject(client.Value.clientId);
                    Debug.Log(spaPos.position);
                    Debug.Log(characterInstance.transform.position + "client 1 pos");

                    GameManager.Instance.players.Add(characterInstance.gameObject);
                    characterInstance.gameObject.GetComponent<Player>().playerTeam = (int)client.Value.clientId - 1;
                    cameras[(int)client.Value.clientId - 1].GetComponent<CameraFollow>().setTarget(characterInstance.gameObject.transform);

                    instantiatePlayerClientRpc(characterInstance, (int)client.Value.clientId - 1);

                    // spawn ball if two players
                    var sp = firstPlayerSpawn;
                    ball = Instantiate(ballPrefab, sp.position, Quaternion.identity);
                    ball.GetComponent<NetworkObject>().Spawn(true);
                    //ulong ballId = ball.GetComponent<NetworkObject>().NetworkObjectId;
                    //Debug.Log(ballId);
                    GameManager.Instance.ball = ball;
                        instantiateClientRpc(ball);

                    

                }
            }
        }

    }
    [ClientRpc]
    public void instantiateClientRpc(NetworkObjectReference ballRef)
    {
        GameManager.Instance.ball = (GameObject)ballRef;
        GameManager.Instance.StartGame();
    }

    [ClientRpc]
    public void instantiatePlayerClientRpc(NetworkObjectReference playerRef, int clientid)
    {
        GameManager.Instance.players.Add(((GameObject)playerRef));
        ((GameObject)playerRef).GetComponent<Player>().playerTeam = clientid;
        cameras[clientid].GetComponent<CameraFollow>().setTarget(((GameObject)playerRef).transform);
        Debug.Log("clientid" + clientid);

    }
}