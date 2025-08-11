using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;

public class LobbyPlayerCard : MonoBehaviour
{
    [SerializeField] TMP_Text txtLevel;
    [SerializeField] TMP_Text txtName;
    [SerializeField] TMP_Text txtTeam;

    public void SetPlayerInfo(Player player)
    {
        txtLevel.text = player.Data["level"].Value;
        txtName.text = player.Data["username"].Value;
    }
}
