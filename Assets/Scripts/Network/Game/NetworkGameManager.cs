using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager Instance;
    public NetworkVariable<int> time = new NetworkVariable<int>(300, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public GameMode gameMode = GameMode.FFA;

    private const bool EDIT_MODE = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Cleanup()
    {
        NetworkManager.OnClientDisconnectCallback -= this.NetworkManager_OnClientDisconnect;
        NetworkManager.OnClientConnectedCallback -= this.NetworkManager_OnClientConnected;
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= this.SceneManager_OnLoadComplete;
    }


    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnect;
        NetworkManager.SceneManager.OnLoadComplete += SceneManager_OnLoadComplete;
        NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnected;

        if(EDIT_MODE) StartCoroutine(GameTimer(360));
    }

    private void NetworkManager_OnClientDisconnect(ulong clientId)
    {
        // si el NetworkManager ya se está apagando, no hagas nada
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
            return;

        var spawnManager = NetworkManager.Singleton.SpawnManager;
        if (spawnManager == null)
            return;

        // Buscar todos los objetos que pertenecen al cliente y despawnea
        foreach (var obj in spawnManager.GetClientOwnedObjects(clientId))
        {
            if (obj != null && obj.IsSpawned)
                obj.Despawn(true);// true = también destruye el objeto en escena
        }
    }

    private void NetworkManager_OnClientConnected(ulong obj)
    {
        NetworkManager.Singleton.ConnectedClients.TryGetValue(obj, out NetworkClient client);
        NetPlayer player = client.PlayerObject.GetComponent<NetPlayer>();
        player.OnPlayerKills += Player_OnPlayerKills;
    }

    private const int TEST_FFA_KILLS = 20;
    private void Player_OnPlayerKills(object sender, NetPlayer killer)
    {
        if (killer.kills.Value == TEST_FFA_KILLS)
        {
            EndGame();
        }
    }

    public override void OnNetworkDespawn()
    {
        Destroy(this.gameObject);
        if (!IsServer)
            return;

        NetworkManager.SceneManager.OnLoadComplete -= SceneManager_OnLoadComplete;
    }

    private void SceneManager_OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {

        if (!IsServer || clientId != NetworkManager.Singleton.LocalClientId)
            return;

        SceneLoadedClientRPC(sceneName);
        StartCoroutine(GameTimer(360));

    }

    [ClientRpc]
    private void SceneLoadedClientRPC(string sceneName)
    {
        Debug.Log("Hola, estas jugando en el servdor XX al mapa " + sceneName);
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
        //NetworkManager.Singleton.Shutdown();

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
