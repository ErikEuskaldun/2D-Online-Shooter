using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginInfo : MonoBehaviour
{
    public string username;
    public PlayerNetwork localPlayer = default;
    public int team = 0;

    public void SetUsername(string username)
    {
        this.username = username;
    }

    public void SetLocalPlayer(PlayerNetwork player)
    {
        localPlayer = player;
    }

    public void SetGun(int gunIndex)
    {
        localPlayer.GetComponent<PlayerInfo>().ChangeGun(gunIndex);
    }

    public void SetTeam(int team)
    {
        this.team = team;
    }
}
