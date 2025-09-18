using UnityEngine;
using Unity.Services.Lobbies.Models;
using TMPro;
using UnityEngine.UI;

public class LobbyRoom : MonoBehaviour
{
    [SerializeField] GameObject lobbyPlayerInfoPrefab;
    [SerializeField] TMP_Text txtLobbyName;
    [SerializeField] TMP_Text txtGamemode;
    [SerializeField] TMP_Text txtInviteCode;
    [SerializeField] TMP_Text txtPlayersOnline;
    [SerializeField] TMP_Text txtMap;
    [SerializeField] Image imgMap;
    [SerializeField] Transform playerPrefabContainer;
    [SerializeField] Button btnStartGame;
    [SerializeField] Button btnLeaveLobby;

    private void Start()
    {
        LobbyManager.Instance.OnLobbyDataChange += OnLobbyDataChange;
        LobbyManager.Instance.OnGameStarting += OnGameStarting;
        btnStartGame.onClick.AddListener(StartGame);
        btnLeaveLobby.onClick.AddListener(LeaveLobby);
    }

    private void StartGame()
    {
        btnStartGame.interactable = false;
        LobbyManager.Instance.StartGame();
    }

    private void LeaveLobby()
    {
        LobbyManager.Instance.LeaveLobby();
    }

    private void OnGameStarting(object sender, System.EventArgs e)
    {
        btnStartGame.interactable = true;
    }

    private void OnLobbyDataChange(object sender, Lobby lobby)
    {
        UpdateLobbyInfo(lobby);
    }

    public void UpdateLobbyInfo(Lobby lobby)
    {
        //Lobby Info
        txtLobbyName.text = lobby.Name;
        txtGamemode.text = lobby.Data["game_mode"].Value;
        txtInviteCode.text = "Invite Code: " + lobby.LobbyCode;

        string mapName = lobby.Data["map"].Value;
        txtMap.text = MapDatabase.Instance.GetDisplayName(mapName);
        imgMap.sprite = MapDatabase.Instance.GetPreviewImage(mapName);
        //imgMap.sprite = ;
        txtPlayersOnline.text = "Players " + lobby.Players.Count + "/" + lobby.MaxPlayers;

        //PlayerList
        foreach (Transform child in playerPrefabContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Player player in lobby.Players)
        {
            LobbyPlayerCard playerCard = Instantiate(lobbyPlayerInfoPrefab, playerPrefabContainer).GetComponent<LobbyPlayerCard>();
            playerCard.SetPlayerInfo(player);
        }

        //Boton Inciar
        if(LobbyManager.Instance.IAmHost())
            btnStartGame.gameObject.SetActive(true);
        else btnStartGame.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        LobbyManager.Instance.OnLobbyDataChange -= OnLobbyDataChange;
        LobbyManager.Instance.OnGameStarting -= OnGameStarting;
        btnStartGame.onClick.RemoveListener(StartGame);
        btnLeaveLobby.onClick.RemoveListener(LeaveLobby);
    }
}
