using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class LoadoutCard : MonoBehaviour
{
    [SerializeField] GunScriptable gunInfo;
    [SerializeField] Image imgGun;
    [SerializeField] TMP_Text txtGun;

    private void Awake()
    {
        imgGun.sprite = gunInfo.icon;
        txtGun.text = gunInfo.gunName;
    }

    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(SetLoadout);
    }

    private void OnDestroy()
    {
        this.GetComponent<Button>().onClick.RemoveListener(SetLoadout);
    }

    private void SetLoadout()
    {
        NetworkObject localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
        NetGunController gunController = localPlayer.GetComponentInChildren<NetGunController>();

        gunController.SetGun(gunInfo);

        LoadoutGameMenu.Instance.SetVisible(false);
    }
}
