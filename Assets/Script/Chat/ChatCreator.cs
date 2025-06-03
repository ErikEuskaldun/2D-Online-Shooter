using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class ChatCreator : NetworkBehaviour
{
    [SerializeField] GameObject prefabChatBox;
    [SerializeField] Transform container;

    public void InstantiateChatBox(string text)
    {
        PlayerInfo user = FindObjectOfType<LoginInfo>().localPlayer.GetComponent<PlayerInfo>();
        string username = user.username.Value.ToString();
        int team = user.team.Value;
        ConfirmSendMessageServerRpc(team, username, text);
    }

    [ServerRpc(RequireOwnership =false)]
    public void ConfirmSendMessageServerRpc(int team, FixedString32Bytes username, FixedString64Bytes text)
    {
        string usernameColored;
        if (team == 0) usernameColored = "<style=blue>" + username + ":</style> ";
        else usernameColored = "<style=red>" + username + ":</style> ";

        string finalText = usernameColored + text;

        SendMesageClientRpc(finalText);
    }

    [ClientRpc]
    public void SendMesageClientRpc(FixedString128Bytes text)
    {
        GameObject newChatBox = Instantiate(prefabChatBox, container);
        newChatBox.GetComponent<ChatBox>().SetText(text.ToString());
        FindObjectOfType<ChatSender>().NewMessageOrdenator();
    }


}
