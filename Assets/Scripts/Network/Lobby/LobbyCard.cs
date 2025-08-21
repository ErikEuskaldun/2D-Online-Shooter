using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

public class LobbyCard : MonoBehaviour
{
    [SerializeField] TMP_Text txtName;
    [SerializeField] TMP_Text txtGamemode;
    [SerializeField] TMP_Text txtMap;
    [SerializeField] TMP_Text txtPlayers;
    [SerializeField] TMP_Text txtBuildVersion;

    private Lobby lobby;

    private void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(JoinLobby);
    }

    public void SetLobbyInfo(Lobby lobby)
    {
        this.lobby = lobby;

        txtName.text = lobby.Name;
        txtGamemode.text = lobby.Data["game_mode"].Value;
        txtMap.text = lobby.Data["map"].Value;
        txtPlayers.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        txtBuildVersion.text = "v" + lobby.Data["build_version"].Value;
    }

    public void JoinLobby()
    {
        LobbyManager.Instance.JoinLobbyByID(lobby.Id);
    }
}
