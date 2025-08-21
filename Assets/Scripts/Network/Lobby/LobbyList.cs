using NUnit.Framework;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyList : MonoBehaviour
{
    [SerializeField] private GameObject lobbyCardPrefab;
    [SerializeField] private Transform container;

    private void Start()
    {
        LobbyManager.Instance.OnLobbyListUpdates += OnLobbyListUpdates;
        LobbyManager.Instance.OnLobbyJoined += OnLobbyJoined;
        LobbyManager.Instance.OnLobbyLeft += OnLobbyLeft;

        LobbyManager.Instance.ListLobbies();
    }

    private void OnLobbyLeft(object sender, System.EventArgs e)
    {
        this.gameObject.SetActive(true);
        LobbyManager.Instance.ListLobbies();
    }

    private void OnLobbyJoined(object sender, Lobby lobby)
    {
        this.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.OnLobbyListUpdates -= OnLobbyListUpdates;
        LobbyManager.Instance.OnLobbyJoined -= OnLobbyJoined;
        LobbyManager.Instance.OnLobbyLeft -= OnLobbyLeft;
    }

    private void OnLobbyListUpdates(object sender, List<Lobby> lobbyList)
    {
        //Limpia la lista
        foreach (Transform child in container)
            Destroy(child.gameObject);

        //Genera la lista
        foreach (Lobby lobby in lobbyList)
        {
            LobbyCard lobbyCard = Instantiate(lobbyCardPrefab, container).GetComponent<LobbyCard>();
            lobbyCard.SetLobbyInfo(lobby);
        }
    }
}
