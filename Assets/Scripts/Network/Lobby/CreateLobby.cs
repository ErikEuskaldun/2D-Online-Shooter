using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreateLobby : MonoBehaviour
{
    [SerializeField] TMP_InputField ifLobbyName;
    [SerializeField] TMP_Dropdown ddMapList;
    [SerializeField] TMP_InputField ifMaxPlayers;
    [SerializeField] Toggle tgIsPrivate;
    [SerializeField] Button btnCreateLobby;

    private void Awake()
    {
        btnCreateLobby.onClick.AddListener(CreateNewLobby);
    }

    private void Start()
    {
        List<string> mapNames = MapDatabase.Instance.GetMaps();

        ddMapList.ClearOptions();
        ddMapList.AddOptions(mapNames);
    }

    private void CreateNewLobby()
    {
        string lobbyName = ifLobbyName.text;
        if (lobbyName == "") lobbyName = "Default Lobby";
        int maxPlayers = int.Parse(ifMaxPlayers.text);
        if (maxPlayers < 2) maxPlayers = 2;
        if(maxPlayers > 12) maxPlayers = 12;
        bool isPrivate = tgIsPrivate.isOn;
        string mapName = MapDatabase.Instance.GetMapFromIndex(ddMapList.value);

        LobbyManager.Instance.CreateLobby(lobbyName, maxPlayers, isPrivate, mapName);

        this.gameObject.SetActive(false);
    }
}
