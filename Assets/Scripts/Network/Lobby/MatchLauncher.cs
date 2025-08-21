using System;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchLauncher : MonoBehaviour
{
    [SerializeField] private GameObject GameStartingUI;
    private bool isLoading = false;
    void Start()
    {
        LobbyManager.Instance.OnLobbyDataChange += LobbyManager_OnLobbyDataChange;
        LobbyManager.Instance.OnGameStarting += LobbyManager_OnGameStarting;
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.OnLobbyDataChange -= LobbyManager_OnLobbyDataChange;
        LobbyManager.Instance.OnGameStarting -= LobbyManager_OnGameStarting;
    }

    private void LobbyManager_OnGameStarting(object sender, EventArgs e)
    {
        GameStartingUI.SetActive(true);
    }

    private void LobbyManager_OnLobbyDataChange(object sender, Lobby e)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        Debug.Log(NetworkManager.Singleton.ConnectedClients.Count + "/" + LobbyManager.Instance.JoinedLobby.Players.Count);
        if (NetworkManager.Singleton.ConnectedClients.Count == LobbyManager.Instance.JoinedLobby.Players.Count && !isLoading)
            LoadGame("map_test", "not_implemented"); //Test data
    }

    private void LoadGame(string map, string gamemode)
    {
        isLoading = true;
        NetworkManager.Singleton.SceneManager.LoadScene("map_test", LoadSceneMode.Single);
    }
}
