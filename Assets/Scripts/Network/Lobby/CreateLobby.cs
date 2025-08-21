using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreateLobby : MonoBehaviour
{
    [SerializeField] TMP_InputField ifLobbyName;
    [SerializeField] TMP_InputField ifMaxPlayers;
    [SerializeField] Toggle tgIsPrivate;
    [SerializeField] Button btnCreateLobby;

    private void Awake()
    {
        btnCreateLobby.onClick.AddListener(CreateNewLobby);
    }

    private void CreateNewLobby()
    {
        string lobbyName = ifLobbyName.text;
        if (lobbyName == "") lobbyName = "Default Lobby";
        int maxPlayers = int.Parse(ifMaxPlayers.text);
        if (maxPlayers < 2) maxPlayers = 2;
        if(maxPlayers > 12) maxPlayers = 12;
        bool isPrivate = tgIsPrivate.isOn;
        LobbyManager.Instance.CreateLobby(lobbyName, maxPlayers, isPrivate);

        this.gameObject.SetActive(false);
    }
}
