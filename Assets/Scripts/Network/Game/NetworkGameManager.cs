using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : NetworkBehaviour
{
    private HashSet<ulong> subscribedClients = new HashSet<ulong>();
    public static NetworkGameManager Instance;
    public NetworkVariable<int> time = new NetworkVariable<int>(300, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public GameMode gameMode = GameMode.FFA;

    public const int FFA_KILLS = 20;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnect;

        // Si ya estaban conectados -> fuerza
        //TODO: Puede dar error
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            NetworkManager_OnClientConnected(client.ClientId);

        //Inicia la cuenta
        StartCoroutine(GameTimer(360));
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
            return;

        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnect;

        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetPlayer netPlayer = player.PlayerObject.GetComponent<NetPlayer>();
            netPlayer.OnPlayerKills -= Player_OnPlayerKills;
        }
    }

    private void NetworkManager_OnClientConnected(ulong obj)
    {
        if (subscribedClients.Contains(obj))
            return; // Ya está suscrito
        NetworkManager.Singleton.ConnectedClients.TryGetValue(obj, out NetworkClient client);
        NetPlayer player = client.PlayerObject.GetComponent<NetPlayer>();
        player.OnPlayerKills += Player_OnPlayerKills;
    }

    private void NetworkManager_OnClientDisconnect(ulong clientId)
    {
        //Para que el servidor no rompa al cerrar
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
            return;

        NetworkSpawnManager spawnManager = NetworkManager.Singleton.SpawnManager;
        if (spawnManager == null)
            return;

        // Buscar todos los objetos que pertenecen al cliente y despawnea
        foreach (NetworkObject obj in spawnManager.GetClientOwnedObjects(clientId))
        {
            if (obj != null && obj.IsSpawned)
                obj.Despawn(true);// true = también destruye el objeto en escena
        }
    }

    private void Player_OnPlayerKills(object sender, NetPlayer.OnPlayerKillsEventArgs e)
    {
        //Killfeed
        string killer = e.killer.username.Value.ToString();
        string victim  = e.victim.username.Value.ToString();
        KillfeedAddClientRpc(killer, victim, e.weaponId);

        //End FFA
        if (e.killer.kills.Value == FFA_KILLS)
            EndGame();
    }

    [ClientRpc]
    private void KillfeedAddClientRpc(string killer, string victim, int weaponId)
    {
        Killfeed.Instance.SpawnKillfeedElement(killer, victim, weaponId);
    }

    private IEnumerator GameTimer(int seconds)
    {
        time.Value = seconds;

        do
        {
            yield return new WaitForSeconds(1f);
            time.Value--;
        } while (time.Value > 0);
        EndGame();
    }

    private void EndGame()
    {
        EndGameClientRpc(TestGetFFAWinner(), 30);
    }

    private string TestGetFFAWinner()
    {
        NetPlayer best = default;
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject playerNetObject = player.PlayerObject;
            NetPlayer netPlayer = playerNetObject.GetComponent<NetPlayer>();
            if (best == default || netPlayer.kills.Value > best.kills.Value)
                best = netPlayer;
        }
        return best.username.Value.ToString();
    }

    [ClientRpc]
    private void EndGameClientRpc(string winner, int seconds)
    {
        Debug.Log("Game Over");
        GameOverUI.Instance.PopUp(winner, seconds);
        if (IsServer)
            StartCoroutine(CountDownToLobby(seconds));
        
    }

    private IEnumerator CountDownToLobby(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        ShutdownClientRpc();
        ResetGameData();
    }

    [ClientRpc]
    private void ShutdownClientRpc()
    {
        GameOverUI.Instance.PopOut();
        StopAllCoroutines();

        if (IsServer)
            ResetGameData();
    }

    private void ResetGameData()
    {
        StartCoroutine(GameTimer(360));

        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject playerNetObject = player.PlayerObject;
            NetPlayer netPlayer = playerNetObject.GetComponent<NetPlayer>();
            netPlayer.kills.Value = 0;
            netPlayer.deaths.Value = 0;
            netPlayer.currentHP.Value = 100;

            var sendParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { player.ClientId }
                }
            };
            netPlayer.SpawnClientRpc(sendParams);
        }
    }
}

public enum GameMode
{
    Null, FFA
}
