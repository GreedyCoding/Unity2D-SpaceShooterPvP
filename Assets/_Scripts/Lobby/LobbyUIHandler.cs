using Eflatun.SceneReference;
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUIHandler : MonoBehaviour
{
    [SerializeField] Button _createLobbyButton;
    [SerializeField] Button _quickJoinLobbyButton;
    [SerializeField] Button _joinLobbyWithCodeButton;
    [SerializeField] TMP_InputField _lobbyCodeInputField;
    [SerializeField] SceneReference _gameScene;

    private void Awake()
    {
        _createLobbyButton.onClick.AddListener(CreateGame);
        _quickJoinLobbyButton.onClick.AddListener(QuickJoinLobby);
        _joinLobbyWithCodeButton.onClick.AddListener(JoinLobbyWithCode);
    }

    private async void CreateGame()
    {
        await RelayLobbyHandler.Instance.CreateLobby();
        NetworkSceneLoader.LoadNetworkScene(_gameScene);
    }

    private async void QuickJoinLobby()
    {
        await RelayLobbyHandler.Instance.QuickJoinLobby();
    }

    private async void JoinLobbyWithCode()
    {
        string lobbyCode = _lobbyCodeInputField.text;
        await RelayLobbyHandler.Instance.JoinLobbyWithCode(lobbyCode);
    }


}
