using Eflatun.SceneReference;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class NetworkSceneLoader
{
    public static void LoadNetworkScene(SceneReference scene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(scene.Name, LoadSceneMode.Single);
    }
}
