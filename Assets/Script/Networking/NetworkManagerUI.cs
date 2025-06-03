using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] Button btnHost, btnClient;
    [SerializeField] TMP_Text txtBullets;
    [SerializeField] LoginInfo loginInfo;
    [SerializeField] GameObject[] itemsToHide;
    private void Awake()
    {
        btnHost.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });

        btnClient.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }

    public void HideUI()
    {
        btnClient.gameObject.SetActive(false);
        btnHost.gameObject.SetActive(false);
        foreach (GameObject g in itemsToHide)
        {
            g.SetActive(false);
        }
    }

    public void GunUI(string infoText)
    {
        txtBullets.text = infoText;
    }
}
