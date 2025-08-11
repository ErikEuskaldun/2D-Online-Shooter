using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyByCode : MonoBehaviour
{
    [SerializeField] private TMP_InputField ifJoinCode;
    [SerializeField] private Button btnJoinLobby;

    private void Awake()
    {
        btnJoinLobby.onClick.AddListener(JoinByCode);
    }

    public void JoinByCode()
    {
        string joinCode = ifJoinCode.text;
        LobbyManager.Singleton.JoinLobbyByCode(joinCode);
    }
}
