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
    public override void OnNetworkSpawn()
    {
        Debug.Log("!!!on Network spawn called!!!!");
        if (!IsServer) { return; }
        Debug.Log("and is not server!!!!");

        foreach (var client in ServerManager.Instance.ClientData)
        {
            Debug.Log("client.Value.characterId : " + client.Value.characterId.ToString());
            var character = characterDatabase.GetCharacterById(client.Value.characterId);
            if (character != null)
            {
                //Debug.Log("entered null");

                if (client.Value.characterId == 0) {
                    var spawnPos = firstPlayerSpawn;
                    var characterInstance = Instantiate(character.GameplayPrefab, spawnPos.position, Quaternion.identity);
                    characterInstance.SpawnAsPlayerObject(client.Value.clientId);
                    GameManager.Instance.players.Add(characterInstance.gameObject);
                    characterInstance.gameObject.GetComponent<Player>().playerTeam = client.Value.characterId;
                    cameras[client.Value.characterId].GetComponent<CameraFollow>().setTarget(characterInstance.gameObject.transform);
                }
                if (client.Value.characterId == 1)
                {
                    var spaPos = secondPlayerSpawn;
                    var characterInstance = Instantiate(character.GameplayPrefab, spaPos.position, Quaternion.identity);
                    characterInstance.SpawnAsPlayerObject(client.Value.clientId);
                    GameManager.Instance.players.Add(characterInstance.gameObject);
                    characterInstance.gameObject.GetComponent<Player>().playerTeam = client.Value.characterId;
                    cameras[client.Value.characterId].GetComponent<CameraFollow>().setTarget(characterInstance.gameObject.transform);

                    // spawn ball if two players
                    spaPos = firstPlayerSpawn;
                    ball = Instantiate(ballPrefab, spaPos.position, Quaternion.identity);
                    ball.GetComponent<NetworkObject>().Spawn(true);
                    GameManager.Instance.ball = ball;

                        GameManager.Instance.StartGame();

                }
            }
        }
    }
}