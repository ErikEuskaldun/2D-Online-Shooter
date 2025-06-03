using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using TMPro;
using Unity.Collections;

public class GameInfo : NetworkBehaviour
{
    [SerializeField] TMP_Text txtPlayersOnline;

    NetworkVariable<int> playersOnline = new NetworkVariable<int>(value: 0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    List<PlayerInfo> playersInfo = new List<PlayerInfo>();
    private void Update()
    {
        if (!IsClient)
            return;
        if(Input.GetKeyDown(KeyCode.R))
        {
            ReloadGameInfoServerRpc();
            Debug.Log("RELOAD");
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        ReloadGameInfoLocal();
    }

    public PlayerInfo FindPlayerByUsername(string username)
    {
        foreach (PlayerInfo player in playersInfo)
        {
            if (player.username.Value.ToString() == username)
                return player;
        }
        return null;
    }

    public void ReloadGameInfoLocal()
    {
        playersInfo.Clear();
        PlayerInfo[] findedInfo = FindObjectsOfType<PlayerInfo>();
        foreach (PlayerInfo info in findedInfo)
            playersInfo.Add(info);
        ReloadGameInfo();
    }

    [ServerRpc(RequireOwnership =false)]
    public void ReloadGameInfoServerRpc()
    {
        ReloadPlayerInfoClientRpc();
    }

    private void ReloadGameInfo()
    {
        txtPlayersOnline.text = "Players Online: " + playersInfo.Count;
        foreach (PlayerInfo player in playersInfo)
        {
            string s = player.username.Value.ToString();
            txtPlayersOnline.text += "\n" + s;
        }
    }

    [ClientRpc]
    private void ReloadPlayerInfoClientRpc()
    {
        ReloadGameInfoLocal();
    }
}
