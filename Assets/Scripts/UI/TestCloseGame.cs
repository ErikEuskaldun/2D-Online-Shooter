using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestCloseGame : MonoBehaviour
{
    [SerializeField] GameObject closeGameGameObject;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            closeGameGameObject.SetActive(!closeGameGameObject.activeSelf);
    }

    public void TestBackToLobby()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Lobby");
    }
}
