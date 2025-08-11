using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager Instance;

    private void Awake()
    {
        if(Instance != null && Instance!=this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        NetworkManager.SceneManager.OnLoadComplete += SceneManager_OnLoadComplete;
    }
    public override void OnNetworkDespawn()
    {
        if (!IsServer)
            return;

        NetworkManager.SceneManager.OnLoadComplete -= SceneManager_OnLoadComplete;
    }

    private void SceneManager_OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!IsServer || clientId != NetworkManager.Singleton.LocalClientId)
            return;

        SceneLoadedClientRPC(sceneName);
    }

    [ClientRpc]
    private void SceneLoadedClientRPC(string sceneName)
    {
        Debug.Log("Hola, estas jugando en el servdor XX al mapa " + sceneName);
    }

    public void Requestrespawn(GameObject player)
    {

    }
}
