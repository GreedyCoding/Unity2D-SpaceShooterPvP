using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayLobbyHandler : PersistentSingleton<RelayLobbyHandler>
{
    const float LOBBY_HEARTBEAT_INTERVAL = 20f;
    const float LOBBY_POLL_INTERVAL = 60f;
    const string KEY_JOIN_CODE = "RelayJoinCode";
    const string DTLS_ENCRYPTION = "dtls";
    const string WSS_ENCRYPTION = "wss";

    [SerializeField] string _lobbyName = "Lobby";
    [SerializeField] int _maxPlayers = 2;
    [SerializeField] EncryptionTypeEnum _encryption = EncryptionTypeEnum.DTLS;

    private Lobby _currentLobby;

    private CountdownTimer _heartbeatTimer = new CountdownTimer(LOBBY_HEARTBEAT_INTERVAL);
    private CountdownTimer _pollForUpdatesTimer = new CountdownTimer(LOBBY_POLL_INTERVAL);

    private string _connectionType => _encryption == EncryptionTypeEnum.DTLS ? DTLS_ENCRYPTION : WSS_ENCRYPTION;

    public string PlayerID { get; private set; }
    public string PlayerName { get; private set; }

    async void Start()
    {
        await Authenticate();

        _heartbeatTimer.OnTimerStop += () =>
        {
            HandleHeartbeatAsync();
            _heartbeatTimer.Start();
        };

        _pollForUpdatesTimer.OnTimerStop += () =>
        {
            HandlePollForUpdatesAsync();
            _pollForUpdatesTimer.Start();
        };
    }

    private async Task Authenticate()
    {
        await Authenticate("Player-" + UnityEngine.Random.Range(0,1000).ToString());
    }

    private async Task Authenticate(string playerName)
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(playerName);

            await UnityServices.InitializeAsync(options);
        }

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerId}");
        };

        if(!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            PlayerID = AuthenticationService.Instance.PlayerId;
            PlayerName = playerName;
        }
    }

    public async Task CreateLobby()
    {
        try
        {
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false
            };

            _currentLobby = await LobbyService.Instance.CreateLobbyAsync(_lobbyName, _maxPlayers, options);

            Debug.Log($"Created Lobby: {_currentLobby.Name} with code {_currentLobby.LobbyCode}");

            _heartbeatTimer.Start();
            _pollForUpdatesTimer.Start();

            await LobbyService.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, _connectionType));

            NetworkManager.Singleton.StartHost();
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError($"Failed to create lobby: {ex.Message}");
        }
    }

    public async Task QuickJoinLobby()
    {
        try
        {
            _currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            _pollForUpdatesTimer.Start();

            string relayJoinCode = _currentLobby.Data[KEY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, _connectionType));

            NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError($"Failed to guick join lobby: {ex.Message}");
        }
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_maxPlayers - 1);
            return allocation;
        } 
        catch(RelayServiceException ex)
        {
            Debug.LogError($"Failed to allocate relay: {ex.Message}");
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch(RelayServiceException ex)
        {
            Debug.LogError($"Failed to get relay join code: {ex.Message}");
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string relayJoinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            return joinAllocation;
        }
        catch(RelayServiceException ex)
        {
            Debug.LogError($"Failed to join relay: {ex.Message}");
            return default;

        }
    }

    private async Task HandleHeartbeatAsync()
    {
        try
        {
            await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
            Debug.Log($"Sent to heartbeat lobby: {_currentLobby.Name}");

        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError($"Failed to heartbeat lobby: {ex.Message}");
        }
    }

    private async Task HandlePollForUpdatesAsync()
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
            Debug.Log($"Polled for updates on lobby: {lobby.Name}");
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError($"Failed to poll for updates on lobby: {ex.Message}");
        }
    }
}
