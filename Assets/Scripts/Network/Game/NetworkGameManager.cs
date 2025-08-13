using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager Instance;
    public NetworkVariable<int> time = new NetworkVariable<int>(300, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private const bool EDIT_MODE = true;

    private void Awake()
    {
        if(Instance != null && Instance != this)
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

        if(EDIT_MODE) StartCoroutine(GameTimer(360));
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
        StartCoroutine(GameTimer(360));
    }

    [ClientRpc]
    private void SceneLoadedClientRPC(string sceneName)
    {
        Debug.Log("Hola, estas jugando en el servdor XX al mapa " + sceneName);
    }

    private IEnumerator GameTimer(int seconds)
    {
        time.Value = seconds;

        do
        {
            yield return new WaitForSeconds(1f);
            time.Value--;
        } while (time.Value > 0);
        //TODO: Game end
    }
}
