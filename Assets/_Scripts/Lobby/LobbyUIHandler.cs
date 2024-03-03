using Eflatun.SceneReference;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUIHandler : MonoBehaviour
{
    [SerializeField] Button _createLobbyButton;
    [SerializeField] Button _joinLobbyButton;
    [SerializeField] SceneReference _gameScene;

    private void Awake()
    {
        _createLobbyButton.onClick.AddListener(CreateGame);
        _joinLobbyButton.onClick.AddListener(JoinGame);
    }

    private async void CreateGame()
    {
        await RelayLobbyHandler.Instance.CreateLobby();
        NetworkSceneLoader.LoadNetworkScene(_gameScene);
    }

    private async void JoinGame()
    {
        await RelayLobbyHandler.Instance.QuickJoinLobby();
    }
}
