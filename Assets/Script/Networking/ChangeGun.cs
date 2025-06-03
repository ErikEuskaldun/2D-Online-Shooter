using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGun : MonoBehaviour
{
    public void ChangePlayerGun(int gunType)
    {
        PlayerNetwork player = FindObjectOfType<LoginInfo>().localPlayer;

        player.gunLogic.ChangeGun(gunType);
    }
}

[SerializeField]
public enum GunType
{
    Null, Triangle, Box
}
