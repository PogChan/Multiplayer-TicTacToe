using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;


public class PlayerObjectController : NetworkBehaviour
{
    //Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;

    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    { 
        get
        {
            if(manager!= null)
            {
                return manager;

            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Start()
    {
        Debug.Log("PlayerObjectControlled.Start() called");
        //make sure we dont destory on swtich scene.
        DontDestroyOnLoad(this.gameObject);
    }

    private void PlayerReadyUpdate(bool OldValue, bool NewValue)
    {
        if (isServer)
        {
            this.Ready = NewValue;
        }
        if(isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CMDSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
    }
    
    public void ChangeReady()
    {
        if (isOwned)
        {
            CMDSetPlayerReady();
        }
    }

    public override void OnStartAuthority()
    {
        Debug.Log("PlayerObjectControlled.OnStartAuthority() called");
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());

        gameObject.name = "LocalGamePlayer";

        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }



    public override void OnStartClient()
    {
        Debug.Log("PlayerObjectControlled.OnStartClient() called");
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string PlayerName)
    {
        this.PlayerNameUpdate(this.PlayerName, PlayerName);
    }

    public void PlayerNameUpdate(string OldValue, string NewValue)
    {
        if(isServer) //Host
        {
            this.PlayerName = NewValue;
        }
        if(isClient) //Client
        {
            LobbyController.Instance.UpdatePlayerList();

        }
    }

    //host calls this function and then calls the command to call
    public void CanStartGame(string SceneName)
    {
        if (isOwned)
        {
            CMDCanStartGame(SceneName);
        }
    }

    [Command]
    public void CMDCanStartGame(string SceneName)
    {
        manager.StartGame(SceneName);
    }
}
