using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetGunController : NetworkBehaviour
{
    [SerializeField] private GunScriptable testGun;
    private NetGun gun;
    private Transform handTransform;

    private NetworkVariable<NetworkObjectReference> gunRef = new NetworkVariable<NetworkObjectReference>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetGun Gun => gun;

    public override void OnNetworkSpawn()
    {
        handTransform = this.transform;
        gunRef.OnValueChanged += OnGunRefChanged;

        if (IsServer)
        {
            if (GameUtils.EDIT_MODE && OwnerClientId == NetworkManager.LocalClientId)
                HandleSceneLoaded(OwnerClientId, default, default);
            NetworkManager.SceneManager.OnLoadComplete += HandleSceneLoaded;
        }

        StartCoroutine(AssignGunRefDelayed());
    }

    private void HandleSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (clientId != OwnerClientId) return; // Solo spawnea cuando el dueño esté listo

        var gunObj = Instantiate(testGun.prefab);
        gun = gunObj.GetComponent<NetGun>();
        gun.Init(OwnerClientId);

        var netObj = gun.GetComponent<NetworkObject>();
        netObj.SpawnWithOwnership(OwnerClientId);

        gunRef.Value = netObj;

        NetworkManager.SceneManager.OnLoadComplete -= HandleSceneLoaded;
    }

    private void OnGunRefChanged(NetworkObjectReference previousValue, NetworkObjectReference newValue)
    {
        if (newValue.TryGet(out NetworkObject netObj))
        {
            gun = netObj.GetComponent<NetGun>();
            gun.SpriteRenderer.material = GameDatabase.Instance.GetPlayerMaterial(IsOwner);
            if (IsOwner)
                StartCoroutine(AssignUIWhenReady());
        }
    }

    #region Delay
    private IEnumerator AssignGunRefDelayed()
    {

        yield return new WaitUntil(() => gunRef.Value.TryGet(out _));
        OnGunRefChanged(default, gunRef.Value);
    }


    private IEnumerator AssignUIWhenReady()
    {
        // Espera hasta que GameUI.Instance no sea null
        yield return new WaitUntil(() => GameUI.Instance != null);

        gun.SetUI();
    }
    #endregion

    private void Update()
    {
        if (!IsOwner || gun == null)
            return;

        Rotate();

        if (Input.GetKeyDown(KeyCode.Mouse0) && gun.CanShoot())
            gun.ShootServerRpc(gun.GetShotDirection());
    }

    private void LateUpdate()
    {
        if (gun == null)
            return;

        gun.transform.position = handTransform.position;
        gun.transform.localRotation = handTransform.rotation;
    }

    private void Rotate()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 dir = (mousePos - transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}