using System;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private LobbyRoom lobbyRoom;
    [SerializeField] private LobbyList lobbyList;
    [SerializeField] private Button btnRefresh;

    private void Start()
    {
        LobbyManager.Instance.OnGameStarting += LobbyManager_OnGameStarting;
        LobbyManager.Instance.OnLobbyJoined += LobbyManager_OnLobbyJoined;
        LobbyManager.Instance.OnLobbyLeft += LobbyManager_OnLobbyLeft;

        btnRefresh.onClick.AddListener(RefreshLobbyList);
    }

    private void RefreshLobbyList()
    {
        LobbyManager.Instance.ListLobbies();
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.OnGameStarting -= LobbyManager_OnGameStarting;
        LobbyManager.Instance.OnLobbyJoined -= LobbyManager_OnLobbyJoined;
        LobbyManager.Instance.OnLobbyLeft -= LobbyManager_OnLobbyLeft;

        btnRefresh.onClick.RemoveListener(RefreshLobbyList);
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
        lobbyRoom.UpdateLobbyInfo(LobbyManager.Instance.JoinedLobby);
    }

    private void LobbyManager_OnGameStarting(object sender, EventArgs e)
    {
        lobbyList.gameObject.SetActive(false);
        lobbyRoom.gameObject.SetActive(false);
    }
}
