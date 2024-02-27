using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUserInterfaceController : MonoBehaviour
{
    public void StartAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartAsServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    public void StartAsClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
