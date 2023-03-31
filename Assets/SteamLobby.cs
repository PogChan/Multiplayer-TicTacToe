using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{

    public static SteamLobby Instance;

    // Callbacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> Joinrequested;
    protected Callback<LobbyEnter_t> LobbyEntered;
    // protected Callback<OnConnectionStatusChanged_t> OnConnectionStatusChanged;

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager manager;
    private LobbyController lobbyController;

    // GameObject
    public GameObject MainGameScreen;
    public Text SplashScreenText;

    private void Start() {
        Debug.Log("SteamLobby.Start() called");

        if (!SteamManager.Initialized) { return; }
        if (Instance == null) { Instance = this; }

        manager = GetComponent<CustomNetworkManager>();
        lobbyController = GetComponent<LobbyController>();
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        Joinrequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequested);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyController.UpdatePlayerList();
    }

    public void HostLobby() {
        Debug.Log("SteamLobby.HostLobby() called");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
        lobbyController.UpdatePlayerList();
    }

    private void OnLobbyCreated(LobbyCreated_t callback) {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }

        Debug.Log("Lobby Created Successfully!!!");

        manager.StartHost();
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
        lobbyController.UpdatePlayerList();
    }

    private void OnJoinRequested(GameLobbyJoinRequested_t callback) {
        Debug.Log("Request To Join Lobby!!!");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        lobbyController.UpdatePlayerList();
    }

    private void OnLobbyEntered(LobbyEnter_t callback) {
        Debug.Log("SteamLobby.OnLobbyEntered() called");
        // Everyone
        // CurrentLobbyID = callback.m_ulSteamIDLobby;
   
        // Client
        if (NetworkServer.active) { return; }
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        manager.StartClient();
        lobbyController.UpdatePlayerList();
    }
}
