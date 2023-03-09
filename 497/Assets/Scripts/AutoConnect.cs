using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutoConnect : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;
    // Start is called before the first frame update
    void Start()
    {
        //missing code frm tutorial but idk if we really need it
        //NOTE: telepathy vs websocket?????? websockets ->webgl
    }
    public void JoinLocal()
    {
        Debug.Log("Join local?");
        // we need to figjure out a way to get the network address autom
        networkManager.networkAddress = "192.168.50.205";
        networkManager.StartClient();

    }
}
