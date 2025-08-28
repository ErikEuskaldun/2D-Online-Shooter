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
    NetworkVariable<bool> flip = new NetworkVariable<bool>(value: default, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);

    public NetGun Gun => gun;

    public override void OnNetworkSpawn()
    {
        handTransform = this.transform;
        gunRef.OnValueChanged += OnGunRefChanged;
        flip.OnValueChanged += OnFlipGun;

        StartCoroutine(AssignGunRefDelayed());
    }

    private void OnFlipGun(bool previousValue, bool newValue)
    {
        gun.SpriteRenderer.flipY = newValue;
    }

    public void SetGun(GunScriptable gunScriptable)
    {
        if (!IsOwner)
            return;

        SetGunServerRpc(gunScriptable.id);
    }

    [ServerRpc]
    private void SetGunServerRpc(int gunId)
    {
        if(gun != null)
        {
            NetworkObject oldGun = gun.GetComponent<NetworkObject>();
            if (oldGun.IsSpawned)
            {
                oldGun.Despawn(true);
            }
        }

        GunScriptable scriptable = GunScriptableDatabase.Instance.GetScriptable(gunId);
        GameObject gunObj = Instantiate(scriptable.prefab);
        gun = gunObj.GetComponent<NetGun>();
        gun.Init(OwnerClientId);

        NetworkObject netObj = gun.GetComponent<NetworkObject>();
        netObj.SpawnWithOwnership(OwnerClientId);

        gunRef.Value = netObj;
    }

    private void OnGunRefChanged(NetworkObjectReference previousValue, NetworkObjectReference newValue)
    {
        if (newValue.TryGet(out NetworkObject netObj))
        {
            gun = netObj.GetComponent<NetGun>();
            gun.SpriteRenderer.material = GameDatabase.Instance.GetPlayerMaterial(IsOwner);
            if (IsOwner)
                StartCoroutine(AssignUIWhenReady());

            //Is dead
            if(this.GetComponentInParent<SpriteRenderer>().enabled == false)
            {
                gun.SetLocked(true);
                gun.SpriteRenderer.enabled = false;
            }
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

        bool isFliped;
        if (angle > 90 || angle < -90)
            isFliped = true;
        else
            isFliped = false;

        if(isFliped!=flip.Value) flip.Value = isFliped;
    }
}