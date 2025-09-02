using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;

public class GameUI : MonoBehaviour
{
    [Header("Gun")]
    [SerializeField] TMP_Text txtAmmo;
    [SerializeField] Image imgGun;

    [Header("Stats")]
    [SerializeField] TMP_Text txtPing;
    [SerializeField] Color colorPingGood;
    [SerializeField] Color colorPingBad;

    public static GameUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InvokeRepeating("GetPing", 0f, 1f);
    }

    public void SetGun(NetGun gun, Sprite sprite)
    {
        imgGun.sprite = sprite;

        UpdateAmmo(default, gun.currentAmmo.Value);
        gun.currentAmmo.OnValueChanged += UpdateAmmo;
    }

    private void UpdateAmmo(int previousValue, int newValue)
    {
        if (newValue == -2) //Reloading
            txtAmmo.text = "Reloading...";
        else if (newValue == -1) //Infinite ammo
            txtAmmo.text = "-/-";
        else //Current ammo
            txtAmmo.text = newValue + "/-";
    }

    private void GetPing()
    {
        if (NetworkManager.Singleton.NetworkConfig.NetworkTransport.ServerClientId == 0)
            return;
        ulong rtt = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.Singleton.NetworkConfig.NetworkTransport.ServerClientId);
        txtPing.text = rtt + "ms";
        txtPing.color = rtt < 120 ? colorPingGood : colorPingBad;
    }
}
