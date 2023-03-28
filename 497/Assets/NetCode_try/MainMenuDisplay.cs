using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuDisplay : MonoBehaviour
{
    //[Header("Settings")]
    //[SerializeField] private string gameplaySceneName = "Gameplay";
    //NetworkManager.Singleton.StartHost();
    //NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);

    public void StartHost()
    {
        //NetworkManager.Singleton.StartHost();
        //NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
        ServerManager.Instance.StartHost();
    }

    public void StartServer()
    {
        //NetworkManager.Singleton.StartServer();
        //NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
        ServerManager.Instance.StartServer();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}