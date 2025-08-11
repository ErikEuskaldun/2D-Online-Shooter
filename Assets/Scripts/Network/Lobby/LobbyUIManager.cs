using System;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private LobbyRoom lobbyRoom;
    [SerializeField] private LobbyList lobbyList;

    private void Start()
    {
        LobbyManager.Singleton.OnGameStarting += LobbyManager_OnGameStarting;
        LobbyManager.Singleton.OnLobbyJoined += LobbyManager_OnLobbyJoined;
        LobbyManager.Singleton.OnLobbyLeft += LobbyManager_OnLobbyLeft;
    }

    private void OnDestroy()
    {
        LobbyManager.Singleton.OnGameStarting -= LobbyManager_OnGameStarting;
        LobbyManager.Singleton.OnLobbyJoined -= LobbyManager_OnLobbyJoined;
        LobbyManager.Singleton.OnLobbyLeft -= LobbyManager_OnLobbyLeft;
    }

    private void LobbyManager_OnLobbyLeft(object sender, EventArgs e)
    {
        lobbyList.gameObject.SetActive(true);
        lobbyRoom.gameObject.SetActive(false);
    }

    private void LobbyManager_OnLobbyJoined(object sender, Lobby e)
    {
        lobbyRoom.gameObject.SetActive(true);
        lobbyList.gameObject.SetActive(false);
        lobbyRoom.UpdateLobbyInfo(LobbyManager.Singleton.JoinedLobby);
    }

    private void LobbyManager_OnGameStarting(object sender, EventArgs e)
    {
        lobbyList.gameObject.SetActive(false);
        lobbyRoom.gameObject.SetActive(false);
    }
}
