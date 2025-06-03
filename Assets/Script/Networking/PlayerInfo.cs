using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;
using TMPro;
using UnityEngine.UI;

public class PlayerInfo : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> username = new NetworkVariable<FixedString32Bytes>(value: "defaultName", readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> kill = new NetworkVariable<int>(value: 0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> death = new NetworkVariable<int>(value: 0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> hp = new NetworkVariable<float>(value: 100f, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> gunId = new NetworkVariable<int>(value: 0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> flipX = new NetworkVariable<bool>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> team = new NetworkVariable<int>(value: 0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);

    GameInfo gameInfo;
    [SerializeField]TMP_Text txtName;
    [SerializeField] Slider slHp;

    private void Start()
    {
        gameInfo = FindObjectOfType<GameInfo>();
        username.OnValueChanged += ReloadUsernameInfo;
        hp.OnValueChanged += ReloadHpInfo;
        gunId.OnValueChanged += ReloadGunInfo;
        flipX.OnValueChanged += ReloadModelFlip;
        team.OnValueChanged += ReloadTeam;
        txtName.text = username.Value.ToString();
        slHp.value = hp.Value/100;
    }

    private void ReloadTeam(int previousValue, int newValue)
    {
        this.GetComponent<PlayerNetwork>().ChangeTeam(newValue);
    }

    private void ReloadModelFlip(bool previousValue, bool newValue)
    {
        this.GetComponent<SpriteRenderer>().flipX = newValue;
    }

    private void ReloadGunInfo(int previousValue, int newValue)
    {
        this.GetComponent<PlayerNetwork>().gunLogic.ChangeGun(newValue);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ReloadHpInfo(default, hp.Value);
        ReloadGunInfo(default, gunId.Value);
        ReloadModelFlip(default, flipX.Value);
        ReloadTeam(default, team.Value);

        if (IsServer)
            hp.Value = 100f;
        if (!IsLocalPlayer)
            return;
        Respawn(FindObjectOfType<RespawnController>().respawns[team.Value].transform.position);
    }

    private void ReloadHpInfo(float previousValue, float newValue)
    {
        float percent = newValue / 100f;
        slHp.value = percent;
    }

    private void ReloadUsernameInfo(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        txtName.text = newValue.ToString();
        gameInfo.ReloadGameInfoLocal();
    }

    public void ChangeGun(int gunValue)
    {
        if (!IsOwner)
            return;

        gunId.Value = gunValue;
    }

    public void SetInfo(string name, int team)
    {
        if (!IsClient || !IsOwner)
            return;

        username.Value = name;
        this.team.Value = team;
    }

    public void Respawn(Vector2 respawnPos)
    {
        if (!IsClient)
            return;

        this.transform.position = respawnPos;
    }

}
