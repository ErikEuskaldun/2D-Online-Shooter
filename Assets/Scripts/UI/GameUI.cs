using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class GameUI : MonoBehaviour
{
    [SerializeField] TMP_Text txtAmmo;
    [SerializeField] Image imgGun;

    public static GameUI Instance;

    private void Awake()
    {
        Instance = this;
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
}
