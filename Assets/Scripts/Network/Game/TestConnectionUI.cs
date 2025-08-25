using Unity.Netcode;
using UnityEngine;

public class TestConnectionUI : MonoBehaviour
{
    [SerializeField] private GameObject UI;
    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        UI.SetActive(false);
    }

    public void Client()
    {
        NetworkManager.Singleton.StartClient();
        UI.SetActive(false);
    }
}
