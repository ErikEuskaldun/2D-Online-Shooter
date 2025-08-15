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
        LobbyManager.Singleton.OnLobbyListUpdates += OnLobbyListUpdates;
        LobbyManager.Singleton.OnLobbyJoined += OnLobbyJoined;
        LobbyManager.Singleton.OnLobbyLeft += OnLobbyLeft;

        LobbyManager.Singleton.ListLobbies();
    }

    private void OnLobbyLeft(object sender, System.EventArgs e)
    {
        this.gameObject.SetActive(true);
        LobbyManager.Singleton.ListLobbies();
    }

    private void OnLobbyJoined(object sender, Lobby lobby)
    {
        this.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        LobbyManager.Singleton.OnLobbyListUpdates -= OnLobbyListUpdates;
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
