using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class NetworkGameAction : NetworkBehaviour
{
    [SerializeField] GameInfo gameInfo;
    [SerializeField] RespawnController respawn;

    [ServerRpc(RequireOwnership =false)]
    public void HitServerRpc(FixedString32Bytes username, float damage)
    {
        PlayerInfo playerInf = gameInfo.FindPlayerByUsername(username.ToString());

        playerInf.hp.Value -= damage;
        if (playerInf.hp.Value <= 0)
        {
            playerInf.death.Value++;
            playerInf.hp.Value = 100;
            RespawnClientRpc(username);
        }
        HitMessageClientRpc(username);
    }

    [ClientRpc]
    public void RespawnClientRpc(FixedString32Bytes username)
    {
        PlayerInfo playerInf = gameInfo.FindPlayerByUsername(username.ToString());

        playerInf.Respawn(respawn.respawns[playerInf.team.Value].transform.position);
    }

    [ClientRpc]
    public void HitMessageClientRpc(FixedString32Bytes username)
    {
        Debug.Log(username + " HIT!");
    }
}
