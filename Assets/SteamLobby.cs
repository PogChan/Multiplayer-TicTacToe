using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    // Callbacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> Joinrequested;
    protected Callback<LobbyEnter_t> LobbyEntered;

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager manager;

    // GameObject
    public GameObject HostButton;
    public GameObject PlayGameButton;
    public GameObject MainGameScreen;
    public Text LobbyNameText;
    public Text SplashScreenText;

    private void Start() {
        if (!SteamManager.Initialized) { return; }
        manager = GetComponent<CustomNetworkManager>();
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        Joinrequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequested);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        PlayGameButton.SetActive(true);
        SplashScreenText.gameObject.SetActive(true);
        HostButton.SetActive(false);
        LobbyNameText.gameObject.SetActive(false);
        MainGameScreen.SetActive(false);
    }

    public void HostLobby() {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }

    public void StartGame() {
        PlayGameButton.SetActive(false);
        SplashScreenText.gameObject.SetActive(false);
        HostButton.SetActive(true);
        LobbyNameText.gameObject.SetActive(true);
    }

    private void OnLobbyCreated(LobbyCreated_t callback) {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }

        Debug.Log("Lobby Created Successfully!!!");

        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
        PlayGameButton.SetActive(false);
        SplashScreenText.gameObject.SetActive(false);
        HostButton.SetActive(false);
        LobbyNameText.gameObject.SetActive(true);
        MainGameScreen.SetActive(true);
    }

    private void OnJoinRequested(GameLobbyJoinRequested_t callback) {
        Debug.Log("Request To Join Lobby!!!");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback) {
        // Everyone
        HostButton.SetActive(false);
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        LobbyNameText.gameObject.SetActive(true);
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        // Client
        if (NetworkServer.active) { return; }
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        manager.StartClient();
    }
}
